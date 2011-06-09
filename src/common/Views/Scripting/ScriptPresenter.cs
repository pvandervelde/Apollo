//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Common.Commands;
using Autofac;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// The presenter for the <see cref="ScriptModel"/>.
    /// </summary>
    public sealed class ScriptPresenter : Presenter<IScriptView, ScriptModel, ScriptParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public ScriptPresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var closeCommand = m_Container.Resolve<CloseScriptCommand>();
            var runScriptCommand = m_Container.Resolve<RunScriptCommand>();
            var cancelScriptCommand = m_Container.Resolve<CancelScriptRunCommand>();
            View.Model = new ScriptModel(closeCommand, runScriptCommand, cancelScriptCommand);
        }
    }
}
