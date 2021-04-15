using System;
using System.Collections.Generic;
using GDT.Model;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.Incremental;
using Microsoft.Msagl.Layout.Initial;
using Microsoft.Msagl.Routing.Rectilinear;
using MSNode = Microsoft.Msagl.Core.Layout.Node;

namespace GDT.Generation.GenerationSteps
{
    public class GraphToGridLayoutStep : IPipelineStep
    {
        public string Name { get; } = "GraphToGridLayoutStep";

        private readonly string _layerName;
        

        public GraphToGridLayoutStep(string layerName)
        {
            _layerName = layerName;
            
        }

        public Graph ExecuteStep(Graph graph)
        {
            var layer = graph.GetLayer(_layerName);
            if (layer == null) throw new ArgumentException($"Layer {_layerName} doesn't exist in graph {graph.Name}!");

            GeometryGraph msGraph = MakeLayoutGraph(layer);
            ApplyLayout(msGraph);

            return graph;
        }

        private GeometryGraph MakeLayoutGraph(Layer layer)
        {
            GeometryGraph geometryGraph = new GeometryGraph();

            Dictionary<Entity, MSNode> nodeMapping = new Dictionary<Entity, MSNode>();
            foreach (var node in layer.GetEntitiesInLayer())
            {
                MSNode msNode = new MSNode()
                {
                    UserData = node,
                    BoundaryCurve = CurveFactory.CreateRectangle(10, 10, new Point())
                };
                
                geometryGraph.Nodes.Add(msNode);
                nodeMapping.Add(node, msNode);
            }

            foreach (var relation in layer.Relations)
            {
                geometryGraph.Edges.Add(new Edge(nodeMapping[relation.Entities[0]], nodeMapping[relation.Entities[1]]));
            }
            
            return geometryGraph;
        }

        private void ApplyLayout(GeometryGraph geometryGraph)
        {
            InitialLayout initialLayout = new InitialLayout(geometryGraph, 
                                                            new FastIncrementalLayoutSettings() { AvoidOverlaps = true });
            initialLayout.Run();
            
            RectilinearEdgeRouter router = new RectilinearEdgeRouter(geometryGraph, 1, 1, false);
            router.Run();

            foreach (var msNode in geometryGraph.Nodes)
            {
                Entity entity = (Entity) msNode.UserData;
                AddPositionComponent(entity, msNode.BoundingBox.LeftTop);
                
                Console.WriteLine($"{entity.Name} is at {msNode.BoundingBox.LeftTop}");
            }
        }

        private static void AddPositionComponent(Entity entity, Point position)
        {
            var positionComponent = new EntityComponent("PositionComponent");
            positionComponent.SetProperty("x", (int) position.X);
            positionComponent.SetProperty("y", (int) position.Y);
            entity.Components.Add(positionComponent);
        }
        
    }
}