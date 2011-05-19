//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines helper and extension methods for reflection of types and methods.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Returns the name of the method which is called inside the expression.
        /// </summary>
        /// <example>
        /// var result = MethodName(x => x.Bar)
        /// </example>
        /// <typeparam name="T">The type on which the method is called.</typeparam>
        /// <typeparam name="TResult">The result of the method call.</typeparam>
        /// <param name="expression">The expression that is used to call the method for which the name must be determined.</param>
        /// <returns>
        /// The name of the method in the expression or <see langword="null"/> if no method was called in the expression.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "By typing it as Expression<T> it becomes possible to use the lambda syntax at the caller site.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The generics are necessar to get the name of the method via a lambda expression.")]
        public static string MethodName<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            var method = expression.Body as MemberExpression;
            if (method != null)
            {
                return method.Member.Name;
            }

            return null;
        }
    }
}
