//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using Lokad.Quality;

namespace Apollo.UI.Explorer.Controls
{
    /// <summary>
    /// Defines a generic automation peer for tab controls that can be used to find dynamically generated UI elements
    /// for UI automation.
    /// </summary>
    /// <remarks>
    /// Original code from here: http://www.colinsalmcorner.com/2011/11/genericautomationpeer-helping-coded-ui.html.
    /// </remarks>
    public sealed class GenericTabControlAutomationPeer : TabControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTabControlAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="T:System.Windows.Controls.TabControl"/> that is associated with this
        ///  <see cref="GenericTabControlAutomationPeer"/>.
        /// </param>
        public GenericTabControlAutomationPeer([NotNull] TabControl owner) 
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the collection of child elements of the <see cref="T:System.Windows.UIElement"/> that is associated with 
        /// this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>. This method is called by 
        /// <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetChildren"/>.
        /// </summary>
        /// <returns>
        /// A list of child <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> elements.
        /// </returns>
        protected override List<AutomationPeer> GetChildrenCore()
        {
            // It is possible for the return value to be null if there are no child controls
            var list = base.GetChildrenCore() ?? new List<AutomationPeer>();
            list.AddRange(GetChildPeers(Owner));
            return list;
        }

        private IEnumerable<AutomationPeer> GetChildPeers(UIElement element)
        {
            var list = new List<AutomationPeer>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null)
                {
                    var childPeer = CreatePeerForElement(child);
                    if (childPeer != null)
                    {
                        list.Add(childPeer);
                    }
                    else
                    {
                        list.AddRange(GetChildPeers(child));
                    }
                }
            }

            return list;
        }
    }
}
