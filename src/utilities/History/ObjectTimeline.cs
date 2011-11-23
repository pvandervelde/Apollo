//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores The timeline values for the member variables of an instance of type of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the timeline should be stored.</typeparam>
    internal sealed class ObjectTimeline<T> : IFollowObjectTimeline where T : class, IAmHistoryEnabled
    {
        /// <summary>
        /// Stores the member information in the same order as the timelines are stored.
        /// </summary>
        private static readonly IList<Tuple<string, Type>> s_Members;

        /// <summary>
        /// Initializes static members of the <see cref="ObjectTimeline{T}"/> class.
        /// </summary>
        static ObjectTimeline()
        {
            var type = typeof(T);
            var members = from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                          where typeof(IVariableTimeline<>).IsAssignableToOpenGenericType(field.FieldType)
                          select field;

            s_Members = new List<Tuple<string, Type>>();
            foreach (var field in members)
            {
                s_Members.Add(new Tuple<string, Type>(field.Name, field.FieldType));
            }
        }

        /// <summary>
        /// The collection that tracks the timelines for the different member variables.
        /// </summary>
        private readonly IList<IStoreTimelineValues> m_Members;

        /// <summary>
        /// The ID number of the object in the timeline.
        /// </summary>
        private readonly HistoryId m_Id;

        /// <summary>
        /// The function that builds the object.
        /// </summary>
        private readonly Func<IEnumerable<Tuple<string, IStoreTimelineValues>>, T> m_ObjectBuilder;

        /// <summary>
        /// The object that is being tracked in the timeline.
        /// </summary>
        private T m_Object;

        /// <summary>
        /// The point on the timeline where the tracked object is created.
        /// </summary>
        private TimeMarker m_CreationTime;

        /// <summary>
        /// The point on the timeline where the tracked object is destroyed or deleted.
        /// </summary>
        private TimeMarker m_DeletionTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectTimeline{T}"/> class.
        /// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <param name="storageBuilder">The function that is used to generate the correct type of timeline storage based on a given type.</param>
        /// <param name="objectBuilder">The function that is used to create new instances of the type <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="objectBuilder"/> is <see langword="null" />.
        /// </exception>
        public ObjectTimeline(
            HistoryId id,
            Func<Type, IStoreTimelineValues> storageBuilder,
            Func<IEnumerable<Tuple<string, IStoreTimelineValues>>, T> objectBuilder)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => storageBuilder);
                Lokad.Enforce.Argument(() => objectBuilder);
            }

            m_Id = id;
            m_ObjectBuilder = objectBuilder;

            m_Members = new List<IStoreTimelineValues>(s_Members.Count);
            foreach (var pair in s_Members)
            {
                var storage = storageBuilder(pair.Item2);
                m_Members.Add(storage);
            }
        }

        /// <summary>
        /// Gets the ID number of the timeline object.
        /// </summary>
        public HistoryId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the object that is related to the current timeline.
        /// </summary>
        public T Object
        {
            get
            {
                return m_Object;
            }
        }

        /// <summary>
        /// Gets the point on the timeline where the object was created.
        /// </summary>
        public TimeMarker CreationTime
        {
            get
            {
                return m_CreationTime;
            }
        }

        /// <summary>
        /// Gets the point on the timeline where the object was destroyed.
        /// </summary>
        public TimeMarker DeletionTime
        {
            get
            {
                return m_DeletionTime;
            }
        }

        /// <summary>
        /// Returns a value indicating if the object is currently alive or not.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the object is currently alive; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsAlive()
        {
            return m_Object != null;
        }

        /// <summary>
        /// Adds the object to the timeline and sets the creation time.
        /// </summary>
        /// <param name="creationTime">The creation time for the object.</param>
        /// <exception cref="ObjectHasAlreadyBeenAddedToTheTimelineException">
        ///     Thrown if an object is already present.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="creationTime"/> is <see langword="null" />.
        /// </exception>
        public void AddToTimeline(TimeMarker creationTime)
        {
            {
                Lokad.Enforce.With<ObjectHasAlreadyBeenAddedToTheTimelineException>(
                    IsAlive(),
                    Resources.Exceptions_Messages_ObjectHasAlreadyBeenCreated);
                Lokad.Enforce.Argument(() => creationTime);
            }

            m_Object = Resurrect();
            m_CreationTime = creationTime;
            m_DeletionTime = null;
        }

        /// <summary>
        /// Deletes the object from the timeline.
        /// </summary>
        /// <param name="deletionTime">The time at which the object is deleted from the timeline.</param>
        /// <exception cref="ObjectHasAlreadyBeenRemovedFromTheTimelineException">
        ///     Thrown if an object has already been removed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="deletionTime"/> is <see langword="null" />.
        /// </exception>
        public void DeleteFromTimeline(TimeMarker deletionTime)
        {
            {
                Lokad.Enforce.With<ObjectHasAlreadyBeenRemovedFromTheTimelineException>(
                    !IsAlive() && (m_DeletionTime != null),
                    Resources.Exceptions_Messages_ObjectHasAlreadyBeenRemoved);

                Lokad.Enforce.Argument(() => deletionTime);
            }

            ClearObject();
            m_DeletionTime = deletionTime;
        }

        private void ClearObject()
        {
            if (m_Object != null)
            {
                m_Object.Dispose();
                m_Object = null;
            }
        }

        /// <summary>
        /// Rolls the current value back to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollBackTo(TimeMarker marker)
        {
            if (m_CreationTime == null)
            {
                // The object doesn't exist and we're trying to roll-back even further.
                return;
            }

            if ((m_DeletionTime != null) && (marker > m_DeletionTime))
            {
                // Rolling back to a time after we've already died. Do nothing
                return;
            }

            // Roll-back to a point in time before we existed
            if (marker < m_CreationTime)
            {
                RollBackToStart();
            }
            else
            {
                RollBackToPointInTime(marker);
            }

            RaiseOnRollBack();
        }

        private void RollBackToStart()
        {
            // Just clear the object. Don't bother resetting the timeline because nobody will be able to 
            // get to the timeline values anyway. At least not until we get an object back and then we'll
            // move the timeline back to where we want it anyway.
            ClearObject();
        }

        private void RollBackToPointInTime(TimeMarker marker)
        {
            foreach (var timeline in m_Members)
            {
                Debug.Assert(timeline != null, "One of the member timelines has not been initialized.");
                timeline.RollBackTo(marker);
            }

            if (m_Object == null)
            {
                m_Object = Resurrect();
            }
        }

        private T Resurrect()
        {
            {
                Debug.Assert(m_Object == null, "Can only ressurect dead objects.");
            }

            var timelines = new List<Tuple<string, IStoreTimelineValues>>();

            Debug.Assert(s_Members.Count == m_Members.Count, "There should be as many timelines as there are members.");
            for (var i = 0; i < s_Members.Count; i++)
            {
                timelines.Add(new Tuple<string, IStoreTimelineValues>(s_Members[i].Item1, m_Members[i]));
            }

            return m_ObjectBuilder(timelines);
        }

        /// <summary>
        /// An event raised when the object state is rolled back.
        /// </summary>
        public event EventHandler<EventArgs> OnRollBack;

        private void RaiseOnRollBack()
        {
            var local = OnRollBack;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollForwardTo(TimeMarker marker)
        {
            if (m_CreationTime == null)
            {
                // The object has never been created so we don't know what state to roll forward to.
                // Just pretend we don't care.
                return;
            }

            // Roll-forward to a point in time after we died
            if ((m_DeletionTime != null) && (marker >= m_DeletionTime))
            {
                RollForwardToEnd();
            }
            else
            {
                RollForwardToPointInTime(marker);
            }

            RaiseOnRollForward();
        }

        private void RollForwardToEnd()
        {
            // Just clear the object. Don't bother resetting the timeline because nobody will be able to 
            // get to the timeline values anyway. At least not until we get an object back and then we'll
            // move the timeline back to where we want it anyway.
            ClearObject();
        }

        private void RollForwardToPointInTime(TimeMarker marker)
        {
            foreach (var timeline in m_Members)
            {
                Debug.Assert(timeline != null, "One of the member timelines has not been initialized.");
                timeline.RollForwardTo(marker);
            }

            if (m_Object == null)
            {
                Resurrect();
            }
        }

        /// <summary>
        /// An event raised when the object state is rolled forward.
        /// </summary>
        public event EventHandler<EventArgs> OnRollForward;

        private void RaiseOnRollForward()
        {
            var local = OnRollForward;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stores the current object state for the given marker.
        /// </summary>
        /// <param name="marker">The marker which indicates at which point on the timeline the data is stored.</param>
        public void Mark(TimeMarker marker)
        {
            if (!IsAlive())
            {
                return;
            }

            foreach (var timeline in m_Members)
            {
                Debug.Assert(timeline != null, "One of the member timelines has not been initialized.");
                timeline.StoreCurrent(marker);
            }
        }
    }
}
