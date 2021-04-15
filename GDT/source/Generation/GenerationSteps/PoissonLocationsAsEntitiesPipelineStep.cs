#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;
using GDT.Algorithm.Sampler;
using GDT.Generation;
using GDT.Model;
using GDT.Utility;

namespace GDT.Generation.GenerationSteps
{
    public class PoissonLocationsAsEntitiesPipelineStep : IPipelineStep
    {
        public string Name { get; } = nameof(PoissonLocationsAsEntitiesPipelineStep);

        private readonly float _radius;
        private readonly Vector2 _offset;
        private readonly Vector2 _sampleRegionSize;
        private readonly int _rejectionThreshold;
        
        private readonly Entity? _parent;
        private readonly Func<float, float, string> _cellNameSupplier = (x, y) => $"Entity ({x:0},{y:0})";

        public PoissonLocationsAsEntitiesPipelineStep(float radius, Vector2 offset, Vector2 sampleRegionSize, Entity? parent, int rejectionThreshold = 30)
        {
            _radius = radius;
            _offset = offset;
            _sampleRegionSize = sampleRegionSize;
            _parent = parent;
            _rejectionThreshold = rejectionThreshold;
        }

        public Graph ExecuteStep(Graph graph)
        {
            
            List<Vector2> points = PoissonDiscSampler.GeneratePoints(_radius, _sampleRegionSize, _offset, _rejectionThreshold);
            
            foreach (var point in points)
            {
                Entity entity = new(_cellNameSupplier.Invoke(point.X, point.Y), graph);
                ComponentUtility.AddPosition2D(entity, point.X, point.Y);

                if (_parent != null) _parent.AddChild(entity);
                else graph.Entities.Add(entity);
            }

            return graph;
        }
    }
}