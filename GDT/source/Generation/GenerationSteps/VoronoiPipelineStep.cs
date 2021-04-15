#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GDT.Algorithm.VoronoiLib;
using GDT.Algorithm.VoronoiLib.Structures;
using GDT.Generation;
using GDT.Model;
using GDT.Utility;

namespace GDT.Generation.GenerationSteps
{
    public class VoronoiPipelineStep : IPipelineStep
    {
        public string Name { get; } = nameof(VoronoiPipelineStep);

        private readonly Func<List<Vector2>> _pointSupplier;
        private readonly Func<double, double, string> _cellNameSupplier = (x, y) => $"Cell ({x:0},{y:0})";
        private readonly string _layerName;
        private readonly Vector2 _generationMin, _generationMax;
        private readonly Entity? _parent;

        public VoronoiPipelineStep(Func<List<Vector2>> pointSupplier, string layerName, Vector2 generationMin, Vector2 generationMax, Entity? parent = null, Func<double, double, string>? cellNameSupplier = null)
        {
            _pointSupplier = pointSupplier;
            _layerName = layerName;
            _generationMin = generationMin;
            _generationMax = generationMax;
            _parent = parent;
            if (cellNameSupplier != null) _cellNameSupplier = cellNameSupplier;
        }

        public Graph ExecuteStep(Graph graph)
        {
            var points = _pointSupplier.Invoke();
            var fortuneSites = points.Select(point => new FortuneSite(point.X, point.Y)).ToList();
            FortunesAlgorithm.Run(fortuneSites, _generationMin.X, _generationMin.Y, _generationMax.X, _generationMax.Y);

            // add entities to graph
            Dictionary<FortuneSite, Entity> siteToEntityMapping = new ();
            foreach (var cell in fortuneSites)
            {
                var areaEntity = new Entity(_cellNameSupplier.Invoke(cell.X, cell.Y), graph);
                EntityComponent areaComponent = new ("AreaComponent");
                areaComponent.SetProperty("cell", cell);
                areaEntity.Components.Add(areaComponent);

                ComponentUtility.AddPosition2D(areaEntity, (float) cell.X, (float) cell.Y);

                if (_parent == null) graph.Entities.Add(areaEntity);
                else _parent.AddChild(areaEntity);
                
                siteToEntityMapping.Add(cell, areaEntity);
            }

            // add relations to graph
            Layer layer = graph.GetOrAddLayer(_layerName);
            foreach (var cell in fortuneSites)
            {
                foreach (var cellNeighbor in cell.Neighbors)
                {
                    layer.AddRelation(siteToEntityMapping[cell], siteToEntityMapping[cellNeighbor]);
                }
            }
            
            return graph;
        }
    }
}