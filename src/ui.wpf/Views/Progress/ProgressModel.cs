//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Progress;

namespace Apollo.UI.Wpf.Views.Progress
{
    /// <summary>
    /// Defines the model for the progress reporting in the application.
    /// </summary>
    public sealed class ProgressModel : Model
    {
        /// <summary>
        /// The object that reports the progress for the application.
        /// </summary>
        private readonly ICollectProgressReports m_Reporter;

        /// <summary>
        /// The description for the process for which progress is being reported.
        /// </summary>
        private string m_Description;

        /// <summary>
        /// The current progress value.
        /// </summary>
        private double m_Progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="progressReporter">The object that provides progress notifications for all the processes.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="progressReporter"/> is <see langword="null" />.
        /// </exception>
        public ProgressModel(IContextAware context, ICollectProgressReports progressReporter)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => progressReporter);
            }

            m_Reporter = progressReporter;
            m_Reporter.OnStartProgress += HandleOnStartProgress;
            m_Reporter.OnProgress += HandleOnProgress;
            m_Reporter.OnStopProgress += HandleOnStopProgress;
        }

        private void HandleOnStartProgress(object sender, EventArgs e)
        {
            Progress = 0.0;
        }

        private void HandleOnProgress(object sender, ProgressEventArgs e)
        {
            Progress = e.Progress / 100.0;
        }

        private void HandleOnStopProgress(object sender, EventArgs e)
        {
            Progress = 0.0;
        }

        /// <summary>
        /// Gets the description for the process for which progress is being reported.
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }

            private set
            {
                m_Description = value;
                Notify(() => Description);
            }
        }

        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        public double Progress
        {
            get
            {
                return m_Progress;
            }

            private set
            {
                var input = value;
                if (input < 0)
                {
                    input = 0.0;
                }

                if (input > 1.0)
                {
                    input = 1.0;
                }

                m_Progress = input;
                Notify(() => Progress);
            }
        }
    }
}
