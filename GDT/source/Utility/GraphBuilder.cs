using System;
using System.Collections.Generic;
using System.Linq;
using GDT.Model;

namespace GDT.Utility
{
    public static class GraphBuilder
    {
        public static Graph BuildCircleGraph(string graphName, int numberOfEntities,
            string layerName = "WfcNeighborLayer", string entityNameFormat = "Location {0}")
        {
            var connections = SeriesGenerator.PairSeries(0, numberOfEntities-1, true);
            
            return BuildGraph(graphName, numberOfEntities, connections.ToList(), layerName, entityNameFormat);
        }
        
        public static Graph BuildGraph(string graphName, int numberOfEntities, List<Tuple<int, int>> connections,
                                            string layerName = "WfcNeighborLayer", string entityNameFormat = "Location {0}")
        {
            Graph connectionGraph = new Graph(graphName);
            
            for (int i = 1; i <= numberOfEntities; i++)
            {
                Entity entity = new(String.Format(entityNameFormat, i), connectionGraph);
                connectionGraph.Entities.Add(entity);
            }
            
            Layer neighborLayer = new(layerName);
            connectionGraph.AddLayer(neighborLayer);

            foreach (var (from, to) in connections)
            {
                var fromEntity = connectionGraph.Entities[from];
                var toEntity = connectionGraph.Entities[to];
                neighborLayer.AddRelation(fromEntity, toEntity, 
                    $"Connection '{fromEntity.Name}' <-> '{toEntity.Name}'");
            }

            return connectionGraph;
        }
    }
}