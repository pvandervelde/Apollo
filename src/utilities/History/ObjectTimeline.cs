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
            s_Members = new List<Tuple<string, Type>>();

            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                if (fieldType.GetCustomAttributes(typeof(DefineAsHistoryTrackingInterfaceAttribute), true).Length > 0)
                {
                    s_Members.Add(new Tuple<string, Type>(field.Name, fieldType));
                }
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
        private readonly Func<HistoryId, IEnumerable<Tuple<string, IStoreTimelineValues>>, T> m_ObjectBuilder;

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
            Func<HistoryId, IEnumerable<Tuple<string, IStoreTimelineValues>>, T> objectBuilder)
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
            [DebuggerStepThrough]
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
            [DebuggerStepThrough]
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
            [DebuggerStepThrough]
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
            [DebuggerStepThrough]
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
        /// <exception cref="ObjectHasAlreadyBeenAddedToTheTimelineException">
        ///     Thrown if an object has previously been added to the timeline and the timeline
        ///     is not at the start of time.
        /// </exception>
        public void AddToTimeline()
        {
            {
                Lokad.Enforce.With<ObjectHasAlreadyBeenAddedToTheTimelineException>(
                    !IsAlive() && IsAtStartOfTime(),
                    Resources.Exceptions_Messages_ObjectHasAlreadyBeenCreated);
            }

            if (m_CreationTime != null)
            {
                // The object has been created before but we're shifting the
                // creation time. Clear all the member collections.
                foreach (var member in m_Members)
                {
                    member.ForgetAllHistory();
                }
            }
            
            m_Object = Resurrect();
            m_CreationTime = null;
            m_DeletionTime = null;
        }

        private bool IsAtStartOfTime()
        {
            foreach (var timeline in m_Members)
            {
                if (!timeline.IsAtBeginOfTime())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes the object from the timeline.
        /// </summary>
        /// <exception cref="ObjectHasNotBeenCreatedYetException">
        /// Thrown if the object has not been created yet.
        /// </exception>
        /// <exception cref="CannotRemoveNonLivingObjectException">
        /// Thrown if an object has already been removed.
        /// </exception>
        public void DeleteFromTimeline()
        {
            {
                Lokad.Enforce.With<ObjectHasNotBeenCreatedYetException>(
                    m_CreationTime != null,
                    Resources.Exceptions_Messages_ObjectHasNotBeenCreatedYet);

                // The object either: has to be alive with no deletion time 
                //   OR
                // Alive after a roll-back with a different deletion time.
                Lokad.Enforce.With<CannotRemoveNonLivingObjectException>(
                    IsAlive(),
                    Resources.Exceptions_Messages_CannotRemoveNonLivingObject);
            }

            foreach (var member in m_Members)
            {
                member.ForgetTheFuture();
            }

            ClearObject();
            m_DeletionTime = null;
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
            foreach (var timeline in m_Members)
            {
                timeline.RollBackToStart();
            }
            
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

            return m_ObjectBuilder(Id, timelines);
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
            foreach (var timeline in m_Members)
            {
                timeline.RollForwardTo(m_DeletionTime);
            }

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
                m_Object = Resurrect();
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
            if (!IsAlive() && (m_CreationTime == null))
            {
                return;
            }

            if (!IsAlive() && (m_DeletionTime == null))
            {
                m_DeletionTime = marker;
                return;
            }

            foreach (var timeline in m_Members)
            {
                Debug.Assert(timeline != null, "One of the member timelines has not been initialized.");
                timeline.StoreCurrent(marker);
            }

            if ((m_Object != null) && (m_CreationTime == null))
            {
                m_CreationTime = marker;
            }

            if (m_DeletionTime != null)
            {
                m_DeletionTime = null;
            }
        }
    }
}
