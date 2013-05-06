//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.UI.Explorer.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines methods that map an <see cref="EventType"/> to an ID number for error reporting
    /// purposes.
    /// </summary>
    internal static class EventTypeToEventCategoryMap
    {
        /// <summary>
        /// The table that maps an event type to an event category.
        /// </summary>
        private static readonly Dictionary<EventType, short> s_EventTypeToEventCategoryMap =
            new Dictionary<EventType, short> 
                { 
                    { EventType.Exception, 0 }
                };

        /// <summary>
        /// Returns the event category for the given <see cref="EventType"/> value.
        /// </summary>
        /// <param name="type">The type of the event.</param>
        /// <returns>The requested category ID.</returns>
        public static short EventCategory(EventType type)
        {
            return s_EventTypeToEventCategoryMap[type];
        }
    }
}
