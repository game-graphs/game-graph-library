using System;
using GDT.Algorithm;
using GDT.Generation;
using GDT.Generation.GenerationSteps;
using GDT.Model;
using GDT.Utility.Visualization;
using GDT.Utility.Visualization.DataStructures;
using GDT.Utility;
using NUnit.Framework;

namespace GDT.Test.Tests.PipelineTests
{
    public class KantoPipelineTests
    {
        [Test]
        public void KantoPipelineTest()
        {
            Pipeline pocketWorldPipeline = PipelineUtility.CreateEmptyPipelineWithYamlGraph(FileUtilities.LoadFile("graph_pokemon_map.yaml"));
            pocketWorldPipeline.AddStep(SetupWfcStep());

            Graph pocketWorldGraph = pocketWorldPipeline.Execute();
            Console.WriteLine("--- Final Map: ---");
            Debugging.PrintGraph(pocketWorldGraph.Entities);
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
                    }
                );
        }
    }
}