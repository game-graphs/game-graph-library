using System;
using System.Collections.Generic;
using GDT.Algorithm.WFC;
using GDT.Algorithm.WFC.Implementation.Graph;
using GDT.IO.Implementation;
using GDT.Model;
using GDT.Algorithm;
using GDT.Utility.Visualization;
using GDT.Utility.Visualization.DataStructures;
using NUnit.Framework;

namespace GDT.Test.Tests.WfcGraphTests
{
    public class PocketWorldTests
    {
        private Random _random;
        
        [SetUp]
        public void Setup()
        {
            _random = new Random();
        }

        [Test]
        public void GenerateSimpleGraphWorldTest()
        {
            Graph worldGraph = YamlGraphStorage.LoadGraph(FileUtilities.LoadFile("graph_map_simple.yaml"));
            
            Blackboard blackboard = new Blackboard();
            List<WfcGraphTile<WorldData>> availableTiles = PokeWorldGraphTiles.GetPocketWorldGraphTiles(blackboard, 3, 2, 0);
            WfcGraphSpace<WorldData> wfcGraphSpace = new WfcGraphSpace<WorldData>(worldGraph, availableTiles, blackboard);

            while (!wfcGraphSpace.GenerationFinished())
            {
                Console.WriteLine("--- Current Map: ---");
                Debugging.PrintWfcGraphModules<WorldData>(worldGraph.Entities);
                Console.WriteLine("----");
                WfcCell<WfcGraphTile<WorldData>, int> selectedCell = wfcGraphSpace.SelectCell();
                Console.WriteLine($"Selected Position: {selectedCell.Position}");
                wfcGraphSpace.Collapse(selectedCell, (space, tile) =>
                {
                    Console.WriteLine($"Selected Type: {tile.Graph.Name}");
                    wfcGraphSpace.Blackboard.Increment($"{tile.Graph.Name}_count", -1);
                });
            }
            Console.WriteLine("--- Final Map: ---");
            Debugging.PrintWfcGraphModules<WorldData>(worldGraph.Entities);
            Console.WriteLine("Generation finished!");
        }
        
        [Test]
        public void GenerateMediumGraphWorldTest()
        {
            Graph worldGraph = YamlGraphStorage.LoadGraph(FileUtilities.LoadFile("graph_map_medium.yaml"));
            
            Blackboard blackboard = new Blackboard();
            List<WfcGraphTile<WorldData>> availableTiles = PokeWorldGraphTiles.GetPocketWorldGraphTiles(blackboard, 10, 4, 0);
            WfcGraphSpace<WorldData> wfcGraphSpace = new WfcGraphSpace<WorldData>(worldGraph, availableTiles, blackboard);

            while (!wfcGraphSpace.GenerationFinished())
            {
                Console.WriteLine("--- Current Map: ---");
                Debugging.PrintWfcGraphModules<WorldData>(worldGraph.Entities);
                Console.WriteLine("----");
                WfcCell<WfcGraphTile<WorldData>, int> selectedCell = wfcGraphSpace.SelectCell(cell => Int32.MaxValue - cell.AvailableModules.Count);
                Console.WriteLine($"Selected Position: {selectedCell.Position}");
                wfcGraphSpace.Collapse(selectedCell, (space, tile) =>
                {
                    Console.WriteLine($"Selected Type: {tile.Graph.Name}");
                    wfcGraphSpace.Blackboard.Increment($"{tile.Graph.Name}_count", -1);
                });
                wfcGraphSpace.PropagateGlobally();
            }
            Console.WriteLine("--- Final Map: ---");
            Debugging.PrintWfcGraphModules<WorldData>(worldGraph.Entities);
            Console.WriteLine($"Generation finished! Valid Map: {wfcGraphSpace.GenerationSuccessful()}");
        }
        
        [Test]
        public void GeneratePocketWorldTest()
        {
            Graph worldGraph = YamlGraphStorage.LoadGraph(FileUtilities.LoadFile("graph_pokemon_map.yaml"));
            
            Blackboard blackboard = new Blackboard();
            List<WfcGraphTile<WorldData>> availableTiles = PokeWorldGraphTiles.GetPocketWorldGraphTiles(blackboard);
            WfcGraphSpace<WorldData> wfcGraphSpace = new WfcGraphSpace<WorldData>(worldGraph, availableTiles, blackboard);

            wfcGraphSpace.PropagateGlobally();
            while (!wfcGraphSpace.GenerationFinished())
            {
                //Console.WriteLine("--- Current Map: ---");
                //Debugging.PrintWfcGraphModules<WorldData>(worldGraph.Nodes);
                Console.WriteLine("----");
                WfcCell<WfcGraphTile<WorldData>, int> selectedCell = wfcGraphSpace.SelectCell(cell => Int32.MaxValue - cell.AvailableModules.Count);
                Console.WriteLine($"Selected Position: {selectedCell.Position}");
                wfcGraphSpace.Collapse(selectedCell, (space, tile) =>
                {
                    Console.WriteLine($"Selected Type: {tile.Graph.Name}");
                    wfcGraphSpace.Blackboard.Increment($"{tile.Graph.Name}_count", -1);
                });
                wfcGraphSpace.PropagateGlobally();
            }
            Console.WriteLine("--- Final Map: ---");
            Debugging.PrintWfcGraphModules<WorldData>(worldGraph.Entities);
            Console.WriteLine("---");
            Console.WriteLine($"Generation finished! Valid Map: {wfcGraphSpace.GenerationSuccessful()}");
        }
    }
}