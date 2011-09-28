//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
        /// <param name="expression">The expression that is used to call the method for which the name must be determined.</param>
        /// <example>
        /// var result = MethodName(() => x.Bar())
        /// </example>
        /// <returns>
        /// The name of the method in the expression or <see langword="null"/> if no method was called in the expression.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The generics are necessar to get the name of the method via a lambda expression.")]
        public static string MethodName(LambdaExpression expression)
        {
            var method = expression.Body as MemberExpression;
            if (method != null)
            {
                return method.Member.Name;
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the method which is called inside the expression.
        /// </summary>
        /// <example>
        /// var result = MethodName(x => x.Bar())
        /// </example>
        /// <typeparam name="T">The type on which the method is called.</typeparam>
        /// <param name="expression">The expression that is used to call the method for which the name must be determined.</param>
        /// <returns>
        /// The name of the method in the expression or <see langword="null"/> if no method was called in the expression.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "By typing it as Expression<T> it becomes possible to use the lambda syntax at the caller site.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The generics are necessar to get the name of the method via a lambda expression.")]
        public static string MethodName<T>(Expression<Action<T>> expression)
        {
            var method = expression.Body as MemberExpression;
            if (method != null)
            {
                return method.Member.Name;
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the method which is called inside the expression.
        /// </summary>
        /// <example>
        /// var result = MethodName(x => x.Bar())
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

        /// <summary>
        /// Builds a comma separated string containing all the method names and parameters for each of the method information 
        /// objects in the collection.
        /// </summary>
        /// <param name="methods">The collection containing the method information.</param>
        /// <returns>A string containing all the method signatures.</returns>
        public static string MethodInfoToString(IEnumerable<MethodInfo> methods)
        {
            var methodsText = new StringBuilder();
            foreach (var methodInfo in methods)
            {
                if (methodsText.Length > 0)
                {
                    methodsText.Append("; ");
                }

                var parametersText = new StringBuilder();
                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    if (parametersText.Length > 0)
                    {
                        parametersText.Append(", ");
                    }

                    parametersText.Append(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}{1}{2} {3}",
                            parameterInfo.IsOut ? "out " : string.Empty,
                            parameterInfo.IsRetval ? "ref " : string.Empty,
                            parameterInfo.ParameterType.Name,
                            parameterInfo.Name));
                }

                methodsText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}({2})",
                        methodInfo.DeclaringType.Name,
                        methodInfo.Name,
                        parametersText.ToString()));
            }

            return methodsText.ToString();
        }

        /// <summary>
        /// Builds a comma separated string containing all the property signatures of the property information in the collection.
        /// </summary>
        /// <param name="properties">The collection containing the property information.</param>
        /// <returns>A string containing the property information.</returns>
        public static string PropertyInfoToString(IEnumerable<PropertyInfo> properties)
        {
            var propertiesText = new StringBuilder();
            foreach (var propertyInfo in properties)
            {
                if (propertiesText.Length > 0)
                {
                    propertiesText.Append("; ");
                }

                propertiesText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        propertyInfo.DeclaringType.Name,
                        propertyInfo.Name));
            }

            return propertiesText.ToString();
        }

        /// <summary>
        /// Builds a comma separated string containing the event signatures of all the events in the 
        /// collection.
        /// </summary>
        /// <param name="events">The collection containing the events.</param>
        /// <returns>A string containing the event signatures.</returns>
        public static string EventInfoToString(IEnumerable<EventInfo> events)
        {
            var propertiesText = new StringBuilder();
            foreach (var eventInfo in events)
            {
                if (propertiesText.Length > 0)
                {
                    propertiesText.Append("; ");
                }

                propertiesText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        eventInfo.DeclaringType.Name,
                        eventInfo.Name));
            }

            return propertiesText.ToString();
        }
    }
}
