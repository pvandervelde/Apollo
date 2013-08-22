//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Base;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetGraphModelTest
    {
        private static Mock<IProxyDataset> CreateDataset(int index, IEnumerable<Mock<IProxyDataset>> children)
        {
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.Name)
                    .Returns(index.ToString(CultureInfo.InvariantCulture));
                proxy.Setup(p => p.Children())
                    .Returns(children.Select(c => c.Object));
                proxy.Setup(p => p.Id)
                    .Returns(new DatasetId(index));
                proxy.Setup(d => d.Equals(It.IsAny<IProxyDataset>()))
                    .Returns<IProxyDataset>(other => ReferenceEquals(other, proxy.Object));
                proxy.Setup(d => d.Equals(It.IsAny<object>()))
                    .Returns<object>(other => ReferenceEquals(other, proxy.Object));
                proxy.Setup(d => d.GetHashCode())
                    .Returns(proxy.GetHashCode());
            }

            return proxy;
        }

        [Test]
        public void AddDataset()
        {
            // Create a 'binary' tree of datasets. This should create the following tree:
            //                            X
            //                          /   \
            //                         /     \
            //                        /       \
            //                       /         \
            //                      /           \
            //                     X             X
            var nodes = new List<Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>>();

            for (int i = 0; i < 3; i++)
            {
                var childCollection = new List<Mock<IProxyDataset>>();
                var child = CreateDataset(i, childCollection);

                nodes.Add(new Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>(child, childCollection));
            }

            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var tracker = new Mock<ITrackSteppingProgress>();
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.BaseDataset())
                    .Returns(nodes[0].Item1.Object);
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            Func<DatasetFacade, DatasetModel> toModel = d => new DatasetModel(context.Object, tracker.Object, projectLink.Object, d);
            var model = new DatasetGraphModel(context.Object, projectFacade, toModel);

            var graph = model.Graph;
            var rootVertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 0.ToString(CultureInfo.InvariantCulture)));
            
            var parent = nodes[0];
            
            // First node
            parent.Item2.Add(nodes[1].Item1);
            project.Raise(p => p.OnDatasetCreated += null, EventArgs.Empty);

            var vertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 1.ToString(CultureInfo.InvariantCulture)));
            Assert.IsNotNull(vertex);

            var edges = graph.InEdges(vertex);
            Assert.AreEqual(1, edges.Count());
            Assert.AreSame(rootVertex, edges.First().Source);

            // Second node
            parent.Item2.Add(nodes[2].Item1);
            project.Raise(p => p.OnDatasetCreated += null, EventArgs.Empty);

            vertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 2.ToString(CultureInfo.InvariantCulture)));
            Assert.IsNotNull(vertex);

            edges = graph.InEdges(vertex);
            Assert.AreEqual(1, edges.Count());
            Assert.AreSame(rootVertex, edges.First().Source);
        }

        [Test]
        public void RemoveDataset()
        {
            // Create a 'binary' tree of datasets. This should create the following tree:
            //                            X
            //                          /   \
            //                         /     \
            //                        /       \
            //                       /         \
            //                      /           \
            //                     X             X
            var nodes = new List<Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>>();

            for (int i = 0; i < 3; i++)
            {
                var childCollection = new List<Mock<IProxyDataset>>();
                var child = CreateDataset(i, childCollection);

                nodes.Add(new Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>(child, childCollection));
            }

            nodes[0].Item2.Add(nodes[1].Item1);
            nodes[0].Item2.Add(nodes[2].Item1);

            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var tracker = new Mock<ITrackSteppingProgress>();
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.BaseDataset())
                    .Returns(nodes[0].Item1.Object);
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            Func<DatasetFacade, DatasetModel> toModel = d => new DatasetModel(context.Object, tracker.Object, projectLink.Object, d);
            var model = new DatasetGraphModel(context.Object, projectFacade, toModel);

            var graph = model.Graph;
            var parent = nodes[0];
            
            // First node
            parent.Item2.Remove(nodes[1].Item1);
            project.Raise(p => p.OnDatasetDeleted += null, EventArgs.Empty);

            Assert.AreEqual(2, graph.VertexCount);
            var vertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 1.ToString(CultureInfo.InvariantCulture)));
            Assert.IsNull(vertex);
            vertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 2.ToString(CultureInfo.InvariantCulture)));
            Assert.IsNotNull(vertex);

            // Second node
            parent.Item2.Remove(nodes[2].Item1);
            project.Raise(p => p.OnDatasetDeleted += null, EventArgs.Empty);

            Assert.AreEqual(1, graph.VertexCount);
            vertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 1.ToString(CultureInfo.InvariantCulture)));
            Assert.IsNull(vertex);
            vertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 2.ToString(CultureInfo.InvariantCulture)));
            Assert.IsNull(vertex);
        }

        [Test]
        public void ReloadProject()
        {
            // Create a 'binary' tree of datasets. This should create the following tree:
            //                            X
            //                          /   \
            //                         /     \
            //                        /       \
            //                       /         \
            //                      /           \
            //                     X             X
            //                   /   \         /   \
            //                  /     \       /     \
            //                 /       \     /       \
            //                X         X   X         X
            //              /   \     /   \
            //             X     X   X     X
            var nodes = new List<Mock<IProxyDataset>>();
            var datasets = new Queue<Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>>();
            var rootChildren = new List<Mock<IProxyDataset>>();
            var root = CreateDataset(0, rootChildren);
            datasets.Enqueue(new Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>(root, rootChildren));
            nodes.Add(root);

            int count = 0;
            while (count < 10)
            {
                var tuple = datasets.Dequeue();

                count++;
                var leftChildCollection = new List<Mock<IProxyDataset>>();
                var leftChild = CreateDataset(count, leftChildCollection);

                tuple.Item2.Add(leftChild);
                datasets.Enqueue(new Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>(leftChild, leftChildCollection));
                nodes.Add(leftChild);

                count++;
                var rightChildCollection = new List<Mock<IProxyDataset>>();
                var rightChild = CreateDataset(count, rightChildCollection);

                tuple.Item2.Add(rightChild);
                datasets.Enqueue(new Tuple<Mock<IProxyDataset>, List<Mock<IProxyDataset>>>(rightChild, rightChildCollection));
                nodes.Add(rightChild);
            }

            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var tracker = new Mock<ITrackSteppingProgress>();
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.BaseDataset())
                    .Returns(root.Object);
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            Func<DatasetFacade, DatasetModel> toModel = d => new DatasetModel(context.Object, tracker.Object, projectLink.Object, d);
            var model = new DatasetGraphModel(context.Object, projectFacade, toModel);

            var graph = model.Graph;
            Assert.AreEqual(nodes.Count, graph.VertexCount);

            var rootVertex = graph.Vertices.FirstOrDefault(d => string.Equals(d.Model.Name, 0.ToString(CultureInfo.InvariantCulture)));
            Assert.IsNotNull(rootVertex);

            var vertices = new Queue<DatasetViewVertex>();
            var linearisedVertices = new List<DatasetViewVertex>();
            vertices.Enqueue(rootVertex);
            linearisedVertices.Add(rootVertex);

            while (vertices.Count > 0)
            {
                var vertex = vertices.Dequeue();
                var outEdges = graph.OutEdges(vertex);

                foreach (var edge in outEdges)
                {
                    vertices.Enqueue(edge.Target);
                    linearisedVertices.Add(edge.Target);
                }
            }

            Assert.That(
                linearisedVertices.Select(l => l.Model.Name),
                Is.EquivalentTo(nodes.Select(n => n.Object.Name)));
        }
    }
}
