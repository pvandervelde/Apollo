//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Communication;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Defines an <see cref="INotificationSet"/> that is used to notify listeners of changes to the registered
    /// plug-ins in a plug-in repository.
    /// </summary>
    public interface INotifyOfPluginInformationChanges : INotificationSet
    {
    }
}
