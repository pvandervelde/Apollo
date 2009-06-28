using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Test.Unit
{
    [Serializable]
    public sealed class ContractFailedInTestException : Exception
    {
        public ContractFailedInTestException()
            : base("A contract failed.")
        { }

        public ContractFailedInTestException(string message)
            : base(message)
        { }
    }
}
