//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using Apollo.Core.Base.Properties;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines helper methods for 'serializing' <see cref="ICommandSet"/> layout and invocation signatures.
    /// </summary>
    internal static class CommandSetProxyExtensions
    {
        /// <summary>
        /// Stores type information about a <see cref="ICommandSet"/> in serializable form.
        /// </summary>
        [Serializable]
        private sealed class SerializedType : ISerializedType
        {
            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(SerializedType first, SerializedType second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return true;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(SerializedType first, SerializedType second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return false;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return !nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SerializedType"/> class.
            /// </summary>
            /// <param name="assemblyQualifiedTypeName">The assembly qualified name of the command set type.</param>
            public SerializedType(string assemblyQualifiedTypeName)
            {
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(assemblyQualifiedTypeName), "No assembly full name specified.");
                }

                AssemblyQualifiedTypeName = assemblyQualifiedTypeName;
            }

            /// <summary>
            /// Gets the assembly qualified name of the command set type.
            /// </summary>
            public string AssemblyQualifiedTypeName
            {
                get;
                private set;
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            ///     <see langword="true" /> if the current object is equal to the other parameter; otherwise, <see langword="false" />.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(ISerializedType other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return string.Equals(AssemblyQualifiedTypeName, other.AssemblyQualifiedTypeName, StringComparison.Ordinal);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public sealed override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                var id = obj as ISerializedType;
                return Equals(id);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                // As obtained from the Jon Skeet answer to:  http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
                //
                // Overflow is fine, just wrap
                unchecked
                {
                    // Pick a random prime number
                    int hash = 17;

                    // Mash the hash together with yet another random prime number
                    hash = (hash * 23) ^ AssemblyQualifiedTypeName.GetHashCode();

                    return hash;
                }
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return AssemblyQualifiedTypeName;
            }
        }

        /// <summary>
        /// Stores information about a serialized method invocation.
        /// </summary>
        [Serializable]
        private sealed class SerializedMethodInvocation : ISerializedMethodInvocation
        {
            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(SerializedMethodInvocation first, SerializedMethodInvocation second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return true;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(SerializedMethodInvocation first, SerializedMethodInvocation second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return false;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return !nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// The collection that holds the parameter names and values.
            /// </summary>
            private readonly List<Tuple<ISerializedType, object>> m_Parameters;

            /// <summary>
            /// Initializes a new instance of the <see cref="SerializedMethodInvocation"/> class.
            /// </summary>
            /// <param name="commandSet">The serialized information about the command set.</param>
            /// <param name="methodName">The name of the method that was called.</param>
            /// <param name="namedParameters">The collection of parameter names and values.</param>
            public SerializedMethodInvocation(ISerializedType commandSet, string methodName, List<Tuple<ISerializedType, object>> namedParameters)
            {
                {
                    Debug.Assert(commandSet != null, "No command set information specified.");
                    Debug.Assert(!string.IsNullOrWhiteSpace(methodName), "No method name specified.");
                }

                CommandSet = commandSet;
                MemberName = methodName;
                m_Parameters = namedParameters;
            }

            /// <summary>
            /// Gets the command set on which the method was invoked.
            /// </summary>
            public ISerializedType CommandSet
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the name of the method.
            /// </summary>
            public string MemberName
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a collection which contains the names and values of the parameters.
            /// </summary>
            public List<Tuple<ISerializedType, object>> Parameters
            {
                get
                {
                    return m_Parameters;
                }
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            ///     <see langword="true" /> if the current object is equal to the other parameter; otherwise, <see langword="false" />.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(ISerializedMethodInvocation other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                var sameType = CommandSet.Equals(other.CommandSet)
                    && string.Equals(MemberName, other.MemberName, StringComparison.Ordinal);

                var sameParameters = Parameters.Count == other.Parameters.Count;
                if (sameParameters)
                {
                    for (int i = 0; i < Parameters.Count; i++)
                    {
                        var ours = Parameters[i];
                        var theirs = other.Parameters[i];

                        sameParameters = ours.Item1.Equals(theirs.Item1) && (ours.Item2 == theirs.Item2);
                        if (!sameParameters)
                        {
                            break;
                        }
                    }
                }

                return sameType && sameParameters;
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public sealed override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                var id = obj as ISerializedMethodInvocation;
                return Equals(id);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                // As obtained from the Jon Skeet answer to:  http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
                //
                // Overflow is fine, just wrap
                unchecked
                {
                    // Pick a random prime number
                    int hash = 17;

                    // Mash the hash together with yet another random prime number
                    hash = (hash * 23) ^ CommandSet.GetHashCode();
                    hash = (hash * 23) ^ MemberName.GetHashCode();
                    foreach (var pair in Parameters)
                    {
                        hash = (hash * 23) ^ pair.Item1.GetHashCode();
                        hash = (hash * 23) ^ pair.Item2.GetHashCode();
                    }

                    return hash;
                }
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return CommandSet.ToString() + "." + MemberName;
            }
        }

        /// <summary>
        /// Translates an <see cref="ICommandSet"/> type into a serializable form.
        /// </summary>
        /// <param name="type">A command set type.</param>
        /// <returns>
        ///     An object that stores the <see cref="ICommandSet"/> type information in a serializable format.
        /// </returns>
        public static ISerializedType FromType(Type type)
        {
            {
                Debug.Assert(typeof(ICommandSet).IsAssignableFrom(type), "Cannot store serialized ICommandSet information for this type.");
            }

            return new SerializedType(type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Translates an <see cref="ICommandSet"/> object into a serializable form.
        /// </summary>
        /// <param name="commandSet">A command set object.</param>
        /// <returns>
        ///     An object that stores the <see cref="ICommandSet"/> type information in a serializable format.
        /// </returns>
        public static ISerializedType FromObject(ICommandSet commandSet)
        {
            return FromType(commandSet.GetType());
        }

        /// <summary>
        /// Returns the type of the <see cref="ICommandSet"/> for which information is stored
        /// in the <paramref name="commandSet"/> parameter.
        /// </summary>
        /// <param name="commandSet">The serialization information for a <see cref="ICommandSet"/>.</param>
        /// <returns>
        /// The <see cref="ICommandSet"/> type.
        /// </returns>
        public static Type ToType(ISerializedType commandSet)
        {
            try
            {
                return Type.GetType(commandSet.AssemblyQualifiedTypeName);
            }
            catch (TargetInvocationException e)
            {
                throw new UnableToLoadCommandSetTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        Resources.Exceptions_Messages_UnableToLoadCommandSetType_WithTypeName, 
                        commandSet.AssemblyQualifiedTypeName),
                    e);
            }
            catch (TypeLoadException e)
            {
                throw new UnableToLoadCommandSetTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadCommandSetType_WithTypeName,
                        commandSet.AssemblyQualifiedTypeName),
                    e);
            }
            catch (FileNotFoundException e)
            {
                throw new UnableToLoadCommandSetTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadCommandSetType_WithTypeName,
                        commandSet.AssemblyQualifiedTypeName),
                    e);
            }
            catch (FileLoadException e)
            {
                throw new UnableToLoadCommandSetTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadCommandSetType_WithTypeName,
                        commandSet.AssemblyQualifiedTypeName),
                    e);
            }
            catch (BadImageFormatException e)
            {
                throw new UnableToLoadCommandSetTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadCommandSetType_WithTypeName,
                        commandSet.AssemblyQualifiedTypeName),
                    e);
            }
        }

        /// <summary>
        /// Translates a <see cref="MethodInfo"/> and the related parameter values into a serializable form.
        /// </summary>
        /// <param name="method">The method information that needs to be serialized.</param>
        /// <param name="parameters">
        ///     The collection of parameter values with which the method should be called. Note that the parameter
        ///     values should be in the same order as they are given by the <c>MethodInfo.GetParameters()</c> method.
        /// </param>
        /// <returns>
        ///     An object that stores the method invocation information in a serializable format.
        /// </returns>
        public static ISerializedMethodInvocation FromMethodInfo(MethodBase method, object[] parameters)
        {
            var methodParameters = method.GetParameters();
            Debug.Assert(methodParameters.Length == parameters.Length, "There are a different number of parameters than there are parameter values.");

            var namedParameters = new List<Tuple<ISerializedType, object>>();
            for (int i = 0; i < parameters.Length; i++)
            {
                namedParameters.Add(Tuple.Create(FromType(methodParameters[i].ParameterType), parameters[i]));
            }

            return new SerializedMethodInvocation(
                FromType(method.DeclaringType),
                method.Name,
                namedParameters);
        }
    }
}
