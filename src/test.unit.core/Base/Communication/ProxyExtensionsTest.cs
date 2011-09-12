//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ProxyExtensionsTest
    {
        // A fake command set interface to invoke methods on
        public interface IMockCommandSet : ICommandSet
        {
            Task MethodWithoutReturnValue(int someNumber);

            Task OtherMethodWithoutReturnValue(int otherNumber);

            Task<int> MethodWithReturnValue(int someNumber);

            Task<int> OtherMethodWithReturnValue(int otherNumber);
        }

        [VerifyContract]
        public readonly IContract SerializedTypeHashCodeVerification = new HashCodeAcceptanceContract<ISerializedType>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ISerializedType> 
                        {
                            ProxyExtensions.FromType(typeof(object)),
                            ProxyExtensions.FromType(typeof(string)),
                            ProxyExtensions.FromType(typeof(ICommandSet)),
                            ProxyExtensions.FromType(typeof(EndpointId)),
                            ProxyExtensions.FromType(typeof(IDatasetApplicationCommands)),
                        },
        };

        [VerifyContract]
        public readonly IContract SerializedTypeEqualityVerification = new EqualityContract<ISerializedType>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        ProxyExtensions.FromType(typeof(object)),
                        ProxyExtensions.FromType(typeof(string)),
                        ProxyExtensions.FromType(typeof(ICommandSet)),
                        ProxyExtensions.FromType(typeof(EndpointId)),
                        ProxyExtensions.FromType(typeof(IDatasetApplicationCommands)),
                    },
        };

        [VerifyContract]
        public readonly IContract SerializedMethodHashCodeVerification = new HashCodeAcceptanceContract<ISerializedMethodInvocation>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ISerializedMethodInvocation> 
                        {
                            ProxyExtensions.FromMethodInfo(
                                typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue"), new object[] { 2 }),
                            ProxyExtensions.FromMethodInfo(
                                typeof(IMockCommandSet).GetMethod("OtherMethodWithoutReturnValue"), new object[] { 2 }),
                            ProxyExtensions.FromMethodInfo(
                                typeof(IMockCommandSet).GetMethod("MethodWithReturnValue"), new object[] { 2 }),
                            ProxyExtensions.FromMethodInfo(
                                typeof(IMockCommandSet).GetMethod("OtherMethodWithReturnValue"), new object[] { 2 }),
                        },
        };

        [VerifyContract]
        public readonly IContract SerializedMethodEqualityVerification = new EqualityContract<ISerializedMethodInvocation>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue"), new object[] { 2 }),
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("OtherMethodWithoutReturnValue"), new object[] { 2 }),
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("MethodWithReturnValue"), new object[] { 2 }),
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("OtherMethodWithReturnValue"), new object[] { 2 }),
                    },
        };
    }
}
