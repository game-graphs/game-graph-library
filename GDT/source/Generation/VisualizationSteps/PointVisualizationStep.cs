#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using GDT.Model;
using GDT.Utility;
using GDT.Utility.Visualization;

namespace GDT.Generation.VisualizationSteps
{
    public class PointVisualizationStep : IPipelineStep
    {
        public string Name { get; } = nameof(PointVisualizationStep);

        private readonly Func<Graph, List<Entity>> _getEntitiesWithPoints = graph => graph.Entities;
        private readonly Func<Entity, Vector2> _getPointFromEntity = PointFromEntityToVector2;
        private readonly Action<ImageDrawing> _onImageProduced;
        private readonly int _width;
        private readonly int _height;
        private readonly float _radius;
        
        private readonly Color _pointColor;
        private readonly Func<Entity, Color>? _getColorFromEntity = null;
        private readonly Color _backgroundColor = Color.Transparent;

        public PointVisualizationStep( 
            int width, 
            int height, 
            Color pointColor, 
            float radius,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>>? getEntitiesWithPoints = null, 
            Func<Entity, Vector2>? getPointFromEntity = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _pointColor = pointColor;
            _radius = radius;
            _onImageProduced = onImageProduced;
            _getEntitiesWithPoints = getEntitiesWithPoints ?? _getEntitiesWithPoints;
            _getPointFromEntity = getPointFromEntity ?? _getPointFromEntity;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }
        
        public PointVisualizationStep( 
            int width, 
            int height, 
            Func<Entity, Color> getColorFromEntity, 
            float radius,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>>? getEntitiesWithPoints = null, 
            Func<Entity, Vector2>? getPointFromEntity = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _getColorFromEntity = getColorFromEntity;
            _radius = radius;
            _onImageProduced = onImageProduced;
            _getEntitiesWithPoints = getEntitiesWithPoints ?? _getEntitiesWithPoints;
            _getPointFromEntity = getPointFromEntity ?? _getPointFromEntity;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }

        public Graph ExecuteStep(Graph graph)
        {
            ImageDrawing drawing = new (_width, _height);
            drawing.Clear(_backgroundColor);

            if (_getColorFromEntity == null)
            {
                var points = _getEntitiesWithPoints(graph).Select(entity => _getPointFromEntity(entity)).ToList();
                drawing.FillCircles(points, _radius, _pointColor);
            }
            else
            {
                var entitiesPointPairs = _getEntitiesWithPoints(graph).Select(entity => (entity,_getPointFromEntity(entity))).ToList();
                entitiesPointPairs.ForEach(
                    (entityPointPair) => drawing.FillCircle(entityPointPair.Item2, _radius, _getColorFromEntity(entityPointPair.Item1)));
            }
            
            _onImageProduced(drawing);

            return graph;
        }
        
        private static Vector2 PointFromEntityToVector2(Entity entity)
        {
            var (x,y) = ComponentUtility.GetPosition2DFromComponent(entity);
            return new Vector2(x,y);
        }
        
    }
}