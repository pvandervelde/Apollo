//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
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
        /// <param name="Property">The property that changed.</param>
        protected void Notify(Expression<Func<object>> Property)
        {
            // Check for null
            if (PropertyChanged == null)
            {
                return;
            }

            // Get property name
            var lambda = Property as LambdaExpression;
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

            var constantExpression = memberExpression.Expression as ConstantExpression;
            var propertyInfo = memberExpression.Member as PropertyInfo;

            // Invoke event
            RaisePropertyChanged(propertyInfo.Name);
        }

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