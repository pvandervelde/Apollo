//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Defines an ID for a plug-in repository.
    /// </summary>
    public sealed class PluginRepositoryId : Id<PluginRepositoryId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginRepositoryId"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PluginRepositoryId(string value) 
            : base(value)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override PluginRepositoryId Clone(string value)
        {
            return new PluginRepositoryId(value);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}
