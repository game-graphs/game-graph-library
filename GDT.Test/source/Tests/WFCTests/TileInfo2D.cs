using System.Collections.Generic;
using System.Drawing;
using GDT.Algorithm.WFC;
using GDT.Utility.Grid;


namespace GDT.Test.Tests.WFC
{
    public class TileInfo2D : IWfcModule<TileInfo2D, Position2D>
    {
        public string Name;
        public Rectangle Region;
        public Dictionary<Direction2D, TileSockets> AllowedSockets;

        public bool CanModuleBePlaced(WfcCell<TileInfo2D, Position2D> currentCell, WfcSpace<TileInfo2D, Position2D> wfcSpace)
        {
            foreach (var neighbor in wfcSpace.GetNeighbors(currentCell))
            {
                if (!CanConnectToAnyNeighborTile(currentCell, neighbor))
                {
                    return false;
                }
            }
            
            return true;
        }

        private bool CanConnectToAnyNeighborTile(WfcCell<TileInfo2D, Position2D> currentCell, WfcCell<TileInfo2D, Position2D> neighborCell)
        {
            Direction2D directionToNeighbor = currentCell.Position.Delta(neighborCell.Position);
            var allowedSockets = AllowedSockets[directionToNeighbor];
            
            if (!neighborCell.AvailableModules.Exists(module => (module.AllowedSockets[directionToNeighbor.Opposite()] & allowedSockets) != 0))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}