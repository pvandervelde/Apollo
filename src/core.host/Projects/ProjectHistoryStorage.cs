//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Utilities.History;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Stores the project specific information that has a timeline.
    /// </summary>
    internal sealed class ProjectHistoryStorage : IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the name field.
        /// </summary>
        private const byte NameIndex = 0;

        /// <summary>
        /// The history index of the summary field.
        /// </summary>
        private const byte SummaryIndex = 1;

        /// <summary>
        /// Creates a new instance of the <see cref="ProjectHistoryStorage"/> class with the given 
        /// history information.
        /// </summary>
        /// <param name="id">The history ID for the project storage.</param>
        /// <param name="members">The collection that holds all the members for the current object.</param>
        /// <param name="constructorArguments">The optional constructor arguments.</param>
        /// <returns>A new instance of the <see cref="ProjectHistoryStorage"/> class.</returns>
        /// <exception cref="UnknownMemberException">
        /// Thrown if the <paramref name="members"/> collection contains data for an unknown member field.
        /// </exception>
        internal static ProjectHistoryStorage Build(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 2, "There should only be two members.");
            }

            IVariableTimeline<string> name = null;
            IVariableTimeline<string> summary = null;
            foreach (var member in members)
            {
                if (member.Item1 == NameIndex)
                {
                    name = member.Item2 as IVariableTimeline<string>;
                    continue;
                }

                if (member.Item1 == SummaryIndex)
                {
                    summary = member.Item2 as IVariableTimeline<string>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new ProjectHistoryStorage(id, name, summary);
        }

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// The name of the project.
        /// </summary>
        [FieldIndexForHistoryTracking(NameIndex)]
        private readonly IVariableTimeline<string> m_Name;

        /// <summary>
        /// The summary for the project.
        /// </summary>
        [FieldIndexForHistoryTracking(SummaryIndex)]
        private readonly IVariableTimeline<string> m_Summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectHistoryStorage"/> class.
        /// </summary>
        /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
        /// <param name="name">The timeline storage that stores the name of the project.</param>
        /// <param name="summary">The timeline storage that stores the summary of the project.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="summary"/> is <see langword="null" />.
        /// </exception>
        private ProjectHistoryStorage(
            HistoryId id,
            IVariableTimeline<string> name,
            IVariableTimeline<string> summary)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => summary);
            }

            m_HistoryId = id;
            m_Name = name;
            m_Summary = summary;
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
        /// Gets or sets the name for the project.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name.Current;
            }

            set
            {
                m_Name.Current = value;
            }
        }

        /// <summary>
        /// Gets or sets the summary for the project.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Summary.Current;
            }

            set
            {
                m_Summary.Current = value;
            }
        }
    }
}
