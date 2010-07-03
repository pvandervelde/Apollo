//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Apollo.Core")]
[assembly: AssemblyDescription("Holds the components for the core part of Apollo")]
[assembly: AssemblyProduct("Apollo.Core")]
[assembly: AssemblyCulture("")]

// Indicate that the assembly is CLS compliant.
[assembly: CLSCompliant(true)]

// Indicate that the neutral language resources are in the assembly.
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

// Security permissions
// Indicate that partially trusted assemblies can call into this assembly. Note that we should
// never give these partially trusted callers reflection permissions!
[assembly: AllowPartiallyTrustedCallers]

// Indicate that this assembly has security critical elements, but most of it
// should be transparent to security demands (i.e. security demands pass through).
[assembly: SecurityCritical]