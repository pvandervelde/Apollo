// Copyright (c) P. van der Velde. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the base class for message objects.
    /// </summary>
    internal abstract class KernelMessage
    {
        // do messages throw if they can't be delivered?  --> Yes but how do we throw into the sending AppDomain?
    }
}
