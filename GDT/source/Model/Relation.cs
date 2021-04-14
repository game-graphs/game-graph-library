using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace GDT.Model
{
    public class Relation
    {
        public Guid ID { get; }
        public string Name { get; }
        
        public List<Entity> Nodes { get; }
        public WeakReference<Layer> Layer { get; }
        
        public Dictionary<string, Property> Properties { get; }

        public Relation(Guid id, string name, List<Entity> nodes, Layer layer, Dictionary<string, Property> properties)
        {
            ID = id;
            Name = name;
            Nodes = nodes;
            Layer = new WeakReference<Layer>(layer);
            Properties = properties;
        }

        public Relation(Entity from, Entity to, Layer layer) 
            : this(from, to, $"{from.Name} <-> {to.Name}", layer)
        {}

        public Relation(Entity from, Entity to, string name, Layer layer) 
            : this(Guid.NewGuid(), name, new List<Entity>() {from, to}, layer, new())
        {}

        public Entity? GetNode(string name)
        {
            return Nodes.Find(node => node.Name.Equals(name));
        }

        public Relation Copy(Graph graph, Layer layer)
        {
            Relation relation = new(ID, Name, new List<Entity>(), layer, new Dictionary<string, Property>());

            foreach (var node in Nodes)
            {
                var newNodeReference = graph.GetNode(node.ID);
                if (newNodeReference == null) throw new ArgumentException($"Copying failed because node id {node.ID} wasn't found!");
                relation.Nodes.Add(newNodeReference);
            }

            foreach (var property in Properties.Values)
            {
                relation.Properties.Add(property.Name, new Property(property.Name, property.Type, property.Value));
            }

            return relation;
        }

        public override string ToString()
        {
            return $"Relation ({Name}): Nodes: {Nodes.Select(node => node.Name).Aggregate((s0, s1) => s0 + " " + s1)}";
        }

        public Entity GetOtherNode(Entity currentEntity)
        {
            if (Nodes[0] == currentEntity) return Nodes[1];
            if (Nodes[1] == currentEntity) return Nodes[0];

            throw new NotSupportedException("Currently only supporting relations with two nodes.");
        }
    }
}