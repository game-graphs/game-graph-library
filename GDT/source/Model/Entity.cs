using System;
using System.Collections.Generic;

namespace GDT.Model
{
    #nullable enable
    public class Entity
    {
        public Guid ID { get;}
        public string Name { get; set; }
        public List<EntityComponent> Components;

        public List<Entity> Children { get; }
        public WeakReference<Entity?> Parent { get; private set; }
        public WeakReference<Graph?> Graph { get; }

        public Entity(Guid id, string name, List<EntityComponent> components, List<Entity> children, Entity? parent, Graph? graph)
        {
            ID = id;
            Name = name;
            Components = components;
            Children = children;
            
            Parent = new WeakReference<Entity?>(parent);
            Graph = new WeakReference<Graph?>(graph);
        }

        public Entity(Guid id, string name, Graph? graph)
            : this(id, name, new List<EntityComponent>(), new List<Entity>(), null, graph)
        {}

        public Entity(string name, Graph? graph)
            : this(Guid.NewGuid(), name, graph)
        {}

        public void AddChild(Entity child)
        {
            Children.Add(child);
            child.Parent = new WeakReference<Entity?>(this);
        }

        public void RemoveFromParent()
        {
            Entity? parentNode;
            if (Parent.TryGetTarget(out parentNode))
            {
                parentNode?.Children.Remove(this);
                Parent = new WeakReference<Entity?>(null);
            }
        }

        public void AddChildren(IEnumerable<Entity> children)
        {
            foreach(var child in children)
            {
                AddChild(child);
            }
        }
        
        public EntityComponent? GetComponent(string name)
        {
            return Components.Find(component => component.Name.Equals(name));
        }

        public bool RemoveComponent(string name)
        {
            var component = GetComponent(name);
            if (component == null) throw new ArgumentException($"Component '{name}' not found", nameof(name));
            
            return Components.Remove(component);
        }

        public Entity? GetChild(string name, bool recursive = true)
        {
            return GetChild(node => node.Name.Equals(name), recursive);
        }

        public Entity? GetChild(Guid id, bool recursive = true)
        {
            return GetChild(node => node.ID.Equals(id), recursive);
        }
        
        public Entity? GetChild(Predicate<Entity> predicate, bool recursive = true)
        {
            foreach (var child in Children)
            {
                if (predicate.Invoke(child)) return child;
                
                if (recursive)
                {
                    Entity? matchedNode = child.GetChild(predicate, recursive);
                    if (matchedNode != null)
                    {
                        return matchedNode;
                    }
                }
            }

            return null;
        }

        public Entity Clone(Graph graph, Entity? parent)
        {
            Entity newEntity = new(ID, Name, new List<EntityComponent>(), new List<Entity>(), parent, graph);
            foreach (var component in Components)
            {
                newEntity.Components.Add(component.Clone());
            }

            foreach (var child in Children)
            {
                newEntity.Children.Add(child.Clone(graph, newEntity));
            }
            
            return newEntity;
        }

        public Entity CreateDuplicate(Graph graph, Entity? parent)
        {
            Entity duplicateEntity = new(Guid.NewGuid(), Name, new List<EntityComponent>(), new List<Entity>(), parent, graph);
            
            foreach (var component in Components)
            {
                duplicateEntity.Components.Add(component.CreateDuplicate());
            }

            foreach (var child in Children)
            {
                duplicateEntity.Children.Add(child.CreateDuplicate(graph, duplicateEntity));
            }
            
            return duplicateEntity;
        }
    }
}