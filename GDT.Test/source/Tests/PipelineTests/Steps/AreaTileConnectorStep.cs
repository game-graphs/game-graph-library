using System;
using System.Linq;
using GDT.Generation;
using GDT.Model;

namespace GDT.Tests.PipelineTests.Steps
{
    public class AreaTileConnectorStep : IPipelineStep
    {
        public string Name { get; } = "AreaTileConnectorStep";

        private readonly string _parentLayerName;
        private readonly string _childLayerName;
        private static readonly Random Random = new Random();

        public AreaTileConnectorStep(string parentLayerName, string childLayerName)
        {
            _parentLayerName = parentLayerName;
            _childLayerName = childLayerName;
        }

        public Graph ExecuteStep(Graph graph)
        {
            var parentLayer = graph.GetLayer(_parentLayerName);
            var childLayer = graph.GetLayer(_childLayerName);

            if (parentLayer == null || childLayer == null) throw new NullReferenceException($"parent ({parentLayer}) or child ({childLayer}) layer is null!");

            foreach (var parentRelation in parentLayer.Relations)
            {
                ConnectChildren(parentRelation, childLayer);
            }

            return graph;
        }

        private void ConnectChildren(Relation parentRelation, Layer childLayer)
        {
            var np0 = parentRelation.Entities[0];
            var np1 = parentRelation.Entities[1];

            var cp0 = FindRandomUnconnectedChildNode(np0, childLayer);
            var cp1 = FindRandomUnconnectedChildNode(np1, childLayer);
            childLayer.AddRelation(cp0, cp1, $"InterCon: {np0.Name} to {np1.Name}");
        }

        private Entity FindRandomUnconnectedChildNode(Entity parentEntity, Layer childLayer)
        {
            var unconnectedChildren = parentEntity.Children.Where(childNode => childLayer.CountNeighbors(childNode) == 1).ToList();
            if (unconnectedChildren.Count == 0) throw new ArgumentException("The passed layers contained an invalid neighbor setup (neighbor count invalid)!");

            return unconnectedChildren[Random.Next(0, unconnectedChildren.Count - 1)];
        }
    }
}