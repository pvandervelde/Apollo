//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

// Design supress messages
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Apollo.Utils", MessageId = "Utils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Apollo.Utils.Fusion", MessageId = "Utils", Justification = "Utils is the known short name for utilities.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Apollo.Utils.Licensing", MessageId = "Utils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "type", Target = "Apollo.Core.Utils.UtilsModule", MessageId = "Utils")]

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Apollo.Core.Utils", MessageId = "Utils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Apollo.Core.Utils.Licensing", MessageId = "Utils")]

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Apollo.Core.Logging.Logger.#TranslateToNlogLevel(Apollo.Core.Logging.LevelToLog)", MessageId = "Nlog")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Apollo.Core.Logging.Logger.#TranslateFromNlogLevel(NLog.Logger)", MessageId = "Nlog")]