//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;
using Lokad.Rules;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Defines the model for a single feedback report.
    /// </summary>
    public sealed class FeedbackFileModel : Model
    {
        /// <summary>
        /// The path to the file that contains the report.
        /// </summary>
        private string m_FilePath;
        
        /// <summary>
        /// The date on which the report was generated.
        /// </summary>
        private DateTimeOffset m_ReportDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackFileModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="path">The path to the file that contains the feedback file.</param>
        /// <param name="date">The date on which the feedback report was generated.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public FeedbackFileModel(IContextAware context, string path, DateTimeOffset date)
            : base(context)
        {
            {
                Enforce.Argument(() => path);
                Enforce.Argument(() => path, StringIs.NotEmpty);
            }

            m_FilePath = path;
            m_ReportDate = date;
        }

        /// <summary>
        /// Gets the file path for the report.
        /// </summary>
        public string Path
        {
            get
            {
                return m_FilePath;
            }
        }

        /// <summary>
        /// Gets the date on which the report was generated.
        /// </summary>
        public DateTimeOffset Date
        {
            get
            {
                return m_ReportDate;
            }
        }
    }
}
