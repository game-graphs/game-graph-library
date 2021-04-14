using GDT.IO.Implementation;
using GDT.Model;
using GDT.Utility.Visualization;
using GDT.Utility;
using NUnit.Framework;

namespace GDT.Test.Tests.GraphTests
{
    public class UnitTests
    {
        private Graph _graph;
        
        [SetUp]
        public void Setup()
        {
            _graph = YamlGraphStorage.LoadGraph(FileUtilities.LoadFile("graph_with_layers.yaml"));
        }

        [Test]
        public void TestGraphClone()
        {
            Graph clonedGraph = _graph.Clone();
            
            Assert.AreNotSame(_graph, clonedGraph);
            for(int i = 0; i < clonedGraph.Entities.Count; i++)
            {
                Assert.AreEqual(_graph.Entities[i].Name, clonedGraph.Entities[i].Name, "Expected the same node index to have equal name");
                Assert.AreNotSame(_graph.Entities[i], clonedGraph.Entities[i], "Expected the same node index not to have the same object");
            }

            for (int i = 0; i < clonedGraph.GetLayers().Count; i++)
            {
                Assert.AreEqual(_graph.GetLayers()[i].Name, clonedGraph.GetLayers()[i].Name, "Expected the same layer index to have equal name");
                Assert.AreNotSame(_graph.GetLayers()[i], clonedGraph.GetLayers()[i], "Expected the same layer index not to have the same object");
            }
        }
        
        [Test]
        public void GetRandomRelationTest()
        {
            foreach (var layer in _graph.GetLayers())
            {
                if (layer.Relations.Count > 0)
                {
                    var randomRelation = layer.GetRandomRelation();
                    Assert.NotNull(randomRelation, "Expected to find at least one relation!");
                    Assert.Contains(randomRelation, layer.Relations, $"Expected {randomRelation.Name} in {layer.Name}!");
                }
            }
        }
    }
}