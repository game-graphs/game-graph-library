using NUnit.Framework;
using GDT.IO;
using GDT.IO.Implementation;
using GDT.Model;
using GDT.Utility.Visualization;

#nullable enable

namespace GDT.Test.Tests.IOTests
{
    public class YamlLoaderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestLoadFile()
        {
            string content = FileUtilities.LoadFile("graph_with_layers.yaml");
            Assert.IsTrue(content.Contains("Orgrimmar"), "Failed to load content. Check the paths!");
        }

        [Test]
        public void TestParseNodes()
        {
            string content = FileUtilities.LoadFile("graph_with_layers.yaml");
            Graph graph = YamlGraphStorage.LoadGraph(content);

            Assert.AreEqual("LocationGraph", graph.Name, "Graph name not correct. Probably error in parser!");
            Assert.AreEqual(2, graph.Entities.Count, "Expected 'kalimdor' and 'eastern kingdoms' to be correctly parsed!");
        }

        [Test]
        public void TestParseLayers()
        {
            string content = FileUtilities.LoadFile("graph_with_layers.yaml");
            Graph graph = YamlGraphStorage.LoadGraph(content);

            Assert.AreEqual("LocationGraph", graph.Name, "Graph name not correct. Probably error in parser!");
            Assert.AreEqual(1, graph.GetLayers().Count, "Layer correctly parsed!");
        }
        
        
        
        [Test]
        public void TestParseChildren()
        {
            string content = FileUtilities.LoadFile("graph_with_layers.yaml");
            Graph graph = YamlGraphStorage.LoadGraph(content);

            Assert.AreEqual("LocationGraph", graph.Name, "Graph name not correct. Probably error in parser!");
            Entity? kalimdor = graph.GetEntity("Kalimdor");
            Assert.NotNull(kalimdor, "Expected 'Kalimdor' to be in the node list!");
            Entity? orgrimmar = kalimdor?.GetChild("Orgrimmar");
            Assert.NotNull(orgrimmar, "Expected 'Orgrimmar' to be a child of Kalimdor!");
        }
        
        [Test]
        public void TestParseReferencedNodes()
        {
            string content = FileUtilities.LoadFile("graph_with_layers.yaml");
            Graph graph = YamlGraphStorage.LoadGraph(content);

            Assert.AreEqual("LocationGraph", graph.Name, "Graph name not correct. Probably error in parser!");
            Layer? zeppelin = graph.GetLayer("Zeppelin Connections");
            Assert.NotNull(zeppelin, "Expected 'Zeppelin Connections' to be a layer!");
            Relation? ogUCRelation = zeppelin?.GetRelation("OG-UC");
            Assert.NotNull(ogUCRelation, "Expected 'OG-UC' to be a relation in the Zeppelin Layer!");
            Entity? orgrimmar = ogUCRelation?.GetEntity("Orgrimmar");
            Assert.NotNull(orgrimmar, "Expected Orgrimmar to be in the OG-UC relation");
            Entity? uc = ogUCRelation?.GetEntity("Undercity");
            Assert.NotNull(uc, "Expected Undercity to be in the OG-UC relation");
        }
        
        [Test]
        public void TestParseComponents()
        {
            string content = FileUtilities.LoadFile("graph_with_layers.yaml");
            Graph graph = YamlGraphStorage.LoadGraph(content);
            
            Entity? stormwind = graph.GetEntity("Stormwind");
            Assert.NotNull(stormwind, "Expected to find 'Stormwind' in the graph");

            EntityComponent? cityComponent = stormwind?.GetComponent("City");
            Assert.NotNull(cityComponent, "Expected 'Stormwind' to have a 'City' component");

            int? population = cityComponent?.Get<int>("Population");
            Assert.AreEqual(200000, population, "Expected a 'Population' attribute with value '200000'");

            string? levelRange = cityComponent?.Get<System.String>("LevelRange");
            Assert.AreEqual("1-60", levelRange, "Expected a 'LevelRange' attribute with value '1-60'");
        }

    }
}