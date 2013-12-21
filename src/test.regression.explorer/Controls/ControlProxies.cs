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
            log.Debug(
                logPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Searching for UI element with ID: [{0}].",
                    automationId));

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

        /// <summary>
        /// Manually searches for a <see cref="UIItem"/> in a container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="automationId">The partial automation ID for the control.</param>
        /// <param name="log">The log object.</param>
        /// <returns>A collection containing all the controls that have an automation ID that matches the partial ID.</returns>
        public static IEnumerable<IUIItem> FindItemsManuallyInUIContainerWithPartialId(UIItemContainer container, string automationId, Log log)
        {
            const string logPrefix = "Controls - Find element manually with partial ID";
            log.Debug(
                logPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Searching for UI element with partial ID: [{0}].",
                    automationId));

            var result = new List<IUIItem>();
            if ((container == null) || string.IsNullOrEmpty(automationId))
            {
                return result;
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
                    if ((!string.IsNullOrEmpty(element.Id)) && element.Id.Contains(automationId))
                    {
                        log.Info(
                            logPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Found matching element of type [{0}] with ID: [{1}]",
                                element.GetType().Name,
                                element.Id));
                        result.Add(element);
                    }

                    var subContainer = element as UIItemContainer;
                    if (subContainer != null)
                    {
                        stack.Push(subContainer);
                    }
                }
            }

            return result;
        }
    }
}
