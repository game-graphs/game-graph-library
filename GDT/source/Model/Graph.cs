using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable enable

namespace GDT.Model
{
    public class Graph
    {
        public Guid ID { get; }
        public string Name { get; }

        public List<Entity> Entities;

        private List<Layer> Layers { get;}

        public ReadOnlyCollection<Layer> GetLayers()
        {
            return Layers.AsReadOnly();
        }

        public Graph(Guid id, string name, List<Entity> entities, List<Layer> layers)
        {
            ID = id;
            Name = name;
            Entities = entities;
            Layers = layers;
        }

        public Graph(string name) : this(Guid.NewGuid(), name, new List<Entity>(), new List<Layer>()) {}

        public static Graph CreatePlaceholder(string name)
        {
            return new(Guid.Empty, name, new List<Entity>(), new List<Layer>());
        }

        public void AddLayer(Layer layer)
        {
            if(!layer.SetGraph(this)) {
                throw new ArgumentException("Layer already assigned to graph!");
            }

            Layers.Add(layer);
        }

        public Entity? GetNode(string name)
        {
            return GetNode(node => node.Name.Equals(name));
        }
        
        public Entity? GetNode(Guid id)
        {
            return GetNode(node => node.ID.Equals(id));
        }
        
        public Entity? GetNode(Predicate<Entity> predicate)
        {
            foreach (var node in Entities)
            {
                if (predicate.Invoke(node)) return node;

                Entity? childMatch = node.GetChild(predicate);
                if (childMatch != null) return childMatch;
            }
            
            return null;
        }

        public Layer? GetLayer(string name)
        {
            return Layers.Find(layer => layer.Name.Equals(name));
        }
        
        public Layer GetOrAddLayer(string name)
        {
            Layer? layer = GetLayer(name);
            if (layer != null) return layer;

            Layer newLayer = new(name);
            AddLayer(newLayer);
            return newLayer;
        }

        public Graph Clone()
        {
            Graph graph = new(ID, Name, new List<Entity>(), new List<Layer>());
            foreach (var node in Entities)
            {
                graph.Entities.Add(node.Clone(graph, null));
            }
            
            foreach (var layer in Layers)
            {
                graph.Layers.Add(layer.Clone(graph));
            }

            return graph;
        }
    }
}