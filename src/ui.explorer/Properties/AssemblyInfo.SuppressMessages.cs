//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

// Global suppress messages
// XAML suppress messages
[module: SuppressMessage(
    "Microsoft.Performance", 
    "CA1800:DoNotCastUnnecessarily", 
    Scope = "member", 
    Target = "Apollo.UI.Explorer.Views.Menu.MenuView.#System.Windows.Markup.IComponentConnector.Connect(System.Int32,System.Object)")]

// Internal classes that seem to be unused but really are used
[module: SuppressMessage(
    "Microsoft.Performance", 
    "CA1811:AvoidUncalledPrivateCode", 
    Scope = "member", 
    Target = "Apollo.UI.Explorer.Commands.ShowAboutWindowCommand.#.ctor(Microsoft.Practices.Composite.Events.IEventAggregator)")]
