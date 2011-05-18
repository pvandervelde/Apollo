//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the type of a processor.
    /// </summary>
    public enum ProcessorArchitecture
    {
        /// <summary>
        /// There is no processor architecture. Usually not a valid value.
        /// </summary>
        None,

        /// <summary>
        /// The processor is of the Intel x86 architecture.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "x",
            Justification = "The correct term for this architecture is x86 in small-caps")]
        x86,

        /// <summary>
        /// The processor is of the MIPS (Microprocessor without Interlocked Pipeline Stages) architecture.
        /// </summary>
        Mips,
        
        /// <summary>
        /// The processor is a DEC Alpha architecture.
        /// </summary>
        Alpha,

        /// <summary>
        /// The processor is a PowerPC processor.
        /// </summary>
        PowerPC,

        /// <summary>
        /// The processor is an Intel Itanium processor.
        /// </summary>
        Itanium,

        /// <summary>
        /// The processor is of the AMD x64 architecture.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "x",
            Justification = "The correct term for this architecture is x64 in small-caps")]
        x64,

        /// <summary>
        /// The processor is of an unknown architecture.
        /// </summary>
        Unknown
    }
}
