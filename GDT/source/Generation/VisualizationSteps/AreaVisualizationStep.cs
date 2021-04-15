#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GDT.Algorithm.VoronoiLib.Structures;
using GDT.Model;
using GDT.Utility.Visualization;

namespace GDT.Generation.VisualizationSteps
{
    public class AreaVisualizationStep : IPipelineStep
    {
        public string Name { get; } = nameof(AreaVisualizationStep);

        private readonly Func<Graph, List<Entity>> _getEntitiesWithAreas;
        private readonly Func<Entity, List<PointF>> _getPointsFromEntity = AreaFromEntityToVector2;
        private readonly Action<ImageDrawing> _onImageProduced;
        private readonly int _width;
        private readonly int _height;
        
        private readonly Color _areaColor;
        private readonly Func<Entity, Color>? _getColorFromEntity = null;
        private readonly Color _backgroundColor = Color.Transparent;

        public AreaVisualizationStep( 
            int width, 
            int height, 
            Color areaColor,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>> getEntitiesWithAreas, 
            Func<Entity, List<PointF>>? getPointsFromEntity = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _areaColor = areaColor;
            _onImageProduced = onImageProduced;
            _getEntitiesWithAreas = getEntitiesWithAreas;
            _getPointsFromEntity = getPointsFromEntity ?? _getPointsFromEntity;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }
        
        public AreaVisualizationStep( 
            int width, 
            int height, 
            Func<Entity, Color> getColorFromEntity,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>> getEntitiesWithAreas, 
            Func<Entity, List<PointF>>? getPointsFromEntity = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _getColorFromEntity = getColorFromEntity;
            _onImageProduced = onImageProduced;
            _getEntitiesWithAreas = getEntitiesWithAreas;
            _getPointsFromEntity = getPointsFromEntity ?? _getPointsFromEntity;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }

        public Graph ExecuteStep(Graph graph)
        {
            ImageDrawing drawing = new (_width, _height);
            drawing.Clear(_backgroundColor);

            if (_getColorFromEntity == null)
            {
                var areas = _getEntitiesWithAreas(graph).Select(entity => _getPointsFromEntity(entity)).ToList();
                areas.ForEach(area => drawing.FillPoly(area, _areaColor));
            }
            else
            {
                var entityAreaPairs = _getEntitiesWithAreas.Invoke(graph).Select(entity => (entity,_getPointsFromEntity.Invoke(entity))).ToList();
                entityAreaPairs.ForEach(
                    (entityPointPair) => drawing.FillPoly(entityPointPair.Item2, _getColorFromEntity(entityPointPair.Item1)));
            }
            
            _onImageProduced(drawing);

            return graph;
        }
        
        private static List<PointF> AreaFromEntityToVector2(Entity entity)
        {
            var areaComponent = entity.GetComponent("AreaComponent");
            var site = areaComponent?.Get<FortuneSite?>("cell");
            var points = site?.Points.Select(p => new PointF((float) p.X, (float) p.Y)).ToList() ?? new List<PointF>();
            return points;
        }
        
    }
}