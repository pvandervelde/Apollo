//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Lokad;

namespace Apollo.UI.Wpf
{
    /// <summary>
    /// A base class for objects that implement the INotifyPropertyChanged interface.
    /// </summary>
    public abstract class Observable : INotifyPropertyChanged
    {
        /// <summary>
        /// The context that is used to perform actions on the UI thread.
        /// </summary>
        private readonly IContextAware m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Observable"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        protected Observable(IContextAware context)
        {
            {
                Enforce.Argument(() => context);
            }

            m_Context = context;
        }

        /// <summary>
        /// Gets the context that is used to perform actions on the UI thread.
        /// </summary>
        protected IContextAware InternalContext
        {
            get
            {
                return m_Context;
            }
        }

        /// <summary>
        /// The event that is fired when a property changes.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the INotifyProperyChanged interface.")]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners about a change.
        /// </summary>
        /// <param name="property">The property that changed.</param>
        /// <exception cref="InvalidExpressionException">
        ///     Thrown when <paramref name="property"/> is not a valid property get lambda expression.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The generic expression makes it possible to get the property name which we need to raise the PropertyChanged event.")]
        protected void Notify(Expression<Func<object>> property)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var lambda = property as LambdaExpression;
            if (lambda == null)
            {
                throw new InvalidExpressionException();
            }

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                if (unaryExpression == null)
                {
                    throw new InvalidExpressionException();
                }

                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new InvalidExpressionException();
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new InvalidExpressionException();
            }

            RaisePropertyChanged(propertyInfo.Name);
        }

        private void RaisePropertyChanged(string name)
        {
            var local = PropertyChanged;
            if (local != null)
            {
                Action action = () => local(this, new PropertyChangedEventArgs(name));
                if (m_Context.IsSynchronized)
                {
                    action();
                }
                else
                {
                    m_Context.Invoke(action);
                }
            }
        }
    }
}
