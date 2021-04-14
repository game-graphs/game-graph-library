using System;
using System.Collections.Generic;
using GDT.Algorithm;
using GDT.Generation;
using GDT.Generation.GenerationSteps;
using GDT.IO.Implementation;
using GDT.Model;
using GDT.Utility.Visualization;
using GDT.Utility.Visualization.DataStructures;
using GDT.Tests.PipelineTests.Steps;
using GDT.Utility;
using NUnit.Framework;

namespace GDT.Test.Tests.PipelineTests
{
    public class PocketWorldPipelineTests
    {
        private string _luaScript;
        private List<NLuaImport> _additionalImports;
        [SetUp]
        public void Setup()
        {
            _luaScript = FileUtilities.LoadFile("pocket_world_pipeline.lua");
            _additionalImports = new List<NLuaImport>()
            {
                new("GDT", "GDT.Utility"),
                new ("System")
            };
        }
        
        [Test]
        public void PocketWorldPipelineInitializationTest()
        {
            Pipeline pipeline = PipelineUtility.CreateEmptyPipelineWithLuaGraph(_luaScript, _additionalImports);

            Graph resultGraph = pipeline.Execute();
            
            Assert.NotNull(resultGraph, "There should be a graph object");
            Assert.AreEqual("Pocket World", resultGraph.Name, "Expected the graph to be called 'Pocket World'");
            Assert.AreEqual("Zone 1", resultGraph.Entities[0].Name, "Expected the graph to have a child node named 'Area 0'");

            var areaLayer = resultGraph.GetLayer("Zone Layer");
            Assert.NotNull(areaLayer, "Expected to find an 'Zone Layer'");

            FileUtilities.WriteFile("GeneratedGraphFromLua.dot", "./", DotGraphVisualizerStorage.SaveGraph(resultGraph));
        }
        
        
        [Test]
        public void PocketWorldPipelineTest()
        {
            Pipeline pocketWorldPipeline = PipelineUtility.CreateEmptyPipelineWithLuaGraph(_luaScript, _additionalImports);
            pocketWorldPipeline.AddStep(new AreaTilesStep("Zone Layer", "Area Layer"));
            pocketWorldPipeline.AddStep(new AreaTileConnectorStep("Zone Layer", "Area Layer"));
            pocketWorldPipeline.AddStep(SetupWfcStep());
            pocketWorldPipeline.AddStep(new ChildToParentPropagatorStep("Area Layer"));
            pocketWorldPipeline.AddStep(new GraphToGridLayoutStep("Area Layer"));

            Graph pocketWorldGraph = pocketWorldPipeline.Execute();
            Console.WriteLine("--- Final Map: ---");
            Debugging.PrintGraph(pocketWorldGraph.Entities);
            FileUtilities.WriteFile("PocketWorldGraph.dot", "./", DotGraphVisualizerStorage.SaveLayerGraph(pocketWorldGraph.GetLayer("Area Layer")));
        }

        private WfcPipelineStep<WorldData> SetupWfcStep()
        {
            Blackboard blackboard = new Blackboard();
            var availableModules = PokeWorldGraphTiles.GetPocketWorldGraphTiles(blackboard);
            return new WfcPipelineStep<WorldData>(availableModules, blackboard, true,
                    (wfcSpace, selectedModule) =>
                    {
                        Console.WriteLine($"Selected Type: {selectedModule.Graph.Name}");
                        wfcSpace.Blackboard.Increment($"{selectedModule.Graph.Name}_count", -1);
                    },
                    "Area Layer"
                );
        }
    }
}