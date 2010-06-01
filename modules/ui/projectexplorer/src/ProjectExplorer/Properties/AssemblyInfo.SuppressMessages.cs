//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

// Global suppress messages
[module: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]

// XAML suppress messages
[module: SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope="member", Target="Apollo.ProjectExplorer.Views.Menu.MenuView.#System.Windows.Markup.IComponentConnector.Connect(System.Int32,System.Object)")]

// Internal classes that seem to be unused but really are used
[module: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Apollo.ProjectExplorer.Events.ShowViewEvent")]
[module: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Apollo.ProjectExplorer.Views.Shell.ShellModel")]
[module: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Apollo.ProjectExplorer.Views.Menu.MenuView")]
[module: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Apollo.ProjectExplorer.Views.About.AboutWindow")]
[module: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Apollo.ProjectExplorer.Events.Listeners.ShowViewEventListener")]
[module: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Apollo.ProjectExplorer.Events.ShowViewRequest`1")]

[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="Apollo.ProjectExplorer.Views.Shell.ShellWindow.#.ctor(Apollo.UI.Common.Commands.ExitCommand)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="Apollo.ProjectExplorer.Commands.ShowAboutWindowCommand.#.ctor(Microsoft.Practices.Composite.Events.IEventAggregator)")]