using NUnit.Framework;
using System;

using GDT.Model;

namespace GDT.Test.Tests.GraphTests
{
    public class IntegrationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGraphWithLayerAndRelation()
        {
            Graph locationGraph = new Graph("Location Graph");
            
            // Add Nodes
            Entity orgrimmar = new Entity("Orgrimmar", locationGraph);
            Entity undercity = new Entity("Undercity", locationGraph);
            Entity gromGol = new Entity("Grom'Gol", locationGraph);
            Entity stormwind = new Entity("Stormwind", locationGraph);
            locationGraph.Entities.AddRange(new Entity[] {orgrimmar, undercity, gromGol, stormwind});

            // Add Zeppelin Layer
            Layer zeppelinLayer = new Layer("Zeppelin Layer");
            locationGraph.AddLayer(zeppelinLayer);

            zeppelinLayer.AddRelation(orgrimmar, gromGol);
            zeppelinLayer.AddRelation(orgrimmar, undercity);
            zeppelinLayer.AddRelation(undercity, gromGol);

            Console.WriteLine(locationGraph.ID.ToString());
            Assert.AreEqual(4, locationGraph.Entities.Count, "Expected 4 nodes (og, uc, gg, sw).");
            Assert.AreEqual(1, locationGraph.GetLayers().Count, "Expected exactly one Layer (zeppelin).");
            Assert.AreEqual(3, zeppelinLayer.Relations.Count, "Expected 3 relations (og-uc, og-gg, uc-gg).");
        }

        [Test]
        public void TestParentChildNodes()
        {
            Graph locationGraph = new Graph("Location Graph");
            
            // Add Nodes
            Entity orgrimmar = new Entity("Orgrimmar", locationGraph);
            Entity thunderbluff = new Entity("Thunderbluff", locationGraph);

            Entity undercity = new Entity("Undercity", locationGraph);
            Entity gromGol = new Entity("Grom'Gol", locationGraph);
            Entity stormwind = new Entity("Stormwind", locationGraph);

            Entity easternKingdoms = new Entity("Eastern Kingdoms", locationGraph);
            easternKingdoms.AddChildren(new Entity[] {undercity, gromGol, stormwind});

            Entity kalimdor = new Entity("Kalimdor", locationGraph);
            kalimdor.AddChildren(new Entity[] {orgrimmar, thunderbluff});

            locationGraph.Entities.AddRange(new Entity[]{easternKingdoms, kalimdor});

            // Add Zeppelin Layer
            Layer zeppelinLayer = new Layer("Zeppelin Layer");
            locationGraph.AddLayer(zeppelinLayer);

            zeppelinLayer.AddRelation(orgrimmar, gromGol);
            zeppelinLayer.AddRelation(orgrimmar, undercity);
            zeppelinLayer.AddRelation(orgrimmar, thunderbluff);
            zeppelinLayer.AddRelation(undercity, gromGol);

            Assert.AreEqual(2, locationGraph.Entities.Count, "Expected 2 nodes (kalimdor, eastern kingdoms).");
            Assert.AreEqual(1, locationGraph.GetLayers().Count, "Expected exactly one Layer (zeppelin).");
            Assert.AreEqual(4, zeppelinLayer.Relations.Count, "Expected 4 relations (og-uc, og-gg, uc-gg, og-tb).");
            Entity orgrimmarParent;
            Assert.IsTrue(orgrimmar.Parent.TryGetTarget(out orgrimmarParent), "Orgrimmar should have a parent");
            Assert.AreEqual("Kalimdor", orgrimmarParent.Name, "Orgrimmar should have Kalimdor as a parent");
        }

        [Test]
        public void TestNodeComponents()
        {
            Graph locationGraph = new Graph("Location Graph");
            Entity munich = new Entity("Munich", locationGraph);
            EntityComponent cityComponent = new EntityComponent("City");
            cityComponent.SetProperty("inhabitants", 123456789L);
            cityComponent.SetProperty("iceConnection", true);
            munich.Components.Add(cityComponent);

            var testCityComponent = munich.GetComponent("City");
            var inhabitants = testCityComponent?.Get<Int64>("inhabitants");
            Assert.AreEqual(123456789L, inhabitants, "Number of inhabitants didn't match");
            var iceConnection = testCityComponent?.Get<bool>("iceConnection");
            Assert.AreEqual(true, iceConnection, "City component should have an ice connection.");

            Assert.Catch<ArgumentException>(() => testCityComponent?.Get<string>("inhabitants"), "inhabitants should not be readable as an int");
        }
    }
}