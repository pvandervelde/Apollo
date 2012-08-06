//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Apollo.UI.Common.Models;

namespace Apollo.UI.Common.Views.Settings
{
    /// <summary>
    /// Defines a <see cref="Model"/> that describes a file that was opened recently with the application.
    /// </summary>
    public sealed class MostRecentlyUsedModel : Model
    {
        /// <summary>
        /// The full path of the file that was opened.
        /// </summary>
        private readonly MostRecentlyUsed m_Mru;

        /// <summary>
        /// Initializes a new instance of the <see cref="MostRecentlyUsedModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="mostRecentlyUsed">The object that stores information about a most recently used file.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="mostRecentlyUsed"/> is <see langword="null" />.
        /// </exception>
        public MostRecentlyUsedModel(IContextAware context, MostRecentlyUsed mostRecentlyUsed)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => mostRecentlyUsed);
            }

            m_Mru = mostRecentlyUsed;
        }

        /// <summary>
        /// Gets the name of the file without extension.
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(m_Mru.FilePath);
            }
        }

        /// <summary>
        /// Gets the full path for the file.
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_Mru.FilePath;
            }
        }

        /// <summary>
        /// Gets the date and time the file was last opened.
        /// </summary>
        public DateTimeOffset LastTimeOpened
        {
            get 
            {
                return m_Mru.LastTimeOpened;
            }
        }
    }
}
