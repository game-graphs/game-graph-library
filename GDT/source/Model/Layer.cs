using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GDT.Model
{
    public class Layer
    {
        public Guid ID { get; init; }
        public string Name { get; set; }

        public List<Relation> Relations;

        private WeakReference<Graph> _graph;
        
        public Layer(Guid id, string name, List<Relation> relations)
        {
            ID = id;
            Name = name;
            Relations = relations;
            _graph = new WeakReference<Graph>(null);
        }

        public Layer(string name) : this(Guid.NewGuid(), name, new List<Relation>()) {}

        public bool SetGraph(Graph graph) 
        {
            Graph g;
            if(_graph.TryGetTarget(out g)) return false;

            _graph = new WeakReference<Graph>(graph);
            return true;
        }

        public void AddRelation(Entity from, Entity to)
        {
            Relations.Add(new Relation(from, to, this));
        }

        public void AddRelation(Entity from, Entity to, string name)
        {
            Relations.Add(new Relation(from, to, name, this));
        }

        public Relation GetRelation(string name)
        {
            return Relations.Find(relation => relation.Name.Equals(name));
        }
        
        public Relation GetRelation(Guid guid)
        {
            return Relations.Find(relation => relation.ID.Equals(guid));
        }

        public List<Relation> GetRelationsContainingNode(Entity entity)
        {
            return Relations.FindAll(relation => relation.Nodes.Contains(entity));
        }

        public Layer Clone(Graph graph)
        {
            Layer layer = new Layer(ID, Name, new List<Relation>());

            foreach (var relation in Relations)
            {
                layer.Relations.Add(relation.Copy(graph, layer));
            }

            return layer;
        }

        public IEnumerable<Entity> GetNodesInLayer()
        {
            HashSet<Guid> seenNodes = new HashSet<Guid>();

            foreach (var relation in Relations)
            {
                foreach (var node in relation.Nodes)
                {
                    if (!seenNodes.Contains(node.ID))
                    {
                        seenNodes.Add(node.ID);
                        yield return node;
                    }
                }
            }
        }

        public int CountNeighbors(Entity entity)
        {
            return Relations.Count(relation => relation.Nodes.Contains(entity));
        }

        public int Distance(Entity from, Entity to, List<Entity> blockedNodes)
        {
            if (from == to) return 0;
            if (blockedNodes == null) blockedNodes = new();
            if (blockedNodes.Contains(to)) throw new ArgumentException("Cannot block 'target' node");

            Dictionary<Entity, int> distanceMap = new Dictionary<Entity, int> { {@from, 0} };
            Stack<Relation> border = new Stack<Relation>(GetRelationsContainingNode(from));

            while (border.Any())
            {
                Relation currentRelation = border.Pop();

                int distanceNode0 = distanceMap.GetValueOrDefault(currentRelation.Nodes[0], -1);
                int distanceNode1 = distanceMap.GetValueOrDefault(currentRelation.Nodes[1], -1);

                if (distanceNode0 == -1 || distanceNode1 == -1)
                {
                    if (distanceNode0 > distanceNode1)
                    {
                        distanceMap.Add(currentRelation.Nodes[1], distanceNode0 + 1);
                    }
                    else
                    {
                        distanceMap.Add(currentRelation.Nodes[0], distanceNode0 + 1);
                    }
                }

                if (distanceMap.ContainsKey(to)) break;
            }

            return distanceMap.GetValueOrDefault(to, -1);
        }
    }
}