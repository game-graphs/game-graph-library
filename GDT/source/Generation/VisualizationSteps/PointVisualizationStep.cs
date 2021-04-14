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

        private readonly Func<Graph, List<Entity>> _getNodesWithPoints = graph => graph.Entities;
        private readonly Func<Entity, Vector2> _getPointFromNode = PointFromNodeToVector2;
        private readonly Action<ImageDrawing> _onImageProduced;
        private readonly int _width;
        private readonly int _height;
        private readonly float _radius;
        
        private readonly Color _pointColor;
        private readonly Func<Entity, Color>? _getColorFromNode = null;
        private readonly Color _backgroundColor = Color.Transparent;

        public PointVisualizationStep( 
            int width, 
            int height, 
            Color pointColor, 
            float radius,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>>? getNodesWithPoints = null, 
            Func<Entity, Vector2>? getPointFromNode = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _pointColor = pointColor;
            _radius = radius;
            _onImageProduced = onImageProduced;
            _getNodesWithPoints = getNodesWithPoints ?? _getNodesWithPoints;
            _getPointFromNode = getPointFromNode ?? _getPointFromNode;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }
        
        public PointVisualizationStep( 
            int width, 
            int height, 
            Func<Entity, Color> getColorFromNode, 
            float radius,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>>? getNodesWithPoints = null, 
            Func<Entity, Vector2>? getPointFromNode = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _getColorFromNode = getColorFromNode;
            _radius = radius;
            _onImageProduced = onImageProduced;
            _getNodesWithPoints = getNodesWithPoints ?? _getNodesWithPoints;
            _getPointFromNode = getPointFromNode ?? _getPointFromNode;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }

        public Graph ExecuteStep(Graph graph)
        {
            ImageDrawing drawing = new (_width, _height);
            drawing.Clear(_backgroundColor);

            if (_getColorFromNode == null)
            {
                var points = _getNodesWithPoints(graph).Select(node => _getPointFromNode(node)).ToList();
                drawing.FillCircles(points, _radius, _pointColor);
            }
            else
            {
                var nodePointPairs = _getNodesWithPoints(graph).Select(node => (node,_getPointFromNode(node))).ToList();
                nodePointPairs.ForEach(
                    (nodePointPair) => drawing.FillCircle(nodePointPair.Item2, _radius, _getColorFromNode(nodePointPair.Item1)));
            }
            
            _onImageProduced(drawing);

            return graph;
        }
        
        private static Vector2 PointFromNodeToVector2(Entity entity)
        {
            var (x,y) = ComponentUtility.GetPosition2DFromComponent(entity);
            return new Vector2(x,y);
        }
        
    }
}