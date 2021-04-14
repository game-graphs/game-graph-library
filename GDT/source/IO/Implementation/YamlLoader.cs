using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SharpYaml.Serialization;

using GDT.Model;

#nullable enable

namespace GDT.IO.Implementation
{
    sealed class YamlLoader
    {
        public static Graph LoadGraph(string content)
        {
            var serializer = new Serializer();
            GraphDocument graphDocument = serializer.Deserialize<GraphDocument>(content);
            return ParseGraph(graphDocument);
        }

        private static Graph ParseGraph(GraphDocument graphDocument)
        {
            Graph graph = new Graph(graphDocument.id, graphDocument.name, new List<Entity>(), new List<Layer>());
            graphDocument.entities.ForEach(node => graph.Entities.Add(ParseNode(node, graph)));
            graphDocument.layers.ForEach(layer => graph.AddLayer(ParseLayer(layer, graph)));
            
            return graph;
        }

        private static Entity ParseNode(EntityDocument entityDocument, Graph graph)
        {
            Entity entity = new Entity(entityDocument.id, entityDocument.name, graph);

            if (entityDocument.components != null)
            {
                foreach (var component in entityDocument.components)
                {
                    entity.Components.Add(ParseComponent(component));
                }
            }
            
            entityDocument.children?.ForEach(childNodeDoc => entity.AddChild(ParseNode(childNodeDoc, graph)));

            return entity;
        }

        private static EntityComponent ParseComponent(ComponentDocument componentDocument)
        {
            EntityComponent component = new EntityComponent(componentDocument.id, 
                                                        componentDocument.name,
                                                        new Dictionary<string, Property>());
            if (componentDocument.attributes != null)
            {
                foreach (PropertyDocument propertyDocument in componentDocument.attributes)
                {
                    Property property = ParseProperty(propertyDocument);
                
                    component.SetProperty(property.Name, property.Value);
                }   
            }

            return component;
        }

        private static Property ParseProperty(PropertyDocument propertyDocument)
        {
            string typeString = propertyDocument.type_name;
            if (propertyDocument.type_assembly != null && !"".Equals(propertyDocument.type_assembly))
                typeString = $"{propertyDocument.type_name}, {propertyDocument.type_assembly}";
            // TODO future Make YAML parser do the conversion. Otherwise only string to class conversions are supported which is insufficient for complex types
            
            //Type? type = Type.GetType(typeString);
            //if(type == null) throw new ArgumentException($"Parsed type {propertyDocument.type_name} cannot be converted to System.Type!");
                
            //var converter = TypeDescriptor.GetConverter(type);
            //var result = converter.ConvertFrom(propertyDocument.value);
                
            return new Property(propertyDocument.name, propertyDocument.value);
        }

        private static Layer ParseLayer(LayerDocument layerDocument, Graph graph)
        {
            Layer layer = new Layer(layerDocument.id, layerDocument.name, new List<Relation>());
            
            layerDocument.relations.ForEach(relation => layer.Relations.Add(ParseRelation(relation, layer, graph)));
            
            return layer;
        }

        private static Relation ParseRelation(RelationDocument relationDocument, Layer layer, Graph graph)
        {
            Relation relation = new (relationDocument.id, 
                                     relationDocument.name, 
                                     new List<Entity>(), 
                                     layer,
                                     new ());

            if (relationDocument.referenced_entities != null)
            {
                foreach (Guid refNodeGuid in relationDocument.referenced_entities)
                {
                    Entity? refNode = graph.GetNode(refNodeGuid);
                    if (refNode != null)
                    {
                        relation.Nodes.Add(refNode);
                    }
                    else
                    {
                        Console.Error.WriteLine($"Could not find node for guid: {refNodeGuid}");
                    }
                }
            }

            if (relationDocument.properties != null)
            {
                foreach (var propertyDocument in relationDocument.properties)
                {
                    var property = ParseProperty(propertyDocument);
                    relation.Properties.Add(property.Name, property);
                }
            }

            return relation;
        }
    }
}