//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage(
    "Microsoft.Design", 
    "CA1020:AvoidNamespacesWithFewTypes", 
    Scope = "namespace", 
    Target = "Apollo.Utilities.Licensing")]
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1704:IdentifiersShouldBeSpelledCorrectly", 
    Scope = "member", 
    Target = "Apollo.Core.Logging.Logger.#TranslateToNlogLevel(Apollo.Core.Logging.LevelToLog)", 
    MessageId = "Nlog")]
[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1704:IdentifiersShouldBeSpelledCorrectly", 
    Scope = "member", 
    Target = "Apollo.Core.Logging.Logger.#TranslateFromNlogLevel(NLog.Logger)", 
    MessageId = "Nlog")]
