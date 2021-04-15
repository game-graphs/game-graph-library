using System;
using GDT.Generation;
using GDT.Model;
using GDT.Utility.Visualization.DataStructures;

namespace GDT.Tests.PipelineTests.Steps
{
    public class AreaTilesStep : IPipelineStep
    {
        public string Name { get; } = nameof(AreaTilesStep);
        private readonly string _parentLayerName;
        private readonly string _newLayerName;
        

        public AreaTilesStep(string parentLayerName, string newLayerName)
        {
            _parentLayerName = parentLayerName;
            _newLayerName = newLayerName;
        }

        public Graph ExecuteStep(Graph graph)
        {
            var parentLayer = graph.GetLayer(_parentLayerName);
            if (parentLayer == null) throw new NullReferenceException($"No layer named '{_parentLayerName}'!");

            var newLayer = graph.GetOrAddLayer(_newLayerName);

            foreach (var node in parentLayer.GetEntitiesInLayer())
            {
                var neighbors = parentLayer.CountNeighbors(node);

                if (neighbors == 4)
                {
                    new CrossingXTile().AddToGraph(graph, newLayer, node);
                }
                else if (neighbors == 3)
                {
                    new CrossingTTile().AddToGraph(graph, newLayer, node);
                }
                else if (neighbors <= 2)
                {
                    new ChainTile().AddToGraph(graph, newLayer, node);
                }
                else
                {
                    Console.Write($"ERROR [{nameof(AreaTilesStep)}]: Number of neighbors not supported ({neighbors})");
                }
            }

            return graph;
        }
        
        
    }
}