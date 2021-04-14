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

            InitializeNodes(availableModules);
        }

        protected override IEnumerable<WfcCell<WfcGraphTile<TTileData>, int>> GetCells()
        {
            return GetWfcNodes().Select(node =>
                {
                    WfcCell<WfcGraphTile<TTileData>, int>? cell = GetWfcCellFromNode(node);
                    
                    if (cell == null) throw new NullReferenceException("WfcCell not properly initialized!");
                    
                    return cell;
                }
            );
        }
        
        private WfcCell<WfcGraphTile<TTileData>, int>? GetWfcCellFromNode(Entity entity)
        {
            EntityComponent? component = entity.GetComponent(_wfcComponentName);
            return component?.Get<WfcCell<WfcGraphTile<TTileData>, int>>(_wfcCellPropertyName);
        }
        
        private WfcCell<WfcGraphTile<TTileData>, int> GetWfcCellFromNodeOrThrow(Entity entity)
        {
            EntityComponent? component = entity.GetComponent(_wfcComponentName);
            var cell = component?.Get<WfcCell<WfcGraphTile<TTileData>, int>>(_wfcCellPropertyName);
            if (cell == null) throw new NullReferenceException("WfcCell not properly initialized!");
            return cell;
        }

        public override WfcCell<WfcGraphTile<TTileData>, int>? GetCell(int position)
        {
            return GetWfcCellFromNode(_graph.Entities[position]);
        }

        public override List<WfcCell<WfcGraphTile<TTileData>, int>> GetNeighbors(WfcCell<WfcGraphTile<TTileData>, int> cell)
        {
            var layer = _graph.GetLayer(_wfcNeighborLayerName);
            if (layer == null) throw new ArgumentException($"Data Structure Graph is missing the neighbor layer '{_wfcNeighborLayerName}'");

            var relationsContainingNode = layer.GetRelationsContainingNode(GetWfcNodes().ToList()[cell.Position]);

            return relationsContainingNode.SelectMany(relation =>
            {
                var neighbors = new List<WfcCell<WfcGraphTile<TTileData>, int>>();

                neighbors.AddRange(relation.Nodes
                    .Where(node => GetWfcCellFromNode(node) != cell)
                    .SelectMany(node => new List<WfcCell<WfcGraphTile<TTileData>, int>>() { GetWfcCellFromNode(node) })
                );
                
                return neighbors;
            }).ToList();
        }

        private void InitializeNodes(List<WfcGraphTile<TTileData>> availableModules)
        {
            int i = 0;
            foreach (Entity node in GetWfcNodes())
            {
                EntityComponent wfcComponent = new(_wfcComponentName);
                wfcComponent.SetProperty(_wfcCellPropertyName, new WfcCell<WfcGraphTile<TTileData>, int>(i, availableModules));
                node.Components.Add(wfcComponent);
                i++;
            }
        }

        private IEnumerable<Entity> GetWfcNodes()
        {
            Layer? wfcLayer = _graph.GetLayer(_wfcNeighborLayerName);
            if (wfcLayer == null) throw new ArgumentException($"Data Structure Graph is missing the neighbor layer '{_wfcNeighborLayerName}'");

            return wfcLayer.GetNodesInLayer();
        }

        private void RemoveWfcComponents()
        {
            foreach (var targetNode in GetWfcNodes())
            {
                targetNode.RemoveComponent(_wfcComponentName);
            }
        }
        
        // This method moves the data of the selected WfcNode to the node of the graph
        public void PropagateToGraphAndCleanup()
        {
            foreach (var targetNode in GetWfcNodes())
            {
                WfcCell<WfcGraphTile<TTileData>, int> cell = GetWfcCellFromNodeOrThrow(targetNode);
                var selectedTemplateTile = cell.AvailableModules[0];
                InstantiateTemplate(selectedTemplateTile.Graph, targetNode);
            }

            RemoveWfcComponents();
        }

        // Creates an instance of a graph, currently assumes no child nodes
        private void InstantiateTemplate(Model.Graph templateGraph, Entity targetEntity)
        {
            Dictionary<Entity, Entity> templateToInstanceMapping = new();
            
            // instantiate all nodes from template and remember mapping
            foreach (var templateNode in templateGraph.Entities)
            {
                var templateInstance = templateNode.CreateDuplicate(_graph, targetEntity);
                templateToInstanceMapping.Add(templateNode, templateInstance);
                targetEntity.AddChild(templateInstance);
            }

            // add internal relations between instantiated nodes
            foreach (var templateLayer in templateGraph.GetLayers())
            {
                Layer layer = _graph.GetOrAddLayer(templateLayer.Name);

                foreach (var templateRelation in templateLayer.Relations)
                {
                    var from = templateToInstanceMapping[templateRelation.Nodes[0]];
                    var to = templateToInstanceMapping[templateRelation.Nodes[1]];
                    layer.AddRelation(from, to, templateRelation.Name);
                }
            }
        }
    }
}