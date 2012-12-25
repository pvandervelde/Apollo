﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Security;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Apollo.UI.Common")]
[assembly: AssemblyDescription("Holds the common user interface components for Apollo")]
[assembly: AssemblyProduct("Apollo.UI.Common")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]

// In order to begin building localizable applications, set 
// <UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
// inside a <PropertyGroup>.  For example, if you are using US english
// in your source files, set the <UICulture> to en-US.  Then uncomment
// the NeutralResourceLanguage attribute below.  Update the "en-US" in
// the line below to match the UICulture setting in the project file.
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

[assembly: ThemeInfo(
    // where theme specific resource dictionaries are located
    // (used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.None,

    // where the generic resource dictionary is located
    // (used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly)]

// Suppress messages
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1704:IdentifiersShouldBeSpelledCorrectly", 
    Scope = "namespace", 
    Target = "Apollo.UI.Common.Eventing", 
    MessageId = "Eventing")]
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1702:CompoundWordsShouldBeCasedCorrectly", 
    Scope = "type", 
    Target = "Apollo.UI.Common.Views.Datasets.DatasetViewGraph", 
    MessageId = "ViewGraph")]
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1702:CompoundWordsShouldBeCasedCorrectly", 
    Scope = "member", 
    Target = "Apollo.UI.Common.Bootstrapper.CompositeBootstrapper.#m_UseDefaultConfiguration", 
    MessageId = "mUse")]
[module: SuppressMessage(
    "Microsoft.Performance", 
    "CA1800:DoNotCastUnnecessarily", 
    Scope = "member", 
    Target = "Apollo.UI.Common.Views.Datasets.DatasetVertexView.#System.Windows.Markup.IComponentConnector.Connect(System.Int32,System.Object)", 
    Justification = "Generated code")]
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1707:IdentifiersShouldNotContainUnderscores", 
    Scope = "member", 
    Target = "Apollo.UI.Common.Views.Datasets.DatasetGraphView.#_CreateDelegate(System.Type,System.String)",
    Justification = "Generated code")]
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1703:ResourceStringsShouldBeSpelledCorrectly", 
    Scope = "resource", 
    Target = "Apollo.UI.Common.Properties.Resources.resources", 
    MessageId = "rb")]
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1703:ResourceStringsShouldBeSpelledCorrectly", 
    Scope = "resource", 
    Target = "Apollo.UI.Common.Properties.Resources.resources", 
    MessageId = "py")]