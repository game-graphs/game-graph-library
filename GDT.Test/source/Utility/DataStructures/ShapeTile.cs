using System;
using System.Collections.Generic;
using GDT.Model;

namespace GDT.Utility.Visualization.DataStructures
{

    public abstract class ShapeTile
    {
        private readonly int _minNeighbors;
        private readonly int _maxNeighbors;
        protected static readonly Random Random = new Random();

        protected ShapeTile(int minNeighbors, int maxNeighbors)
        {
            _minNeighbors = minNeighbors;
            _maxNeighbors = maxNeighbors;
        }

        public bool CheckNeighborCount(int neighbors)
        {
            return neighbors <= _maxNeighbors && neighbors >= _minNeighbors;
        }

        public abstract void AddToGraph(Graph graph, Layer layer, Entity parent);
    }
    
    public class CrossingXTile : ShapeTile 
    {
        public CrossingXTile() : base(4, 4)
        {
        }

        public override void AddToGraph(Graph graph, Layer layer, Entity parent)
        {
            var n1 = new Entity("CrossingX Area 1", graph);
            var n2 = new Entity("CrossingX Area 2", graph);
            var n3 = new Entity("CrossingX Area 3", graph);
            var n4 = new Entity("CrossingX Area 4", graph);
            var nCenter = new Entity("CrossingX Area Center", graph);
            
            parent.Children.AddRange(new List<Entity>() { n1, n2, n3, n4, nCenter });
            
            layer.AddRelation(n1, nCenter, "1 to Center");
            layer.AddRelation(n2, nCenter, "2 to Center");
            layer.AddRelation(n3, nCenter, "3 to Center");
            layer.AddRelation(n4, nCenter, "4 to Center");
        }
    }
    
    public class CrossingTTile : ShapeTile 
    {
        public CrossingTTile() : base(3, 3)
        {
        }

        public override void AddToGraph(Graph graph, Layer layer, Entity parent)
        {
            var n1 = new Entity("CrossingT Area 1", graph);
            var n2 = new Entity("CrossingT Area 2", graph);
            var n3 = new Entity("CrossingT Area 3", graph);
            var nCenter = new Entity("CrossingT Area Center", graph);
            
            parent.Children.AddRange(new List<Entity>() { n1, n2, n3, nCenter });
            
            layer.AddRelation(n1, nCenter, "1 to Center");
            layer.AddRelation(n2, nCenter, "2 to Center");
            layer.AddRelation(n3, nCenter, "3 to Center");
        }
    }
    
    public class ChainTile : ShapeTile
    {
        private readonly int _chainLength; 
        
        public ChainTile(int chainLength = -1) : base(1, 2)
        {
            _chainLength = (chainLength == -1) ? Random.Next(2, 4) : chainLength;
           
        }

        public override void AddToGraph(Graph graph, Layer layer, Entity parent)
        {
            var previousNode = new Entity("Chain Area 1", graph);
            parent.Children.Add(previousNode);
            
            for (int i = 2; i <= _chainLength; i++)
            {
                var currentNode = new Entity($"Chain Area {i}", graph);
                parent.Children.Add(currentNode);
                layer.AddRelation(previousNode, currentNode, $"Chain {i-1} to {i}");
                
                previousNode = currentNode;
            }

        }
    }
}