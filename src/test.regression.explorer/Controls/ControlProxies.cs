//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using TestStack.White.UIItems;

namespace Test.Regression.Explorer.Controls
{
    /// <summary>
    /// Defines helper methods for dealing with controls.
    /// </summary>
    internal static class ControlProxies
    {
        /// <summary>
        /// Manually searches for a <see cref="UIItem"/> in a container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="automationId">The automation ID for the control.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The control if it can be found; otherwise, <see langword="null" />.</returns>
        public static IUIItem FindItemManuallyInUIContainer(UIItemContainer container, string automationId, Log log)
        {
            const string logPrefix = "Controls - Find element manually";
            if ((container == null) || string.IsNullOrEmpty(automationId))
            {
                return null;
            }

            var stack = new Stack<UIItemContainer>();
            stack.Push(container);
            while (stack.Count > 0)
            {
                var localContainer = stack.Pop();
                foreach (var element in localContainer.Items)
                {
                    log.Debug(
                        logPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Found UI item of type [{0}] with ID: [{1}]. Name: [{2}]",
                            element.GetType().Name,
                            element.Id,
                            element.Name));
                    if (string.Equals(element.Id, automationId, StringComparison.Ordinal))
                    {
                        log.Info(
                            logPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Found desired element of type [{0}] with ID: [{1}]",
                                element.GetType().Name,
                                element.Id));
                        return element;
                    }

                    var subContainer = element as UIItemContainer;
                    if (subContainer != null)
                    {
                        stack.Push(subContainer);
                    }
                }
            }

            return null;
        }
    }
}
