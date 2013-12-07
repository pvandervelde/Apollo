//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Apollo.UI.Explorer.Controls
{
    /// <summary>
    /// Defines a tab control which has special automation skills.
    /// </summary>
    internal sealed class ChromeAutomationTabControl : TabControl
    {
        /// <summary>
        /// Provides an appropriate <see cref="T:System.Windows.Automation.Peers.TabControlAutomationPeer"/> implementation for this 
        /// control, as part of the WPF automation infrastructure.
        /// </summary>
        /// <returns>
        /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            // return new ChromeTabControlAutomationPeer(this);
            return base.OnCreateAutomationPeer();
        }
    }
}
