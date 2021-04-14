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

        private readonly Func<Graph, List<Entity>> _getNodesWithAreas;
        private readonly Func<Entity, List<PointF>> _getPointsFromNode = AreaFromNodeToVector2;
        private readonly Action<ImageDrawing> _onImageProduced;
        private readonly int _width;
        private readonly int _height;
        
        private readonly Color _areaColor;
        private readonly Func<Entity, Color>? _getColorFromNode = null;
        private readonly Color _backgroundColor = Color.Transparent;

        public AreaVisualizationStep( 
            int width, 
            int height, 
            Color areaColor,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>> getNodesWithAreas, 
            Func<Entity, List<PointF>>? getPointsFromNode = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _areaColor = areaColor;
            _onImageProduced = onImageProduced;
            _getNodesWithAreas = getNodesWithAreas;
            _getPointsFromNode = getPointsFromNode ?? _getPointsFromNode;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }
        
        public AreaVisualizationStep( 
            int width, 
            int height, 
            Func<Entity, Color> getColorFromNode,
            Action<ImageDrawing> onImageProduced,
            Func<Graph, List<Entity>> getNodesWithAreas, 
            Func<Entity, List<PointF>>? getPointsFromNode = null,
            Color? backgroundColor = null)
        {
            _width = width;
            _height = height;
            _getColorFromNode = getColorFromNode;
            _onImageProduced = onImageProduced;
            _getNodesWithAreas = getNodesWithAreas;
            _getPointsFromNode = getPointsFromNode ?? _getPointsFromNode;
            _backgroundColor = backgroundColor ?? _backgroundColor;
        }

        public Graph ExecuteStep(Graph graph)
        {
            ImageDrawing drawing = new (_width, _height);
            drawing.Clear(_backgroundColor);

            if (_getColorFromNode == null)
            {
                var areas = _getNodesWithAreas(graph).Select(node => _getPointsFromNode(node)).ToList();
                areas.ForEach(area => drawing.FillPoly(area, _areaColor));
            }
            else
            {
                var nodeAreaPairs = _getNodesWithAreas.Invoke(graph).Select(node => (node,_getPointsFromNode.Invoke(node))).ToList();
                nodeAreaPairs.ForEach(
                    (nodePointPair) => drawing.FillPoly(nodePointPair.Item2, _getColorFromNode(nodePointPair.Item1)));
            }
            
            _onImageProduced(drawing);

            return graph;
        }
        
        private static List<PointF> AreaFromNodeToVector2(Entity entity)
        {
            var areaComponent = entity.GetComponent("AreaComponent");
            var site = areaComponent?.Get<FortuneSite?>("cell");
            var points = site?.Points.Select(p => new PointF((float) p.X, (float) p.Y)).ToList() ?? new List<PointF>();
            return points;
        }
        
    }
}