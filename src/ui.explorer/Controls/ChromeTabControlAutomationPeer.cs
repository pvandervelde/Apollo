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
using Apollo.UI.Wpf.Utilities;
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
    public sealed class ChromeTabControlAutomationPeer : TabControlAutomationPeer
    {
        /// <summary>
        /// Defines the name or tag for the panel that contains the scroll controls in the <see cref="ChromeAutomationTabControl"/>.
        /// </summary>
        public const string TabScrollControllerPanel = "Explorer.ChromeTabControl.TabScrollControllerPanel";

        /// <summary>
        /// Defines the name or tag for the panel that contains the tab items in the <see cref="ChromeAutomationTabControl"/>.
        /// </summary>
        public const string TabScrollPanel = "Explorer.ChromeTabControl.TabScrollPanel";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeTabControlAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="T:System.Windows.Controls.TabControl"/> that is associated with this
        ///  <see cref="ChromeTabControlAutomationPeer"/>.
        /// </param>
        internal ChromeTabControlAutomationPeer([NotNull] ChromeAutomationTabControl owner) 
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

            var tabControl = Owner as ChromeAutomationTabControl;
            foreach (UIElement item in tabControl.Items)
            {
                if ((item != null) && item.IsVisible)
                {
                    list.Add(CreatePeerForElement(item));
                }
            }

            // Find the menu + the buttons
            var control = FindScrollControllerPanel(tabControl);
            foreach (UIElement child in control.Children)
            {
                if (child != null)
                {
                    var childPeer = CreatePeerForElement(child);
                    if (childPeer != null)
                    {
                        list.Add(childPeer);
                    }
                }
            }

            // Find the tab items
            foreach (var tabItem in FindTabItems(tabControl))
            {
                var childPeer = CreatePeerForElement(tabItem);
                if (childPeer != null)
                {
                    list.Add(childPeer);
                }
            }

            return list;
        }

        private Panel FindScrollControllerPanel(UIElement element)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null)
                {
                    var panel = child as Panel;
                    if (panel != null)
                    {
                        if (panel.Tag.Equals(TabScrollControllerPanel))
                        {
                            return panel;
                        }
                    }

                    var control = FindScrollControllerPanel(child);
                    if (control != null)
                    {
                        return control;
                    }
                }
            }

            return null;
        }

        private IEnumerable<TabItem> FindTabItems(UIElement element)
        {
            var result = new List<TabItem>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null)
                {
                    var panel = child as ScrollableTabPanel;
                    if (panel != null)
                    {
                        if (panel.Tag.Equals(TabScrollPanel))
                        {
                            var tabItems = GetTabItemsFromPanel(panel);
                            result.AddRange(tabItems);
                            return result;
                        }
                    }

                    var list = FindTabItems(child);
                    if (list != null)
                    {
                        result.AddRange(list);
                    }
                }
            }

            return result;
        }

        private IEnumerable<TabItem> GetTabItemsFromPanel(ScrollableTabPanel panel)
        {
            var result = new List<TabItem>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(panel); i++)
            {
                var child = VisualTreeHelper.GetChild(panel, i) as UIElement;
                if (child != null)
                {
                    var tabItem = child as TabItem;
                    if (tabItem != null)
                    {
                        result.Add(tabItem);
                    }

                    var list = FindTabItems(child);
                    if (list != null)
                    {
                        result.AddRange(list);
                    }
                }
            }

            return result;
        }
    }
}
