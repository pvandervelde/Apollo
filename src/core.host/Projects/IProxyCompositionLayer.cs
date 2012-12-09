//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Base.Plugins;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that provide a proxy layer to the <see cref="ICompositionLayer"/>.
    /// </summary>
    internal interface IProxyCompositionLayer : ICompositionLayer, IAmProxyForDataset
    {
    }
}
