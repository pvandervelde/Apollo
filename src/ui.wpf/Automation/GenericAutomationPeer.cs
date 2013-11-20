//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace Apollo.UI.Wpf.Automation
{
    /// <summary>
    /// Defines a generic automation peer that can be used to find dynamically generated UI elements
    /// for UI automation.
    /// </summary>
    /// <remarks>
    /// Original code from here: http://www.colinsalmcorner.com/2011/11/genericautomationpeer-helping-coded-ui.html.
    /// </remarks>
    public sealed class GenericAutomationPeer : UIElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The UI element that is the top level owner for this peer.</param>
        public GenericAutomationPeer(UIElement owner)
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
            var list = base.GetChildrenCore();
            list.AddRange(GetChildPeers(Owner));
            return list;
        }

        private List<AutomationPeer> GetChildPeers(UIElement element)
        {
            var list = new List<AutomationPeer>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null)
                {
                    var childPeer = UIElementAutomationPeer.CreatePeerForElement(child);
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
