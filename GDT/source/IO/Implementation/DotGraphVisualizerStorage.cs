using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDT.Model;

namespace GDT.IO.Implementation
{
    public class DotGraphVisualizerStorage : IGraphVisualizerStorage
    {
        public static string SaveGraph(Graph graph)
        {
            return ConvertToDotModel(graph);
        }
        
        public static string SaveLayerGraph(Layer layer)
        {
            return ConvertLayerToDotModel(layer);
        }
        
        private static string ConvertLayerToDotModel(Layer layer)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            AddGraphToDotModel(stringBuilder, layer);

            return stringBuilder.ToString();
        }
        
        private static void AddGraphToDotModel(in StringBuilder stringBuilder, in Layer layer)
        {
            stringBuilder.AppendLine($"graph \"{layer.Name}\" {{");
            var indent = 1;
            stringBuilder.AppendLine($"{MakeIndent(indent)}graph [fontsize=10 fontname=\"Verdana\" compound=true];");
            stringBuilder.AppendLine($"{MakeIndent(indent)}node [shape=record fontsize=10 fontname=\"Verdana\"];");
            stringBuilder.AppendLine();

            AddSingleLayer(stringBuilder, layer, 1);

            stringBuilder.AppendLine("}");
        }

        private static string ConvertToDotModel(Graph graph)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            AddGraphToDotModel(stringBuilder, graph);

            return stringBuilder.ToString();
        }

        private static void AddGraphToDotModel(in StringBuilder stringBuilder, in Graph graph)
        {
            stringBuilder.AppendLine($"graph \"{graph.Name}\" {{");
            var indent = 1;
            stringBuilder.AppendLine($"{MakeIndent(indent)}graph [fontsize=10 fontname=\"Verdana\" compound=true];");
            stringBuilder.AppendLine($"{MakeIndent(indent)}node [shape=record fontsize=10 fontname=\"Verdana\"];");

            AddEntities(stringBuilder, graph.Entities, indent);
            
            stringBuilder.AppendLine();

            foreach (var layer in graph.GetLayers())
            {
                AddLayer(stringBuilder, layer, 1);
            }

            stringBuilder.AppendLine("}");
        }

        private static void AddEntities(in StringBuilder stringBuilder, in IEnumerable<Entity> entities, int indent)
        {
            foreach (var entity in entities)
            {
                if (entity.Children.Count > 0)
                {
                    AddEntitiesAsSubgraph(stringBuilder, entity, indent);
                }
                else
                {
                    stringBuilder.AppendLine($"{MakeIndent(indent)}\"{entity.ID}\" [label=\"{entity.Name}\"];");
                }
            }
        }
        
        private static void AddEntitiesAsSubgraph(in StringBuilder stringBuilder, in Entity entity, int indent)
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{MakeIndent(indent)}subgraph cluster_{entity.ID:N} {{");
            indent++;
            stringBuilder.AppendLine($"{MakeIndent(indent)}node [style=filled];");
            stringBuilder.AppendLine($"{MakeIndent(indent)}\"{entity.ID}\" [shape=point style=invis];");
            stringBuilder.AppendLine($"{MakeIndent(indent)}label = \"{entity.Name}\";");
            var pos = MakePositionOrEmpty(entity);
            if(pos.Length != 0) stringBuilder.AppendLine($"{MakeIndent(indent)}{pos};");
            

            foreach (var child in entity.Children)
            {
                if (child.Children.Count > 0)
                {
                    indent++;
                    AddEntitiesAsSubgraph(stringBuilder, child, indent);
                    indent--;
                }
                else
                {
                    stringBuilder.AppendLine($"{MakeIndent(indent)}\"{child.ID}\" [label=\"{child.Name}\" {MakePositionOrEmpty(child)}];");
                }
            }
            
            indent--;
            stringBuilder.AppendLine($"{MakeIndent(indent)}}}");
        }
        
        private static void AddLayer(in StringBuilder stringBuilder, in Layer layer, int indent)
        {
            foreach (var relation in layer.Relations)
            {
                string entityConnections = relation.Entities.Select(entity => entity.ID.ToString())
                    .Aggregate((s1, s2) => $"\"{s1}\" -- \"{s2}\"");

                string clusterConnection = "";
                if (relation.Entities[0].Children.Any())
                {
                    clusterConnection = $" [ltail=cluster_{relation.Entities[0].ID:N}, lhead=cluster_{relation.Entities[1].ID:N}]";
                }
                stringBuilder.AppendLine($"{MakeIndent(indent)}{entityConnections} {clusterConnection};");
            }
            
        }
        
        private static void AddSingleLayer(in StringBuilder stringBuilder, in Layer layer, int indent)
        {
            foreach (var entity in layer.GetEntitiesInLayer())
            {
                stringBuilder.AppendLine($"{MakeIndent(indent)}\"{entity.ID}\" [label=\"{entity.Name}\" {MakePositionOrEmpty(entity)}]");
            }

            stringBuilder.AppendLine();

            foreach (var relation in layer.Relations)
            {
                string entitiesConnections = relation.Entities.Select(entity => entity.ID.ToString())
                    .Aggregate((s1, s2) => $"\"{s1}\" -- \"{s2}\"");
                
                stringBuilder.AppendLine($"{MakeIndent(indent)}{entitiesConnections};");
            }
            
        }
        
        private static string MakeIndent(int indent)
        {
            return Enumerable.Repeat("\t", indent).Aggregate((c1, c2) => $"{c1}{c2}").ToString();
        }

        private static string MakePositionOrEmpty(Entity entity)
        {
            var positionComponent = entity.GetComponent("PositionComponent");
            if (positionComponent == null) return "";

            var x = positionComponent.Get<int>("x");
            var y = positionComponent.Get<int>("y");

            return $"pos = \"{x},{y}!\"";
        }
    }
}