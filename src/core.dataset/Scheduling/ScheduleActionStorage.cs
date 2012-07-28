//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Dataset.Properties;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Stores schedule actions by ID.
    /// </summary>
    internal sealed class ScheduleActionStorage : IStoreScheduleActions, IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the storage field.
        /// </summary>
        private const byte StorageIndex = 0;

        /// <summary>
        /// Maps the action to the information describing the action.
        /// </summary>
        private sealed class ActionMap
        {
            /// <summary>
            /// The action.
            /// </summary>
            private readonly IScheduleAction m_Action;

            /// <summary>
            /// The information describing the action.
            /// </summary>
            private readonly ScheduleActionInformation m_Info;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionMap"/> class.
            /// </summary>
            /// <param name="information">The information describing the action.</param>
            /// <param name="action">The action.</param>
            public ActionMap(ScheduleActionInformation information, IScheduleAction action)
            {
                {
                    Debug.Assert(information != null, "The action information should not be a null reference.");
                    Debug.Assert(action != null, "The action should not be a null reference.");
                }

                m_Info = information;
                m_Action = action;
            }

            /// <summary>
            /// Gets the action information.
            /// </summary>
            public ScheduleActionInformation Information
            {
                get
                {
                    return m_Info;
                }
            }

            /// <summary>
            /// Gets the action.
            /// </summary>
            public IScheduleAction Action
            {
                get
                {
                    return m_Action;
                }
            }
        }

        /// <summary>
        /// Creates a default storage that isn't linked to a timeline.
        /// </summary>
        /// <returns>The newly created instance.</returns>
        internal static ScheduleActionStorage BuildStorageWithoutTimeline()
        {
            return new ScheduleActionStorage(new HistoryId(), new DictionaryHistory<ScheduleElementId, ActionMap>());
        }

        /// <summary>
        /// Creates a new <see cref="ScheduleActionStorage"/> instance with the given parameters.
        /// </summary>
        /// <param name="id">The ID that will be used to uniquely identify the object to the history system.</param>
        /// <param name="members">The collection containing all the member collections.</param>
        /// <param name="constructorArguments">The constructor arguments.</param>
        /// <returns>The newly created instance.</returns>
        public static ScheduleActionStorage BuildStorage(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 1, "There should only be 1 member.");
            }

            var pair = members.First();
            if (pair.Item1 != StorageIndex)
            {
                throw new UnknownMemberNameException();
            }

            var actions = pair.Item2 as IDictionaryTimelineStorage<ScheduleElementId, ActionMap>;
            return new ScheduleActionStorage(id, actions);
        }

        /// <summary>
        /// The collection of schedule actions.
        /// </summary>
        [FieldIndexForHistoryTracking(StorageIndex)]
        private readonly IDictionaryTimelineStorage<ScheduleElementId, ActionMap> m_Actions;

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleActionStorage"/> class.
        /// </summary>
        /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
        /// <param name="knownActions">The collection that contains the actions.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="knownActions"/> is <see langword="null" />.
        /// </exception>
        private ScheduleActionStorage(
            HistoryId id,
            IDictionaryTimelineStorage<ScheduleElementId, ActionMap> knownActions)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => knownActions);
            }

            m_HistoryId = id;
            m_Actions = knownActions;
        }

        /// <summary>
        /// Gets the ID which relates the object to the timeline.
        /// </summary>
        public HistoryId HistoryId
        {
            get
            {
                return m_HistoryId;
            }
        }

        /// <summary>
        /// Adds the <see cref="IScheduleAction"/> object with the variables it affects and the dependencies for that action.
        /// </summary>
        /// <param name="action">The action that should be stored.</param>
        /// <param name="name">The name of the action that is being described by this information object.</param>
        /// <param name="summary">The summary of the action that is being described by this information object.</param>
        /// <param name="description">The description of the action that is being described by this information object.</param>
        /// <param name="produces">The variables that are affected by the action.</param>
        /// <param name="dependsOn">The variables for which data should be available in order to execute the action.</param>
        /// <returns>An object identifying and describing the action.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="action"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="produces"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dependsOn"/> is <see langword="null" />.
        /// </exception>
        public ScheduleActionInformation Add(
            IScheduleAction action,
            string name,
            string summary,
            string description,
            IEnumerable<IScheduleVariable> produces,
            IEnumerable<IScheduleDependency> dependsOn)
        {
            {
                Lokad.Enforce.Argument(() => action);
                Lokad.Enforce.Argument(() => produces);
                Lokad.Enforce.Argument(() => dependsOn);
            }

            var id = new ScheduleElementId();
            var info = new ScheduleActionInformation(id, name, summary, description, produces, dependsOn);
            m_Actions.Add(id, new ActionMap(info, action));

            return info;
        }

        /// <summary>
        /// Replaces the action current stored against the given ID with a new one.
        /// </summary>
        /// <param name="actionToReplace">The ID of the action that should be replaced.</param>
        /// <param name="newAction">The new action.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionToReplace"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="newAction"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleActionException">
        ///     Thrown if <paramref name="actionToReplace"/> is not linked to a known action.
        /// </exception>
        public void Update(
            ScheduleElementId actionToReplace,
            IScheduleAction newAction)
        {
            {
                Lokad.Enforce.Argument(() => actionToReplace);
                Lokad.Enforce.Argument(() => newAction);
                Lokad.Enforce.With<UnknownScheduleActionException>(
                    m_Actions.ContainsKey(actionToReplace), 
                    Resources.Exceptions_Messages_UnknownScheduleAction);
            }

            var oldInfo = m_Actions[actionToReplace].Information;
            var info = new ScheduleActionInformation(
                actionToReplace, 
                oldInfo.Name, 
                oldInfo.Summary, 
                oldInfo.Description, 
                oldInfo.Produces(), 
                oldInfo.DependsOn());
            m_Actions[actionToReplace] = new ActionMap(info, newAction);
        }

        /// <summary>
        /// Removes the action with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the action that should be removed.</param>
        public void Remove(ScheduleElementId id)
        {
            if ((id != null) && m_Actions.ContainsKey(id))
            {
                m_Actions.Remove(id);
            }
        }

        /// <summary>
        /// Returns a value indicating if there is an action with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>
        /// <see langword="true" /> if there is an action with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Contains(ScheduleElementId id)
        {
            return (id != null) && m_Actions.ContainsKey(id);
        }

        /// <summary>
        /// Returns the action that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the action.</param>
        /// <returns>The action.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleActionException">
        ///     Thrown if <paramref name="id"/> is not linked to a known action.
        /// </exception>
        public IScheduleAction Action(ScheduleElementId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownScheduleActionException>(
                    m_Actions.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownScheduleAction);
            }

            return m_Actions[id].Action;
        }

        /// <summary>
        /// Returns the action information for the action that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the action.</param>
        /// <returns>The action information.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleActionException">
        ///     Thrown if <paramref name="id"/> is not linked to a known action.
        /// </exception>
        public ScheduleActionInformation Information(ScheduleElementId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownScheduleActionException>(
                    m_Actions.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownScheduleAction);
            }

            return m_Actions[id].Information;
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<ScheduleElementId> GetEnumerator()
        {
            foreach (var key in m_Actions.Keys)
            {
                yield return key;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
