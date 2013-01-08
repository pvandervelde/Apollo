//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Data;

namespace Apollo.UI.Explorer.Converters
{
    /// <summary>
    /// Gets the TabItems that are related to a collection of Model objects.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class ModelCollectionToTabItemsConverter : IValueConverter
    {
        /// <summary>
        /// A class that stores an <see cref="ItemCollection"/> and converts items in that collection to 
        /// TabItem objects. Also notifies the outside world if the original ItemCollection changes.
        /// </summary>
        private sealed class ConvertingCollection : IEnumerable<TabItem>, INotifyCollectionChanged
        {
            /// <summary>
            /// The control that originally owns the ItemCollection.
            /// </summary>
            private readonly ItemsControl m_Control;

            /// <summary>
            /// The collection of items.
            /// </summary>
            private readonly ItemCollection m_Items;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConvertingCollection"/> class.
            /// </summary>
            /// <param name="control">The control that owns the original ItemCollection.</param>
            /// <param name="items">The collection of items that should be converted.</param>
            public ConvertingCollection(ItemsControl control, ItemCollection items)
            {
                m_Control = control;
                m_Items = items;

                var notify = m_Items as INotifyCollectionChanged;
                {
                    notify.CollectionChanged += HandleNotifyCollectionChanged;
                }
            }

            private void HandleNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                RaiseCollectionChanged();
            }

            /// <summary>
            /// Occurs when the collection changes.
            /// </summary>
            [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
                Justification = "Event is inherited from the INotifyCollectionChanged interface.")]
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            private void RaiseCollectionChanged()
            {
                var local = CollectionChanged;
                if (local != null)
                {
                    local(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
            
            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>A System.Collections.Generic.IEnumerator{T} that can be used to iterate through the collection.</returns>
            public IEnumerator<TabItem> GetEnumerator()
            {
                foreach (var item in m_Items)
                {
                    yield return m_Control.ItemContainerGenerator.ContainerFromItem(item) as TabItem;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var tabControl = value as TabControl;
            var models = tabControl.Items;

            return new ConvertingCollection(tabControl, models);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
