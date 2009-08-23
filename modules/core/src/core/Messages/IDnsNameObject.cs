using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Messages;

namespace Apollo.Core
{
    public interface IDnsNameObject
    {
        DnsName Name { get; }
    }
}
