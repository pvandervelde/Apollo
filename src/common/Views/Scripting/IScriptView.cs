//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Scripting;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// The interface for objects that display information about
    /// a script.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "We need an interface for the view because Prism needs it.")]
    public interface IScriptView : IView<ScriptModel>
    {
        /// <summary>
        /// Provides the view with a function that can build output pipes.
        /// </summary>
        /// <param name="builder">The function that builds script output pipes.</param>
        void OutputPipeBuilder(Func<ISendScriptOutput> builder);
    }
}
