using System;
using System.Collections.Generic;
using System.Linq;
using GDT.Model;
using GDT.Algorithm;

#nullable enable

namespace GDT.Algorithm.WFC.Implementation.Graph
{
    public class WfcGraphSpace<TTileData> : WfcSpace<WfcGraphTile<TTileData>, int>
                                where TTileData: struct
    {
        private readonly string _wfcComponentName;
        private readonly string _wfcCellPropertyName;
        private readonly string _wfcNeighborLayerName;
        
        
        private Model.Graph _graph;

        public WfcGraphSpace(Model.Graph graph, List<WfcGraphTile<TTileData>> availableModules, Blackboard blackboard, 
            string wfcComponentName = "WfcComponent", 
            string wfcCellPropertyName = "WfcCell", 
            string wfcNeighborLayerName = "WfcNeighborLayer")
            : base(blackboard)
        {
            _graph = graph;
            _wfcComponentName = wfcComponentName;
            _wfcCellPropertyName = wfcCellPropertyName;
            _wfcNeighborLayerName = wfcNeighborLayerName;

            InitializeEntities(availableModules);
        }

        protected override IEnumerable<WfcCell<WfcGraphTile<TTileData>, int>> GetCells()
        {
            return GetWfcEntities().Select(entity =>
                {
                    WfcCell<WfcGraphTile<TTileData>, int>? cell = GetWfcCellFromEntity(entity);
                    
                    if (cell == null) throw new NullReferenceException("WfcCell not properly initialized!");
                    
                    return cell;
                }
            );
        }
        
        private WfcCell<WfcGraphTile<TTileData>, int>? GetWfcCellFromEntity(Entity entity)
        {
            EntityComponent? component = entity.GetComponent(_wfcComponentName);
            return component?.Get<WfcCell<WfcGraphTile<TTileData>, int>>(_wfcCellPropertyName);
        }
        
        private WfcCell<WfcGraphTile<TTileData>, int> GetWfcCellFromEntityOrThrow(Entity entity)
        {
            EntityComponent? component = entity.GetComponent(_wfcComponentName);
            var cell = component?.Get<WfcCell<WfcGraphTile<TTileData>, int>>(_wfcCellPropertyName);
            if (cell == null) throw new NullReferenceException("WfcCell not properly initialized!");
            return cell;
        }

        public override WfcCell<WfcGraphTile<TTileData>, int>? GetCell(int position)
        {
            return GetWfcCellFromEntity(_graph.Entities[position]);
        }

        public override List<WfcCell<WfcGraphTile<TTileData>, int>> GetNeighbors(WfcCell<WfcGraphTile<TTileData>, int> cell)
        {
            var layer = _graph.GetLayer(_wfcNeighborLayerName);
            if (layer == null) throw new ArgumentException($"Data Structure Graph is missing the neighbor layer '{_wfcNeighborLayerName}'");

            var relationsContainingEntity = layer.GetRelationsContainingEntity(GetWfcEntities().ToList()[cell.Position]);

            return relationsContainingEntity.SelectMany(relation =>
            {
                var neighbors = new List<WfcCell<WfcGraphTile<TTileData>, int>>();

                neighbors.AddRange(relation.Entities
                    .Where(entity => GetWfcCellFromEntity(entity) != cell)
                    .SelectMany(entity => new List<WfcCell<WfcGraphTile<TTileData>, int>>() { GetWfcCellFromEntity(entity) })
                );
                
                return neighbors;
            }).ToList();
        }

        private void InitializeEntities(List<WfcGraphTile<TTileData>> availableModules)
        {
            int i = 0;
            foreach (Entity entity in GetWfcEntities())
            {
                EntityComponent wfcComponent = new(_wfcComponentName);
                wfcComponent.SetProperty(_wfcCellPropertyName, new WfcCell<WfcGraphTile<TTileData>, int>(i, availableModules));
                entity.Components.Add(wfcComponent);
                i++;
            }
        }

        private IEnumerable<Entity> GetWfcEntities()
        {
            Layer? wfcLayer = _graph.GetLayer(_wfcNeighborLayerName);
            if (wfcLayer == null) throw new ArgumentException($"Data Structure Graph is missing the neighbor layer '{_wfcNeighborLayerName}'");

            return wfcLayer.GetEntitiesInLayer();
        }

        private void RemoveWfcComponents()
        {
            foreach (var targetEntity in GetWfcEntities())
            {
                targetEntity.RemoveComponent(_wfcComponentName);
            }
        }
        
        // This method moves the data of the selected WfcNode to the node of the graph
        public void PropagateToGraphAndCleanup()
        {
            foreach (var targetEntity in GetWfcEntities())
            {
                WfcCell<WfcGraphTile<TTileData>, int> cell = GetWfcCellFromEntityOrThrow(targetEntity);
                var selectedTemplateTile = cell.AvailableModules[0];
                InstantiateTemplate(selectedTemplateTile.Graph, targetEntity);
            }

            RemoveWfcComponents();
        }

        // Creates an instance of a graph, currently assumes no child entities
        private void InstantiateTemplate(Model.Graph templateGraph, Entity targetEntity)
        {
            Dictionary<Entity, Entity> templateToInstanceMapping = new();
            
            // instantiate all entities from template and remember mapping
            foreach (var templateEntity in templateGraph.Entities)
            {
                var templateInstance = templateEntity.CreateDuplicate(_graph, targetEntity);
                templateToInstanceMapping.Add(templateEntity, templateInstance);
                targetEntity.AddChild(templateInstance);
            }

            // add internal relations between instantiated entities
            foreach (var templateLayer in templateGraph.GetLayers())
            {
                Layer layer = _graph.GetOrAddLayer(templateLayer.Name);

                foreach (var templateRelation in templateLayer.Relations)
                {
                    var from = templateToInstanceMapping[templateRelation.Entities[0]];
                    var to = templateToInstanceMapping[templateRelation.Entities[1]];
                    layer.AddRelation(from, to, templateRelation.Name);
                }
            }
        }
    }
}