//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Projects;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the FusionHelper class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectServiceTest
    {
        #region internal class - MockDnsNameConstants

        private sealed class MockDnsNameConstants : IDnsNameConstants
        {
            public DnsName AddressOfLogger
            {
                get
                {
                    return new DnsName("logger");
                }
            }

            public DnsName AddressOfKernel
            {
                get
                {
                    return new DnsName("kernel");
                }
            }

            public DnsName AddressOfMessagePipeline
            {
                get
                {
                    return new DnsName("pipeline");
                }
            }

            public DnsName AddressOfProjects
            {
                get
                {
                    return new DnsName("projects");
                }
            }

            public DnsName AddressOfUserInterface
            {
                get
                {
                    return new DnsName("ui");
                }
            }
        }

        #endregion

        [Test]
        [Description("Checks that an object cannot be created without an IDnsNameConstants object.")]
        public void CreateWithNullDnsNames()
        {
            Assert.Throws<ArgumentNullException>(() => new ProjectService(
                null,
                new Mock<IHelpMessageProcessing>().Object,
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IHelpMessageProcessing object.")]
        public void CreateWithNullMessageProcessor()
        {
            Assert.Throws<ArgumentNullException>(() => new ProjectService(
                new MockDnsNameConstants(),
                null,
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IValidationResultStorage object.")]
        public void CreateWithNullValidationResultStorage()
        {
            Assert.Throws<ArgumentNullException>(() => new ProjectService(
                new MockDnsNameConstants(),
                new Mock<IHelpMessageProcessing>().Object,
                null,
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IHelpDistributionDatasets object.")]
        public void CreateWithNullDatasetDistributor()
        {
            Assert.Throws<ArgumentNullException>(() => new ProjectService(
                new MockDnsNameConstants(),
                new Mock<IHelpMessageProcessing>().Object,
                new LicenseValidationResultStorage(),
                null,
                new Mock<IBuildProjects>().Object));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IBuildProjects object.")]
        public void CreateWithNullProjectBuilder()
        {
            Assert.Throws<ArgumentNullException>(() => new ProjectService(
                new MockDnsNameConstants(),
                new Mock<IHelpMessageProcessing>().Object,
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                null));
        }

        [Test]
        [Description("Checks that the service returns the correct service types that should be available.")]
        public void ServicesToBeAvailable()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            Assert.AreElementsEqualIgnoringOrder(new Type[] { typeof(LogSink) }, service.ServicesToBeAvailable());
        }

        [Test]
        [Description("Checks that the service returns the correct service types to connect to.")]
        public void ServicesToConnectTo()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            Assert.AreElementsEqualIgnoringOrder(new Type[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new Mock<KernelService>().Object);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline(new DnsNameConstants()));
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);

            service.DisconnectFrom(pipeline);
            Assert.IsFalse(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the service can stop successfully.")]
        public void Stop()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);

            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.StartupState);
        }

        [Test]
        [Description("Checks that ServiceShutdownCapabilityRequestMessage is handled correctly.")]
        public void HandleServiceShutdownCapabilityRequestMessage()
        {
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            DnsName storedSender = null;
            MessageBody storedBody = null;
            MessageId storedInReplyTo = null;
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
                processor.Setup(p => p.SendMessage(It.IsAny<DnsName>(), It.IsAny<MessageBody>(), It.IsAny<MessageId>()))
                    .Callback<DnsName, MessageBody, MessageId>(
                        (d, b, m) =>
                        {
                            storedSender = d;
                            storedBody = b;
                            storedInReplyTo = m;
                        });
            }

            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();

            Assert.IsTrue(actions.ContainsKey(typeof(ServiceShutdownCapabilityRequestMessage)));
            {
                var body = new ServiceShutdownCapabilityRequestMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfUserInterface);
                actions[typeof(ServiceShutdownCapabilityRequestMessage)](new KernelMessage(header, body));

                Assert.AreEqual(header.Sender, storedSender);
                Assert.AreEqual(header.Id, storedInReplyTo);
                Assert.IsInstanceOfType<ServiceShutdownCapabilityResponseMessage>(storedBody);
            }
        }

        [Test]
        [Description("Checks that CreateNewProjectMessage is handled correctly.")]
        public void HandleCreateNewProjectMessage()
        {
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            DnsName storedSender = null;
            MessageBody storedBody = null;
            MessageId storedInReplyTo = null;
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
                processor.Setup(p => p.SendMessage(It.IsAny<DnsName>(), It.IsAny<MessageBody>(), It.IsAny<MessageId>()))
                    .Callback<DnsName, MessageBody, MessageId>(
                        (d, b, m) =>
                        {
                            storedSender = d;
                            storedBody = b;
                            storedInReplyTo = m;
                        });
            }

            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();

            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();

            Assert.IsTrue(actions.ContainsKey(typeof(CreateNewProjectMessage)));
            {
                var body = new CreateNewProjectMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfUserInterface);
                actions[typeof(CreateNewProjectMessage)](new KernelMessage(header, body));

                Assert.AreEqual(header.Sender, storedSender);
                Assert.AreEqual(header.Id, storedInReplyTo);
                Assert.IsInstanceOfType<ProjectRequestResponseMessage>(storedBody);

                var message = storedBody as ProjectRequestResponseMessage;
                Assert.IsNotNull(message.ProjectReference);

                // Get rid of the project registration in the 
                // Remoting services.
                service.UnloadProject();
            }
        }

        [Test]
        [Description("Checks that LoadProjectMessage is handled correctly.")]
        [Ignore("The project persistence is not finished yet. Can't test until that is there.")]
        public void HandleLoadProjectMessage()
        {
        }

        [Test]
        [Description("Checks that UnloadProjectMessage is handled correctly.")]
        public void HandleUnloadProjectMessage()
        {
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            DnsName storedSender = null;
            MessageBody storedBody = null;
            MessageId storedInReplyTo = null;
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
                processor.Setup(p => p.SendMessage(It.IsAny<DnsName>(), It.IsAny<MessageBody>(), It.IsAny<MessageId>()))
                    .Callback<DnsName, MessageBody, MessageId>(
                        (d, b, m) =>
                        {
                            storedSender = d;
                            storedBody = b;
                            storedInReplyTo = m;
                        });
            }

            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            {
                closable.Setup(p => p.Close())
                    .Verifiable();
            }

            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            service.CreateNewProject();

            Assert.IsTrue(actions.ContainsKey(typeof(UnloadProjectMessage)));
            {
                var body = new UnloadProjectMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfUserInterface);
                actions[typeof(UnloadProjectMessage)](new KernelMessage(header, body));

                closable.Verify(c => c.Close(), Times.Exactly(1));
            }
        }

        [Test]
        [Description("Checks that ProjectRequestMessage is handled correctly.")]
        public void HandleProjectRequestMessage()
        {
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            DnsName storedSender = null;
            MessageBody storedBody = null;
            MessageId storedInReplyTo = null;
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
                processor.Setup(p => p.SendMessage(It.IsAny<DnsName>(), It.IsAny<MessageBody>(), It.IsAny<MessageId>()))
                    .Callback<DnsName, MessageBody, MessageId>(
                        (d, b, m) =>
                        {
                            storedSender = d;
                            storedBody = b;
                            storedInReplyTo = m;
                        });
            }

            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();

            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);
            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            service.CreateNewProject();

            Assert.IsTrue(actions.ContainsKey(typeof(ProjectRequestMessage)));
            {
                var body = new ProjectRequestMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfUserInterface);
                actions[typeof(ProjectRequestMessage)](new KernelMessage(header, body));

                Assert.AreEqual(header.Sender, storedSender);
                Assert.AreEqual(header.Id, storedInReplyTo);
                Assert.IsInstanceOfType<ProjectRequestResponseMessage>(storedBody);

                var message = storedBody as ProjectRequestResponseMessage;
                Assert.IsNotNull(message.ProjectReference);

                // Get rid of the project registration in the 
                // Remoting services.
                service.UnloadProject();
            }
        }

        [Test]
        [Description("Checks that a new project can be created.")]
        public void CreateNewProject()
        {
            var dnsNames = new MockDnsNameConstants();

            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            service.CreateNewProject();
            
            builder.Verify(b => b.Define(), Times.Exactly(1));
            builder.Verify(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()), Times.Exactly(1));
            builder.Verify(b => b.Build(), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that a new project cannot be loaded without a null persistence object.")]
        public void LoadProjectWithNullPersistenceInformation()
        {
            var dnsNames = new MockDnsNameConstants();

            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            Assert.Throws<ArgumentNullException>(() => service.LoadProject(null));
        }

        [Test]
        [Description("Checks that a new project can be loaded.")]
        public void LoadProject()
        {
            var dnsNames = new MockDnsNameConstants();

            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.FromStorage(It.IsAny<IPersistenceInformation>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            service.LoadProject(new Mock<IPersistenceInformation>().Object);

            builder.Verify(b => b.Define(), Times.Exactly(1));
            builder.Verify(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()), Times.Exactly(1));
            builder.Verify(b => b.FromStorage(It.IsAny<IPersistenceInformation>()), Times.Exactly(1));
            builder.Verify(b => b.Build(), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that a project cannot be saved with a null persistence object.")]
        public void SaveProjectWithNullPersistenceInformation()
        {
            var dnsNames = new MockDnsNameConstants();

            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            Assert.Throws<ArgumentNullException>(() => service.SaveProject(null));
        }

        [Test]
        [Description("Checks that a project can be saved.")]
        public void SaveProject()
        {
            var dnsNames = new MockDnsNameConstants();

            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            IPersistenceInformation persistence = null;
            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            {
                project.Setup(p => p.Save(It.IsAny<IPersistenceInformation>()))
                    .Callback<IPersistenceInformation>(p => persistence = p)
                    .Verifiable();
            }

            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object);

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()))
                    .Returns(builder.Object);

                builder.Setup(b => b.Build())
                    .Returns(project.Object);
            }

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            service.CreateNewProject();
            service.SaveProject(new Mock<IPersistenceInformation>().Object);

            Assert.IsNotNull(persistence);
            project.Verify(p => p.Save(It.IsAny<IPersistenceInformation>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that a project can be unloaded.")]
        public void UnloadProject()
        {
            var dnsNames = new MockDnsNameConstants();

            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            {
                closable.Setup(p => p.Close())
                    .Verifiable();
            }

            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object);

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, DistributionPlan>>()))
                    .Returns(builder.Object);

                builder.Setup(b => b.Build())
                    .Returns(project.Object);
            }

            var service = new ProjectService(
                dnsNames,
                processor.Object,
                storage,
                distributor.Object,
                builder.Object);

            service.CreateNewProject();
            service.UnloadProject();

            closable.Verify(p => p.Close(), Times.Exactly(1));
        }
    }
}
