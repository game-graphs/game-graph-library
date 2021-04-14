using System;
using System.Reflection.Metadata;
using GDT.Model;

namespace GDT.Generation.GenerationSteps
{
    public class ChildToParentPropagatorStep : IPipelineStep
    {
        public string Name { get; } = "ChildToParentPropagatorStep";
        private readonly string _layerName;

        public ChildToParentPropagatorStep(string layerName)
        {
            _layerName = layerName;
        }

        public Graph ExecuteStep(Graph graph)
        {
            Layer layer = graph.GetLayer(_layerName);
            if (layer == null) throw new ArgumentException($"Invalid layer name ({_layerName})!");

            foreach (var node in layer.GetNodesInLayer())
            {
                if (node.Children.Count != 1)
                    throw new ArgumentException($"Expected 1 child got {node.Children.Count}");

                var child = node.Children[0];
                node.Name = child.Name;
                node.Components.AddRange(child.Components);
                
                child.RemoveFromParent();
            }

            return graph;
        }
    }
}