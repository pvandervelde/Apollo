//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Apollo.UI.Common
{
    /// <summary>
    /// A base class for objects that implement the INotifyPropertyChanged interface.
    /// </summary>
    public abstract class Observable : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that is fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners about a change.
        /// </summary>
        /// <param name="property">The property that changed.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "We need the expresion bit to determine the property name programatically.")]
        protected void Notify(Expression<Func<object>> property)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var lambda = property as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            RaisePropertyChanged(propertyInfo.Name);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to call said event.")]
        private void RaisePropertyChanged(string name)
        {
            var local = PropertyChanged;
            if (local != null)
            {
                local(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}