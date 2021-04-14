using System;
using System.Collections.Generic;
using System.Linq;
using GDT.Model;

namespace GDT.Utility
{
    public static class GraphBuilder
    {
        public static Graph BuildCircleGraph(string graphName, int numberOfNodes,
            string layerName = "WfcNeighborLayer", string nodeNameFormat = "Location {0}")
        {
            var connections = SeriesGenerator.PairSeries(0, numberOfNodes-1, true);
            
            return BuildGraph(graphName, numberOfNodes, connections.ToList(), layerName, nodeNameFormat);
        }
        
        public static Graph BuildGraph(string graphName, int numberOfNodes, List<Tuple<int, int>> connections,
                                            string layerName = "WfcNeighborLayer", string nodeNameFormat = "Location {0}")
        {
            Graph connectionGraph = new Graph(graphName);
            
            for (int i = 1; i <= numberOfNodes; i++)
            {
                Entity entity = new(String.Format(nodeNameFormat, i), connectionGraph);
                connectionGraph.Entities.Add(entity);
            }
            
            Layer neighborLayer = new(layerName);
            connectionGraph.AddLayer(neighborLayer);

            foreach (var (from, to) in connections)
            {
                var fromNode = connectionGraph.Entities[from];
                var toNode = connectionGraph.Entities[to];
                neighborLayer.AddRelation(fromNode, toNode, 
                    $"Connection '{fromNode.Name}' <-> '{toNode.Name}'");
            }

            return connectionGraph;
        }
    }
}