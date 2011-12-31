﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Tracks the history of a series of objects.
    /// </summary>
    internal sealed class Timeline : ITimeline, ICreateSnapshots
    {
        /// <summary>
        /// The collection that tracks all the dependencies.
        /// </summary>
        private readonly ValueAtTimeStorage<IEnumerable<UpdateFromHistoryDependency>> m_Dependencies
            = new ValueAtTimeStorage<IEnumerable<UpdateFromHistoryDependency>>();

        /// <summary>
        /// The collection that tracks the history of object creations.
        /// </summary>
        /// <remarks>
        /// At the moment we never delete an object that has survived at least one mark. Objects that don't
        /// survive a mark are only recorded in an temporary collection. Based on this we can track
        /// objects based on their creation moment.
        /// </remarks>
        private readonly ValueAtTimeStorage<List<HistoryId>> m_CreatedObjectHistory
            = new ValueAtTimeStorage<List<HistoryId>>();

        /// <summary>
        /// The collection of objects that track the timeline for an individual object that currently
        /// exist or has existed in the past.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     We expect that this collection is going to get big really quickly. At the moment
        ///     we just use a dictionary but really there should be a custom storage object so that
        ///     we don't run into silly constraints (due to the Large Object Heap etc.).
        ///     </para>
        ///     <para>
        ///     We pretty much expect this collection to only grow during its life span. As long as
        ///     the timeline isn't removed we want to track all the objects so there should only be
        ///     growth. This does mean however that we probably have to get more clever about
        ///     the allocation of the internal collection.
        ///     </para>
        /// </remarks>
        private readonly Dictionary<HistoryId, IFollowObjectTimeline> m_ObjectTimelines
            = new Dictionary<HistoryId, IFollowObjectTimeline>();

        /// <summary>
        /// The collection of objects that track a timeline for a recently created, but non-marked, object.
        /// </summary>
        private readonly Dictionary<HistoryId, IFollowObjectTimeline> m_NonMarkedObjectTimelines
            = new Dictionary<HistoryId, IFollowObjectTimeline>();

        /// <summary>
        /// The collection that maps a name to a time marker.
        /// </summary>
        private readonly Dictionary<string, TimeMarker> m_NameToTimeMap
            = new Dictionary<string, TimeMarker>();

        /// <summary>
        /// The function that creates history storage objects based on the given type of the storage.
        /// </summary>
        private readonly Func<Type, IStoreTimelineValues> m_StorageBuilder;

        /// <summary>
        /// The time marker that was created on the last <see cref="TimeMarker"/> action.
        /// </summary>
        private TimeMarker m_Current = TimeMarker.TheBeginOfTime;

        /// <summary>
        /// The time marker that is the furthest into the future.
        /// </summary>
        private TimeMarker m_Latest = TimeMarker.TheBeginOfTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeline"/> class.
        /// </summary>
        /// <param name="storageBuilder">The function that builds instances of storage types.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageBuilder"/> is <see langword="null" />.
        /// </exception>
        public Timeline(Func<Type, IStoreTimelineValues> storageBuilder)
        {
            {
                Lokad.Enforce.Argument(() => storageBuilder);
            }

            m_StorageBuilder = storageBuilder;
        }

        /// <summary>
        /// Adds a new object to the timeline.
        /// </summary>
        /// <typeparam name="T">The type of object that should be added to the timeline.</typeparam>
        /// <param name="objectBuilder">
        /// The function that is used to create a new object of type <typeparamref name="T"/> with the given
        /// collection of member field objects.
        /// </param>
        /// <returns>The newly created object of type <typeparamref name="T"/>.</returns>
        public T AddToTimeline<T>(Func<HistoryId, IEnumerable<Tuple<string, IStoreTimelineValues>>, T> objectBuilder)
            where T : class, IAmHistoryEnabled
        {
            var history = new ObjectTimeline<T>(
                new HistoryId(),
                m_StorageBuilder,
                objectBuilder);

            m_NonMarkedObjectTimelines.Add(history.Id, history);
            history.AddToTimeline();

            return history.Object;
        }

        /// <summary>
        /// Removes the object with the given <see cref="HistoryId"/> from the timeline.
        /// </summary>
        /// <param name="id">The ID of the object that should be removed.</param>
        public void RemoveFromTimeline(HistoryId id)
        {
            if ((id == null) || (!m_ObjectTimelines.ContainsKey(id) && !m_NonMarkedObjectTimelines.ContainsKey(id)))
            {
                return;
            }

            if (m_ObjectTimelines.ContainsKey(id))
            {
                // Only delete living objects. Don't delete objects that are already dead
                // and don't delete objects that don't exist yet.
                var timeline = m_ObjectTimelines[id];
                if (timeline.IsAlive())
                {
                    timeline.DeleteFromTimeline();
                }
            }
            else 
            {
                // The timeline exists but the object doesn't have a creation time. Thus the creation time
                // must be the next marker. Hence we delete the object and then nuke the timeline.
                m_NonMarkedObjectTimelines.Remove(id);
            }
        }

        /// <summary>
        /// Returns the object that belongs to the given <see cref="HistoryId"/>.
        /// </summary>
        /// <typeparam name="T">The type of object that should be returned.</typeparam>
        /// <param name="id">The ID of the object that should be returned.</param>
        /// <returns>
        ///     The object that belongs to the given <see cref="HistoryId"/> if the object exists at the current
        ///     <see cref="TimeMarker"/>; otherwise, <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ObjectUnknownToTimelineException">
        ///     Thrown if <paramref name="id"/> is for an object that has never existed in the curren timeline.
        /// </exception>
        public T IdToObject<T>(HistoryId id) where T : class, IAmHistoryEnabled
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<ObjectUnknownToTimelineException>(
                    HasObjectEverExisted(id),
                    Resources.Exceptions_Messages_ObjectUnknownToTimeline_WithId,
                    id);
            }

            var trackingObject = m_ObjectTimelines.ContainsKey(id) ? m_ObjectTimelines[id] : m_NonMarkedObjectTimelines[id];
            var timeline = trackingObject as ObjectTimeline<T>;
            Debug.Assert(timeline != null, "Could not find the correct timeline.");

            return timeline.Object;
        }

        /// <summary>
        /// Returns a value indicating if the object that belongs to the given <see cref="HistoryId"/>
        /// exist at the current <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <returns>
        /// <see langword="true" /> if the object exists at the current <see cref="TimeMarker"/>; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool DoesObjectExistCurrently(HistoryId id)
        {
            if (!HasObjectEverExisted(id))
            {
                return false;
            }

            if (!m_ObjectTimelines.ContainsKey(id) && !m_NonMarkedObjectTimelines.ContainsKey(id))
            {
                return false;
            }

            var timeline = m_ObjectTimelines.ContainsKey(id) ? m_ObjectTimelines[id] : m_NonMarkedObjectTimelines[id];
            return timeline.IsAlive();
        }

        /// <summary>
        /// Returns a value indicating if the object that belongs to the given <see cref="HistoryId"/>
        /// exist at any point in time.
        /// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <returns>
        /// <see langword="true" /> if the object exists at any point in time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasObjectEverExisted(HistoryId id)
        {
            return (id != null)
                && (m_ObjectTimelines.ContainsKey(id) || m_NonMarkedObjectTimelines.ContainsKey(id));
        }

        /// <summary>
        /// Gets the current time marker.
        /// </summary>
        public TimeMarker Current
        {
            get
            {
                return m_Current;
            }
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to roll-back to a previous value.
        /// </summary>
        public bool CanRollBack
        {
            get
            {
                return m_Current > TimeMarker.TheBeginOfTime;
            }
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to roll-forward to a next value.
        /// </summary>
        public bool CanRollForward
        {
            get
            {
                return m_Current < m_Latest;
            }
        }

        /// <summary>
        /// Rolls the state back to the given point in time and provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The point in time to roll-back to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-back.</param>
        public void RollBackTo(TimeMarker mark, TimelineTraveller traveller = null)
        {
            {
                Lokad.Enforce.Argument(() => mark);
                Lokad.Enforce.With<InvalidTimeMarkerException>(
                    mark >= TimeMarker.TheBeginOfTime,
                    Resources.Exceptions_Messages_CannotMovePastBeginningOfTime);
                Lokad.Enforce.With<InvalidTimeMarkerException>(
                    mark <= m_Current,
                    Resources.Exceptions_Messages_CannotRollBackToAPointInTheFuture);
            }

            // Check that we are able to perform the roll-back
            VerifyThatPastDependenciesAllowRollBack(mark);

            RaiseOnRollingBack();

            RollBackDependencies(mark);
            RollBackCreatedObjects(mark);

            // Roll back all the changes
            foreach (var pair in m_ObjectTimelines)
            {
                pair.Value.RollBackTo(mark);
            }
            
            // Clear all the objects that were created but weren't marked
            m_NonMarkedObjectTimelines.Clear();

            // Make the state consistent before sending the traveller back
            m_Current = mark;

            // Send the traveller back to the owner
            if (traveller != null)
            {
                SendTravellerBackToOwner(traveller);
            }

            RaiseOnRolledBack();
        }

        private void VerifyThatPastDependenciesAllowRollBack(TimeMarker mark)
        {
            m_Dependencies.TrackBackwardsInTime(
                (t, d) =>
                {
                    foreach (var dependency in d)
                    {
                        if (((dependency.Blocks == ChangeBlocker.RollBack) || (dependency.Blocks == ChangeBlocker.RollBackAndRollForward))
                            && (!dependency.CanExecuteChange()))
                        {
                            throw new UnableToPerformRollBackDueToBlockingDependencyException();
                        }
                    }

                    return t > mark;
                });
        }

        private void RollBackDependencies(TimeMarker mark)
        {
            if (!m_Dependencies.IsAtBeginOfTime())
            {
                m_Dependencies.RollBackTo(mark);
            }
        }

        private void RollBackCreatedObjects(TimeMarker mark)
        {
            if (!m_CreatedObjectHistory.IsAtBeginOfTime())
            {
                m_CreatedObjectHistory.RollBackTo(
                    mark,
                    (t, ids) =>
                    {
                        foreach (var id in ids)
                        {
                            Debug.Assert(m_ObjectTimelines.ContainsKey(id), "Missing the timeline for an object.");
                            var timeline = m_ObjectTimelines[id];
                            timeline.RollBackTo(mark);
                        }
                    });
            }
        }

        private void SendTravellerBackToOwner(TimelineTraveller traveller)
        {
            {
                Debug.Assert(traveller != null, "The traveller should not be a null reference.");
            }

            var id = traveller.Owner;
            if (m_ObjectTimelines.ContainsKey(id))
            {
                var owner = m_ObjectTimelines[id] as IReceiveMessagesFromTheFuture;
                if (owner != null)
                {
                    owner.ReceiveTraveller(traveller);
                }
            }
        }

        /// <summary>
        /// Rolls the state back to the named point in time and provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The named point in time to roll-back to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-back.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="mark"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="mark"/> is an empty string.
        /// </exception>
        public void RollBackTo(string mark, TimelineTraveller traveller = null)
        {
            {
                Lokad.Enforce.Argument(() => mark);
                Lokad.Enforce.Argument(() => mark, Lokad.Rules.StringIs.NotEmpty);
            }

            if (!m_NameToTimeMap.ContainsKey(mark))
            {
                throw new UnknownTimeMarkerException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnknownTimeMarker_WithName,
                        mark));
            }

            var marker = m_NameToTimeMap[mark];
            RollBackTo(marker, traveller);
        }

        /// <summary>
        /// An event that is raised just before a roll-back takes place.
        /// </summary>
        public event EventHandler<EventArgs> OnRollingBack;

        private void RaiseOnRollingBack()
        {
            var local = OnRollingBack;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event that is raised just after a roll-back has taken place.
        /// </summary>
        public event EventHandler<EventArgs> OnRolledBack;

        private void RaiseOnRolledBack()
        {
            var local = OnRolledBack;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Rolls the state forward to the given point in time an provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The point in time to roll-foward to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-forward.</param>
        public void RollForwardTo(TimeMarker mark, TimelineTraveller traveller = null)
        {
            {
                Lokad.Enforce.Argument(() => mark);
                Lokad.Enforce.With<InvalidTimeMarkerException>(
                    mark >= m_Current,
                    Resources.Exceptions_Messages_CannotRollForwardToAPointInThePast);
            }

            VerifyThatFutureDependenciesAllowRollForward(mark);

            RaiseOnRollingForward();

            RollFowardDependencies(mark);
            RollForwardCreatedObjects(mark);

            // Roll the changes forward
            foreach (var pair in m_ObjectTimelines)
            {
                pair.Value.RollForwardTo(mark);
            }

            // Clear all the objects that were created but weren't marked
            m_NonMarkedObjectTimelines.Clear();

            // Make the state consistent before sending the traveller back
            m_Current = mark;

            // Send the traveller back to the owner
            if (traveller != null)
            {
                SendTravellerBackToOwner(traveller);
            }

            RaiseOnRolledForward();
        }

        private void VerifyThatFutureDependenciesAllowRollForward(TimeMarker mark)
        {
            m_Dependencies.TrackForwardsInTime(
                (t, d) =>
                {
                    foreach (var dependency in d)
                    {
                        if (((dependency.Blocks == ChangeBlocker.RollForward) || (dependency.Blocks == ChangeBlocker.RollBackAndRollForward))
                            && (!dependency.CanExecuteChange()))
                        {
                            throw new UnableToPerformRollForwardDueToBlockingDependencyException();
                        }
                    }

                    return t > mark;
                });
        }

        private void RollFowardDependencies(TimeMarker mark)
        {
            if (!m_Dependencies.IsAtEndOfTime())
            {
                m_Dependencies.RollForwardTo(mark);
            }
        }

        private void RollForwardCreatedObjects(TimeMarker mark)
        {
            if (!m_CreatedObjectHistory.IsAtEndOfTime())
            {
                m_CreatedObjectHistory.RollForwardTo(
                    mark,
                    (t, ids) =>
                    {
                        foreach (var id in ids)
                        {
                            Debug.Assert(m_ObjectTimelines.ContainsKey(id), "Missing the timeline for an object.");
                            var timeline = m_ObjectTimelines[id];
                            timeline.RollForwardTo(mark);
                        }
                    });
            }
        }

        /// <summary>
        /// Rolls the state forward to the named point in time an provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The named point in time to roll-foward to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-forward.</param>
        public void RollForwardTo(string mark, TimelineTraveller traveller = null)
        {
            {
                Lokad.Enforce.Argument(() => mark);
                Lokad.Enforce.Argument(() => mark, Lokad.Rules.StringIs.NotEmpty);
            }

            if (!m_NameToTimeMap.ContainsKey(mark))
            {
                throw new UnknownTimeMarkerException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnknownTimeMarker_WithName,
                        mark));
            }

            var marker = m_NameToTimeMap[mark];
            RollForwardTo(marker, traveller);
        }

        /// <summary>
        /// An even that is raised just before a roll-forward takes place.
        /// </summary>
        public event EventHandler<EventArgs> OnRollingForward;

        private void RaiseOnRollingForward()
        {
            var local = OnRollingForward;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An even that is raised just after a roll-forward has taken place.
        /// </summary>
        public event EventHandler<EventArgs> OnRolledForward;

        private void RaiseOnRolledForward()
        {
            var local = OnRolledForward;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stores the current state and returns the <see cref="TimeMarker"/> that belongs to this change set.
        /// </summary>
        /// <returns>The time marker for the stored state.</returns>
        public TimeMarker Mark()
        {
            return Mark(null, null);
        }

        /// <summary>
        /// Stores the current state and the dependencies and returns the <see cref="TimeMarker"/> that belongs to 
        /// this change set.
        /// </summary>
        /// <param name="dependencies">The dependencies that indicate if the current change set can be rolled back or rolled forward.</param>
        /// <returns>The time marker for the stored state.</returns>
        public TimeMarker Mark(IEnumerable<UpdateFromHistoryDependency> dependencies)
        {
            return Mark(null, dependencies);
        }

        /// <summary>
        /// Stores the current state with the given name and returns the <see cref="TimeMarker"/> that 
        /// belongs to this change set.
        /// </summary>
        /// <param name="name">The name for the state.</param>
        /// <returns>The time marker for the stored state.</returns>
        public TimeMarker Mark(string name)
        {
            return Mark(name, null);
        }

        /// <summary>
        /// Stores the current state with the given name and the dependencies and returns the <see cref="TimeMarker"/> that 
        /// belongs to this change set.
        /// </summary>
        /// <param name="name">The name for the state.</param>
        /// <param name="dependencies">The dependencies that indicate if the current change set can be rolled back or rolled forward.</param>
        /// <returns>The time marker for the stored state.</returns>
        public TimeMarker Mark(string name, IEnumerable<UpdateFromHistoryDependency> dependencies)
        {
            // All these objects have been created between the last mark and the new one.
            // They're about to become embedded in history forever.
            var newIds = new List<HistoryId>(m_NonMarkedObjectTimelines.Count);
            foreach (var pair in m_NonMarkedObjectTimelines)
            {
                m_ObjectTimelines.Add(pair.Key, pair.Value);
                newIds.Add(pair.Key);
            }

            m_NonMarkedObjectTimelines.Clear();

            // Remove all the objects that only exist in the future. They won't
            // be created any more after the current mark.
            m_CreatedObjectHistory.TrackForwardsInTime(
                (t, ids) =>
                {
                    foreach (var id in ids)
                    {
                        if (m_ObjectTimelines.ContainsKey(id))
                        {
                            m_ObjectTimelines.Remove(id);
                        }
                    }

                    return true;
                });

            m_Current = !string.IsNullOrEmpty(name) ? m_Current.Next(name) : m_Current.Next();
            m_CreatedObjectHistory.StoreCurrent(m_Current, newIds);
            foreach (var pair in m_ObjectTimelines)
            {
                pair.Value.Mark(m_Current);
            }

            if (dependencies != null)
            {
                m_Dependencies.StoreCurrent(m_Current, dependencies);
            }

            if (!string.IsNullOrEmpty(name))
            {
                m_NameToTimeMap.Add(name, m_Current);
            }

            m_Latest = m_Current;
            RaiseOnMark(m_Current);

            return m_Current;
        }

        /// <summary>
        /// An event that is raised when the current change set is stored in the timeline.
        /// </summary>
        public event EventHandler<TimelineMarkEventArgs> OnMark;

        private void RaiseOnMark(TimeMarker mark)
        {
            {
                Debug.Assert(mark != null, "This event should only be raised if there is a new time mark");
            }

            var local = OnMark;
            if (local != null)
            {
                local(this, new TimelineMarkEventArgs(mark));
            }
        }

        /// <summary>
        /// Creates a new snapshot and stores it.
        /// </summary>
        public void CreateSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts recording history and returns a change set for those changes.
        /// </summary>
        /// <returns>
        /// The change set that records the changes when it is disposed.
        /// </returns>
        public IChangeSet RecordHistory()
        {
            return new ChangeSet(this);
        }

        /// <summary>
        /// Starts recording history and returns a named change set for those changes.
        /// </summary>
        /// <param name="name">The name of the change set.</param>
        /// <returns>
        /// The change set that records the changes when it is disposed.
        /// </returns>
        public IChangeSet RecordHistory(string name)
        {
            return new ChangeSet(this, name);
        }
    }
}
