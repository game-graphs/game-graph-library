using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using GDT.Model;
using GDT.Algorithm;

namespace GDT.Algorithm.WFC.Implementation.Graph
{
    public class WfcGraphTile<TTileData> : IWfcModule<WfcGraphTile<TTileData>, int>
                                where TTileData: struct
    {
        public Model.Graph Graph;
        public TileDescriptor<TTileData> TileDescriptor;
        private readonly Func<WfcGraphTile<TTileData>, WfcCell<WfcGraphTile<TTileData>, int>, WfcSpace<WfcGraphTile<TTileData>, int>, bool> _canModuleBePlacedFunction;

        public WfcGraphTile(Model.Graph graph, Func<WfcGraphTile<TTileData>, WfcCell<WfcGraphTile<TTileData>, int>, WfcSpace<WfcGraphTile<TTileData>, int>,bool> canModuleBePlacedFunction, TileDescriptor<TTileData> tileDescriptor)
        {
            Graph = graph;
            _canModuleBePlacedFunction = canModuleBePlacedFunction;
            TileDescriptor = tileDescriptor;
        }
        
        public WfcGraphTile(Model.Graph graph, Func<WfcGraphTile<TTileData>, WfcCell<WfcGraphTile<TTileData>, int>, WfcSpace<WfcGraphTile<TTileData>, int>,bool> canModuleBePlacedFunction)
        {
            Graph = graph;
            _canModuleBePlacedFunction = canModuleBePlacedFunction;
            TileDescriptor = new TileDescriptor<TTileData>();
        }

        public bool CanModuleBePlaced(WfcCell<WfcGraphTile<TTileData>, int> currentCell,
            WfcSpace<WfcGraphTile<TTileData>, int> wfcSpace)
        {
            return _canModuleBePlacedFunction.Invoke(this, currentCell, wfcSpace);
        }
    }

    public class TileDescriptor<TTileData> where TTileData: struct
    {
        public List<BorderNode> BorderNodes { get; }
        public TTileData AdditionalTileData { get; set; }

        public TileDescriptor()
        {
            BorderNodes = new List<BorderNode>();
        }

        public void AddBorderNode(Entity entity, uint mandatoryConnections = 1, uint optionalConnections = 0)
        {
            BorderNodes.Add(new BorderNode()
            {
                Entity = entity,
                MandatoryConnections = mandatoryConnections, 
                OptionalConnections = optionalConnections
            });
        }
    }

    public class BorderNode
    {
        public Entity Entity { get; set; }
        public uint MandatoryConnections { get; init; }
        public uint OptionalConnections { get; init; }

        public bool HasMandatoryConnection() => MandatoryConnections > 0;
        public bool HasOptionalConnections() => OptionalConnections > 0;
        public uint TotalConnections => MandatoryConnections + OptionalConnections;
    }
}