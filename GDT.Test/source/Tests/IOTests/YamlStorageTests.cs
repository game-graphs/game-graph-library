using System;
using System.Collections.Generic;
using NUnit.Framework;
using GDT.IO;
using GDT.IO.Implementation;
using GDT.Model;
using GDT.Utility;
using GDT.Utility.Visualization;
using Microsoft.Msagl.Core.ProjectionSolver;

#nullable enable

namespace GDT.Test.Tests.IOTests
{
    public class YamlStorageTests
    {
        [SetUp]
        public void Setup()
        {
        }

        enum Faction
        {
            Horde,
            Alliance
        }
        
        [Test]
        public void TestSaveGraph()
        {
            Graph locationGraph = new Graph("Location Graph");
            
            // Add Nodes
            Entity orgrimmar = new("Orgrimmar", locationGraph);
            EntityComponent cityComponentOg = new EntityComponent("City");
            cityComponentOg.SetProperty("faction", Faction.Horde);
            cityComponentOg.SetProperty("population", 14000L);
            orgrimmar.Components.Add(cityComponentOg);
            Entity thunderbluff = new("Thunderbluff", locationGraph);

            Entity undercity = new("Undercity", locationGraph);
            Entity gromGol = new("Grom'Gol", locationGraph);
            Entity stormwind = new("Stormwind", locationGraph);
            EntityComponent cityComponentSt = new EntityComponent("City");
            cityComponentSt.SetProperty("faction", Faction.Alliance);
            cityComponentSt.SetProperty("population", 200000L);
            stormwind.Components.Add(cityComponentSt);

            Entity easternKingdoms = new("Eastern Kingdoms", locationGraph);
            easternKingdoms.AddChildren(new[] {undercity, gromGol, stormwind});

            Entity kalimdor = new("Kalimdor", locationGraph);
            kalimdor.AddChildren(new[] {orgrimmar, thunderbluff});

            locationGraph.Entities.AddRange(new[]{easternKingdoms, kalimdor});

            // Add Zeppelin Layer
            Layer zeppelinLayer = new("Zeppelin Layer");
            locationGraph.AddLayer(zeppelinLayer);

            zeppelinLayer.AddRelation(orgrimmar, gromGol);
            zeppelinLayer.AddRelation(orgrimmar, undercity);
            zeppelinLayer.AddRelation(orgrimmar, thunderbluff);
            zeppelinLayer.AddRelation(undercity, gromGol);
            
            string content = YamlGraphStorage.SaveGraph(locationGraph);

            Assert.IsTrue(content.Contains("Zeppelin Layer"), "Failed to store the 'Zeppelin Layer'.");
            Assert.IsTrue(content.Contains("Kalimdor"), "Failed to store the 'Kalimdor'.");
            Assert.IsTrue(content.Contains("Stormwind"), "Failed to store the 'Stormwind'.");
            FileUtilities.WriteFile("graph-test.yaml", "./", content);

            Graph g = YamlGraphStorage.LoadGraph(content);
            Layer? zl = g.GetLayer("Zeppelin Layer");
            Assert.NotNull(zl, "Expected to find the zeppelin layer!");
        }
        
        [Test]
        public void TestGenerateSimpleGraphSpace()
        {
            List<Tuple<int, int>> connections = new ();
            connections.AddRange(SeriesGenerator.PairSeries(0,3));
            connections.Add(new Tuple<int, int>(3,0));
            
            Graph connectionGraph = GraphBuilder.BuildGraph("Ring Graph", 4, connections);
            
            string content = YamlGraphStorage.SaveGraph(connectionGraph);
            FileUtilities.WriteFile("graph_map_simple.yaml", "./", content);

            // test save and load in the same order
            Graph g = YamlGraphStorage.LoadGraph(content);
            for (int i = 0; i < g.Entities.Count; i++)
            {
                Assert.AreEqual($"Location {i+1}", g.Entities[i].Name);
            }
            Layer? wfcNeighborLayer = g.GetLayer("WfcNeighborLayer");
            Assert.NotNull(wfcNeighborLayer, "Expected to find the WfcNeighborLayer!");
            Assert.AreEqual(4, g.Entities.Count, "Expected 4 nodes.");
        }
        
        [Test]
        public void TestGenerateMediumGraphSpace()
        {
            List<Tuple<int, int>> connections = new ();
            connections.AddRange(SeriesGenerator.PairSeries(0,5, true));
            connections.AddRange(SeriesGenerator.PairSeries(6,11, true));
            connections.Add(new Tuple<int, int>(5,11));
            connections.Add(new Tuple<int, int>(2,8));

            Graph connectionGraph = GraphBuilder.BuildGraph("Connected Rings Graph (o=o)", 12, connections);

            string content = YamlGraphStorage.SaveGraph(connectionGraph);
            FileUtilities.WriteFile("graph_map_medium.yaml", "./", content);

            // test save and load in the same order
            Graph g = YamlGraphStorage.LoadGraph(content);
            for (int i = 0; i < g.Entities.Count; i++)
            {
                Assert.AreEqual($"Location {i+1}", g.Entities[i].Name);
            }
            Layer? wfcNeighborLayer = g.GetLayer("WfcNeighborLayer");
            Assert.NotNull(wfcNeighborLayer, "Expected to find the WfcNeighborLayer!");
        }
        
        [Test]
        public void TestGenerateComplexGraphSpace()
        {
            Graph connectionGraph = new Graph("Pokemon Region Graph");
            
            // Add Nodes
            for (int i = 0; i < 43; i++)
            {
                Entity entity = new($"Location {i}", connectionGraph);
                connectionGraph.Entities.Add(entity);
            }
            
            // Add WfcNeighbor Layer
            Layer neighborLayer = new("WfcNeighborLayer");
            connectionGraph.AddLayer(neighborLayer);

            List<Tuple<int, int>> connections = new ();
            connections.AddRange(SeriesGenerator.PairSeries(0,31,true));
            //"victory road"
            connections.Add(new(2,32));
            connections.AddRange(SeriesGenerator.PairSeries(32,35));
            //"bills house"
            connections.Add(new(10,36));
            connections.AddRange(SeriesGenerator.PairSeries(36,38));
            //"saffron" - "cerulean"
            connections.AddRange(SeriesGenerator.PairWise(new[] {10, 39, 20}));
            //"saffron" - "lavender"
            connections.AddRange(SeriesGenerator.PairWise(new[] {20, 40, 14}));
            //"Fuschia" - "verm/lavender"
            connections.AddRange(SeriesGenerator.PairWise(new[] {26, 41, 42, 16}));

            foreach (var (from, to) in connections)
            {
                Assert.AreEqual($"Location {from}", connectionGraph.Entities[from].Name);
                neighborLayer.AddRelation(
                    connectionGraph.Entities[from], connectionGraph.Entities[to], 
                    $"Connection from {from} to {to}");
            }
            
            string content = YamlGraphStorage.SaveGraph(connectionGraph);
            FileUtilities.WriteFile("graph_pokemon_map.yaml", "./",  content);

            // test save and load in the same order
            Graph g = YamlGraphStorage.LoadGraph(content);
            for (int i = 0; i < g.Entities.Count; i++)
            {
                Assert.AreEqual($"Location {i}", g.Entities[i].Name);
            }
            Layer? wfcNeighborLayer = g.GetLayer("WfcNeighborLayer");
            Assert.NotNull(wfcNeighborLayer, "Expected to find the WfcNeighborLayer!");
        }

        [Test]
        public void TestGenerateTheSkeldGraph()
        {
            Graph theSkeldGraph = new Graph("The Skeld graph");

            Entity reactor = new Entity("Reactor", theSkeldGraph);
            Entity reactorVentNorth = new Entity("Reactor Vent North", theSkeldGraph);
            AddTask(reactor, "Start Reactor", "Long", "ReactorGame:Execute()", "middle");
            AddTask(reactor, "Unlock Manifolds", "Short", "NumberGame:Execute()", "top-left");
            Entity reactorVentSouth = new Entity("Reactor Vent South", theSkeldGraph);
            reactor.AddChildren(new []{reactorVentNorth, reactorVentSouth});
            
            Entity upperEngine = new Entity("Upper Engine", theSkeldGraph);
            AddTask(upperEngine, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "top");
            AddTask(upperEngine, "Align Engine Output", "Long", "AlignEngine:Execute()", "bottom-left");
            AddTask(upperEngine, "Fuel Engines", "Long", "FuelEngines:Execute()", "bottom-left");
            Entity upperEngineVent = new Entity("Upper Engine Vent", theSkeldGraph);
            upperEngine.AddChild(upperEngineVent);
            
            Entity westCorridor = new Entity("West Corridor", theSkeldGraph);
            Entity lowerEngine = new Entity("Lower Engine", theSkeldGraph);
            AddTask(lowerEngine, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "top-right");
            AddTask(lowerEngine, "Align Engine Output", "Long", "AlignEngine:Execute()", "bottom-left");
            AddTask(lowerEngine, "Fuel Engines", "Long", "FuelEngines:Execute()", "bottom-left");
            Entity lowerEngineVent = new Entity("Lower Engine Vent", theSkeldGraph);
            lowerEngine.AddChild(lowerEngineVent);
            
            Entity security = new Entity("Security", theSkeldGraph);
            AddTask(security, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "top-right");
            AddTask(security, "Fix Wiring", "Common", "Wiring:Execute()", "left");
            Entity securityVent = new Entity("Security Vent", theSkeldGraph);
            security.AddChild(securityVent);
            
            Entity medBayCorridor = new Entity("MedBay Corridor", theSkeldGraph);
            Entity medBay = new Entity("MedBay", theSkeldGraph);
            AddTask(medBay, "Submit Scan", "Long", "Scan:Execute()", "bottom");
            AddTask(medBay, "Inspect Sample", "Long", "InspectSample:Execute()", "bottom-right");
            Entity medBayVent = new Entity("MedBay Vent", theSkeldGraph);
            medBay.AddChild(medBayVent);
            
            Entity electricalCorridor = new Entity("Electrical Corridor", theSkeldGraph);
            Entity electrical = new Entity("Electrical", theSkeldGraph);
            AddTask(electrical, "Calibrate Distributor", "Short", "Distributor:Execute()", "top-right");
            AddTask(electrical, "Divert Power", "Short", "DivertPower:Execute()", "top-left");
            AddTask(electrical, "Download Data", "Short", "Download:Execute()", "top-left");
            AddTask(electrical, "Fix Wiring", "Common", "Wiring:Execute()", "top");
            Entity electricalVent = new Entity("Electrical Vent", theSkeldGraph);
            electrical.AddChild(electricalVent);
            
            Entity cafeteria = new Entity("Cafeteria", theSkeldGraph);
            AddTask(reactor, "Download Data", "Short", "Download:Execute()", "top-right");
            AddTask(reactor, "Empty Garbage", "Long", "Garbage:Execute()", "top-right");
            AddTask(reactor, "Fix Wiring", "Common", "Wiring:Execute()", "top-left");
            Entity cafeteriaVent = new Entity("Cafeteria Vent", theSkeldGraph);
            cafeteria.AddChild(cafeteriaVent);
            
            Entity adminCorridor = new Entity("Admin Corridor", theSkeldGraph);
            Entity admin = new Entity("Admin", theSkeldGraph);
            AddTask(admin, "Fix Wiring", "Common", "Wiring:Execute()", "top-left");
            AddTask(admin, "Swipe Card", "Short", "Upload:Execute()", "right");
            AddTask(admin, "Upload Data", "Short", "Upload:Execute()", "top-left");
            Entity adminVent = new Entity("Admin Vent", theSkeldGraph);
            admin.AddChild(adminVent);
            
            Entity storage = new Entity("Storage", theSkeldGraph);
            AddTask(storage, "Empty Chute", "Long", "Garbage:Execute()", "bottom-right");
            AddTask(storage, "Fix Wiring", "Common", "Wiring:Execute()", "top-left");
            AddTask(storage, "Fuel Engines", "Long", "Fuel:Execute()", "center");
            
            Entity communicationsCorridor = new Entity("Communications Corridor", theSkeldGraph);
            Entity communications = new Entity("Communications", theSkeldGraph);
            AddTask(communications, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "top-right");
            AddTask(communications, "Download Data", "Short", "Download:Execute()", "top");
            
            Entity weapons = new Entity("Weapons", theSkeldGraph);
            AddTask(weapons, "Download Data", "Short", "Download:Execute()", "top-left");
            AddTask(weapons, "Clear Asteroids", "Long", "Asteroids:Execute()", "center");
            AddTask(weapons, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "right");
            Entity weaponsVent = new Entity("Weapons Vent", theSkeldGraph);
            weapons.AddChild(weaponsVent);
            
            Entity o2 = new Entity("O2", theSkeldGraph);
            AddTask(o2, "Clean O2 Filter", "Short", "O2Filter:Execute()", "top-left");
            AddTask(o2, "Empty Garbage", "Long", "Garbage:Execute()", "bottom-left");
            AddTask(o2, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "right");
            Entity eastCorridor = new Entity("East Corridor", theSkeldGraph);
            Entity eastCorridorVent = new Entity("East Corridor Vent", theSkeldGraph);
            eastCorridor.AddChild(eastCorridorVent);
            
            Entity navigation = new Entity("Navigation", theSkeldGraph);
            AddTask(navigation, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "top-left");
            AddTask(navigation, "Chart Course", "Short", "ChartCourse:Execute()", "top-right");
            AddTask(navigation, "Download Data", "Short", "Download:Execute()", "top-right");
            AddTask(navigation, "Fix Wiring", "Common", "Wiring:Execute()", "left");
            AddTask(navigation, "Stabilize Steering", "Short", "Steering:Execute()", "right");
            Entity navigationVentNorth = new Entity("Navigation Vent North", theSkeldGraph);
            Entity navigationVentSouth = new Entity("Navigation Vent South", theSkeldGraph);
            navigation.AddChildren(new []{navigationVentNorth, navigationVentSouth});
            
            Entity shields = new Entity("Shields", theSkeldGraph);
            AddTask(shields, "Accept Diverted Power", "Short", "DivertedPower:Execute()", "top-right");
            AddTask(shields, "Prime Shields", "Short", "Shields:Execute()", "bottom-left");
            Entity shieldsVent = new Entity("Shields Vent", theSkeldGraph);
            shields.AddChild(shieldsVent);
            
            theSkeldGraph.Entities.AddRange(new []{reactor, upperEngine, westCorridor, lowerEngine, security, medBay, 
                medBayCorridor, electrical, electricalCorridor, cafeteria, admin, adminCorridor, storage, communications, 
                communicationsCorridor, weapons, o2, eastCorridor, navigation, shields});

            // Add path layer
            Layer pathLayer = new("Path Layer");
            theSkeldGraph.AddLayer(pathLayer);
            
            pathLayer.AddRelation(reactor, westCorridor);
            pathLayer.AddRelation(upperEngine, westCorridor);
            pathLayer.AddRelation(security, westCorridor);
            pathLayer.AddRelation(lowerEngine, westCorridor);
            
            pathLayer.AddRelation(upperEngine, medBayCorridor);
            pathLayer.AddRelation(medBay, medBayCorridor);
            pathLayer.AddRelation(cafeteria, medBayCorridor);
            
            pathLayer.AddRelation(lowerEngine, electricalCorridor);
            pathLayer.AddRelation(electrical, electricalCorridor);
            pathLayer.AddRelation(storage, electricalCorridor);
            
            pathLayer.AddRelation(cafeteria, adminCorridor);
            pathLayer.AddRelation(admin, adminCorridor);
            pathLayer.AddRelation(storage, adminCorridor);
            
            pathLayer.AddRelation(cafeteria, weapons);
            
            pathLayer.AddRelation(weapons, eastCorridor);
            pathLayer.AddRelation(o2, eastCorridor);
            pathLayer.AddRelation(navigation, eastCorridor);
            pathLayer.AddRelation(shields, eastCorridor);
            
            pathLayer.AddRelation(storage, communicationsCorridor);
            pathLayer.AddRelation(communications, communicationsCorridor);
            pathLayer.AddRelation(shields, communicationsCorridor);
            
            
            // Add vent layer
            Layer ventLayer = new("Vent Layer");
            theSkeldGraph.AddLayer(ventLayer);
            
            ventLayer.AddRelation(upperEngineVent, reactorVentNorth);
            ventLayer.AddRelation(lowerEngineVent, reactorVentSouth);
            
            ventLayer.AddRelation(securityVent, medBayVent);
            ventLayer.AddRelation(securityVent, electricalVent);
            ventLayer.AddRelation(medBayVent, electricalVent);
            
            ventLayer.AddRelation(cafeteriaVent, eastCorridorVent);
            ventLayer.AddRelation(cafeteriaVent, adminVent);
            ventLayer.AddRelation(eastCorridorVent, adminVent);
            
            ventLayer.AddRelation(weaponsVent, navigationVentNorth);
            ventLayer.AddRelation(shieldsVent, navigationVentSouth);
            
            string content = YamlGraphStorage.SaveGraph(theSkeldGraph);
            FileUtilities.WriteFile("the_skeld_map.yml", "./",  content);

            // test save and load in the same order
            Graph g = YamlGraphStorage.LoadGraph(content);
            Assert.AreEqual(20, g.Entities.Count, "Expected 20 rooms");
            
            Layer? testVentLayer = g.GetLayer("Vent Layer");
            Assert.NotNull(testVentLayer, "Expected to find the Vent Layer!");
            Assert.AreEqual(10, testVentLayer?.Relations.Count, "Expected 10 vent connections");
            
            Layer? testPathLayer = g.GetLayer("Path Layer");
            Assert.NotNull(testVentLayer, "Expected to find the Path Layer!");
            Assert.AreEqual(21, testPathLayer?.Relations.Count, "Expected 21 path connections");
        }

        private static void AddTask(Entity entity, string taskName, string type, string luaScript, string approximateLocation)
        {
            if (!entity.Graph.TryGetTarget(out var graph)) throw new NullReferenceException("Expected the node to be part of a graph");
            
            Entity taskEntity = new(taskName, graph);
            EntityComponent taskComponent = new ("Task Component");
            taskComponent.SetProperty("type", type);
            taskComponent.SetProperty("lua_script", luaScript);
            taskComponent.SetProperty("approximate_location", approximateLocation);

            taskEntity.Components.Add(taskComponent);
            entity.AddChild(taskEntity);
        }

    }
}