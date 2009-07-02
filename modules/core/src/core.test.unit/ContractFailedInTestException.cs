//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Test.Unit
{
    [Serializable]
    public sealed class ContractFailedInTestException : Exception
    {
        public ContractFailedInTestException()
            : base("A contract failed.")
        { 
        }

        public ContractFailedInTestException(string message)
            : base(message)
        { 
        }
    }
}
