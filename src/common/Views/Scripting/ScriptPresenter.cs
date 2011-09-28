//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Apollo.Core.Host.UserInterfaces.Scripting;
using Apollo.UI.Common.Commands;
using Apollo.UI.Common.Properties;
using Autofac;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Win32;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// The presenter for the <see cref="ScriptModel"/>.
    /// </summary>
    public sealed class ScriptPresenter : Presenter<IScriptView, ScriptModel, ScriptParameter>
    {
        /// <summary>
        /// The collection that maps a file filter to a language.
        /// </summary>
        private static readonly IDictionary<string, ScriptLanguage> s_FileExtensionToLanguageMap
            = new SortedList<string, ScriptLanguage> 
                {
                    { Resources.ScriptFileExtension_Language_IronPython, ScriptLanguage.IronPython },
                    { Resources.ScriptFileExtension_Language_IronRuby, ScriptLanguage.IronRuby },
                };

        private static ScriptLanguage LanguageFromFileExtension(string extension)
        {
            return s_FileExtensionToLanguageMap[extension];
        }

        private static string ScriptFileFilter()
        {
            return Resources.ScriptFileExtensionFilter_Language_IronPython + "|"
                + Resources.ScriptFileExtensionFilter_Language_IronRuby;
        }

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
            var context = m_Container.Resolve<IContextAware>();
            View.Model = new ScriptModel(context) 
                {
                    ScriptLanguage = new ScriptDescriptionModel(context, ScriptLanguage.IronPython),
                    SyntaxVerifier = m_Container.Resolve<IHostScripts>().VerifySyntax(ScriptLanguage.IronPython),
                };
            View.OutputPipeBuilder(() => m_Container.Resolve<ISendScriptOutput>());

            var compositeCommand = CreateCloseCommand();
            var newScriptCommand = CreateNewScriptCommand();
            var openScriptCommand = CreateOpenScriptCommand();

            View.Model.CloseCommand = compositeCommand;
            View.Model.NewScriptCommand = newScriptCommand;
            View.Model.OpenScriptCommand = openScriptCommand;
            View.Model.RunCommand = m_Container.Resolve<RunScriptCommand>();
            View.Model.CancelRunCommand = m_Container.Resolve<CancelScriptRunCommand>();
        }

        private CompositeCommand CreateCloseCommand()
        {
            var context = m_Container.Resolve<IContextAware>();
            var closeScriptCommand = m_Container.Resolve<CloseScriptCommand>();
            var closeViewCommand = m_Container.Resolve<CloseViewCommand>(
                new TypedParameter(typeof(IEventAggregator), m_Container.Resolve<IEventAggregator>()),
                new TypedParameter(typeof(string), CommonRegionNames.Content),
                new TypedParameter(typeof(Parameter), new ScriptParameter(context)));
            var compositeCommand = new CompositeCommand();
            compositeCommand.RegisterCommand(closeScriptCommand);
            compositeCommand.RegisterCommand(closeViewCommand);
            return compositeCommand;
        }

        private NewScriptCommand CreateNewScriptCommand()
        {
            var context = m_Container.Resolve<IContextAware>();
            Func<Tuple<bool, ScriptDescriptionModel>> selectScriptLanguage =
                () =>
                {
                    var presenter = (IPresenter)m_Container.Resolve(typeof(SelectScriptLanguagePresenter));
                    var view = m_Container.Resolve(presenter.ViewType) as ISelectScriptLanguageView;
                    presenter.Initialize(view, new SelectScriptLanguageParameter(context));

                    var window = view as Window;
                    window.Owner = Application.Current.MainWindow;
                    if (window.ShowDialog() ?? false)
                    {
                        return new Tuple<bool, ScriptDescriptionModel>(true, view.Model.SelectedLanguage);
                    }
                    else
                    {
                        return new Tuple<bool, ScriptDescriptionModel>(false, null);
                    }
                };

            Action<ScriptDescriptionModel, ISyntaxVerifier> storeVerifier =
                (l, v) =>
                {
                    View.Model.ScriptLanguage = l;
                    View.Model.SyntaxVerifier = v;
                };

            var newScriptCommand = m_Container.Resolve<NewScriptCommand>(
                new TypedParameter(typeof(IHostScripts), m_Container.Resolve<IHostScripts>()),
                new TypedParameter(typeof(Func<Tuple<bool, ScriptDescriptionModel>>), selectScriptLanguage),
                new TypedParameter(typeof(Action<ScriptDescriptionModel, ISyntaxVerifier>), storeVerifier));
            return newScriptCommand;
        }

        private OpenScriptCommand CreateOpenScriptCommand()
        {
            var context = m_Container.Resolve<IContextAware>();
            Func<Tuple<FileInfo, ScriptDescriptionModel>> selectScriptLanguage =
                () =>
                {
                    var dlg = new OpenFileDialog
                        {
                            AddExtension = true,
                            CheckPathExists = true,
                            DereferenceLinks = true,
                            Filter = ScriptFileFilter(),
                            FilterIndex = 0,
                            Multiselect = false,
                            Title = Resources.ScriptPresenter_SelectScriptFile,
                            ValidateNames = true,
                        };
                    if (dlg.ShowDialog(Application.Current.MainWindow) ?? false)
                    {
                        return new Tuple<FileInfo, ScriptDescriptionModel>(
                            new FileInfo(dlg.FileName), 
                            new ScriptDescriptionModel(context, LanguageFromFileExtension(Path.GetExtension(dlg.FileName))));
                    }
                    else
                    {
                        return new Tuple<FileInfo, ScriptDescriptionModel>(null, null);
                    }
                };

            Action<ScriptDescriptionModel, FileInfo, ISyntaxVerifier> storeVerifier =
                (d, f, v) =>
                {
                    View.Model.ScriptLanguage = d;
                    View.Model.ScriptFile = f.FullName;
                    View.Model.SyntaxVerifier = v;
                };

            var openScriptCommand = m_Container.Resolve<OpenScriptCommand>(
                new TypedParameter(typeof(IHostScripts), m_Container.Resolve<IHostScripts>()),
                new TypedParameter(typeof(Func<Tuple<FileInfo, ScriptDescriptionModel>>), selectScriptLanguage),
                new TypedParameter(typeof(Action<ScriptDescriptionModel, FileInfo, ISyntaxVerifier>), storeVerifier));
            return openScriptCommand;
        }
    }
}
