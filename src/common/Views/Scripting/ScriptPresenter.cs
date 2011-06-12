//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Input;
using Apollo.UI.Common.Commands;
using Autofac;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;

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
            var closeScriptCommand = m_Container.Resolve<CloseScriptCommand>();
            var closeViewCommand = m_Container.Resolve<CloseViewCommand>(
                new TypedParameter(typeof(IEventAggregator), m_Container.Resolve<IEventAggregator>()),
                new TypedParameter(typeof(string), CommonRegionNames.Content),
                new TypedParameter(typeof(Parameter), new ScriptParameter()));
            var compositeCommand = new CompositeCommand();
            compositeCommand.RegisterCommand(closeScriptCommand);
            compositeCommand.RegisterCommand(closeViewCommand);

            var runScriptCommand = m_Container.Resolve<RunScriptCommand>();
            var cancelScriptCommand = m_Container.Resolve<CancelScriptRunCommand>();
            View.Model = new ScriptModel(compositeCommand, runScriptCommand, cancelScriptCommand);
        }
    }
}
