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
        
        public List<Entity> Entities { get; }
        public WeakReference<Layer> Layer { get; }
        
        public Dictionary<string, Property> Properties { get; }

        public Relation(Guid id, string name, List<Entity> entities, Layer layer, Dictionary<string, Property> properties)
        {
            ID = id;
            Name = name;
            Entities = entities;
            Layer = new WeakReference<Layer>(layer);
            Properties = properties;
        }

        public Relation(Entity from, Entity to, Layer layer) 
            : this(from, to, $"{from.Name} <-> {to.Name}", layer)
        {}

        public Relation(Entity from, Entity to, string name, Layer layer) 
            : this(Guid.NewGuid(), name, new List<Entity>() {from, to}, layer, new())
        {}

        public Entity? GetEntity(string name)
        {
            return Entities.Find(entity => entity.Name.Equals(name));
        }

        public Relation Copy(Graph graph, Layer layer)
        {
            Relation relation = new(ID, Name, new List<Entity>(), layer, new Dictionary<string, Property>());

            foreach (var entity in Entities)
            {
                var newNodeReference = graph.GetEntity(entity.ID);
                if (newNodeReference == null) throw new ArgumentException($"Copying failed because node id {entity.ID} wasn't found!");
                relation.Entities.Add(newNodeReference);
            }

            foreach (var property in Properties.Values)
            {
                relation.Properties.Add(property.Name, new Property(property.Name, property.Type, property.Value));
            }

            return relation;
        }

        public override string ToString()
        {
            return $"Relation ({Name}): Entities: {Entities.Select(node => node.Name).Aggregate((s0, s1) => s0 + " " + s1)}";
        }

        public Entity GetOtherEntity(Entity currentEntity)
        {
            if (Entities[0] == currentEntity) return Entities[1];
            if (Entities[1] == currentEntity) return Entities[0];

            throw new NotSupportedException("Currently only supporting relations with two entities.");
        }
    }
}