using System.Collections.Generic;
using System.Linq;
using GDT.Model;
using SharpYaml.Serialization;

namespace GDT.IO.Implementation
{
    sealed class YamlStorage
    {
        public static string SaveGraph(Graph graph, bool emitAlias = false)
        {
            var settings = new SerializerSettings {EmitAlias = emitAlias}; 
            var serializer = new Serializer(settings);
            GraphDocument graphDocument = MakeGraphDocument(graph);
            return serializer.Serialize(graphDocument);
        }

        private static GraphDocument MakeGraphDocument(Graph graph)
        {
            GraphDocument graphDocument = new()
            {
                id = graph.ID,
                name = graph.Name,
                layers = MakeLayerDocuments(graph.GetLayers()),
                entities = MakeEntityDocuments(graph.Entities)
            };

            return graphDocument;
        }

        private static List<EntityDocument> MakeEntityDocuments(IEnumerable<Entity> entities)
        {
            List<EntityDocument> entityDocuments = new List<EntityDocument>();

            foreach (var entity in entities)
            {
                entityDocuments.Add(MakeEntityDocument(entity));
            }

            return entityDocuments;
        }

        private static EntityDocument MakeEntityDocument(Entity entity)
        {
            EntityDocument entityDocument = new EntityDocument()
            {
                id = entity.ID,
                name = entity.Name,
                children = MakeEntityDocuments(entity.Children),
                components = MakeComponentDocuments(entity.Components)
            };

            return entityDocument;
        }

        private static List<ComponentDocument> MakeComponentDocuments(IEnumerable<EntityComponent> entityComponents)
        {
            List<ComponentDocument> componentDocuments = new List<ComponentDocument>();

            foreach (var component in entityComponents)
            {
                componentDocuments.Add(MakeComponentDocument(component));
            }
            
            return componentDocuments;
        }

        private static ComponentDocument MakeComponentDocument(EntityComponent entityComponent)
        {
            ComponentDocument componentDocument = new ComponentDocument()
            {
                id = entityComponent.ID,
                name = entityComponent.Name,
                attributes = MakeAttributeDocuments(entityComponent.Properties.Values)
            };

            return componentDocument;
        }

        private static List<PropertyDocument> MakeAttributeDocuments(IEnumerable<Property> componentAttributes)
        {
            List<PropertyDocument> attributeDocuments = new List<PropertyDocument>();

            foreach (var componentAttribute in componentAttributes)
            {
                attributeDocuments.Add(MakeAttributeDocument(componentAttribute));
            }
            
            return attributeDocuments;
        }

        private static PropertyDocument MakeAttributeDocument(Property property)
        {
            PropertyDocument propertyDocument = new PropertyDocument()
            {
                name = property.Name,
                value = property.Value,
                type_name = property.Type.FullName,
                type_assembly = property.Type.Assembly.GetName().Name
            };
            
            return propertyDocument;
        }
        
        
        private static List<LayerDocument> MakeLayerDocuments(IEnumerable<Layer> layers)
        {
            List<LayerDocument> layerDocuments = new List<LayerDocument>();
            
            foreach (var layer in layers)
            {
                layerDocuments.Add(MakeLayerDocument(layer));
            }
            
            return layerDocuments;
        }

        private static LayerDocument MakeLayerDocument(Layer layer)
        {
            LayerDocument layerDocument = new LayerDocument()
            {
                id = layer.ID,
                name = layer.Name,
                relations = MakeRelationDocuments(layer)
            };

            return layerDocument;
        }

        private static List<RelationDocument> MakeRelationDocuments(Layer layer)
        {
            List<RelationDocument> relationDocuments = new List<RelationDocument>();

            foreach (var relation in layer.Relations)
            {
                relationDocuments.Add(MakeRelationDocument(relation));
            }

            return relationDocuments;
        }

        private static RelationDocument MakeRelationDocument(Relation relation)
        {
            var referencedEntities = relation.Entities.ConvertAll(entity => entity.ID);

            RelationDocument relationDocument = new RelationDocument()
            {
                id = relation.ID,
                name = relation.Name,
                referenced_entities = referencedEntities,
                properties = new List<PropertyDocument>(relation.Properties.Values.Select(MakeAttributeDocument)),
            };
            return relationDocument;
        }
        
    }
}