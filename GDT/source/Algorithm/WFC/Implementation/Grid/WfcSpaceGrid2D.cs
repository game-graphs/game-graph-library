using System.Collections.Generic;
using GDT.Utility.Grid;

#nullable enable

namespace GDT.Algorithm.WFC.Implementation.Grid
{
    public class WfcSpaceGrid2D<TModule> : WfcSpace<TModule, Position2D>
                            where TModule: class, IWfcModule<TModule, Position2D>
    {
        public Grid2D<WfcCell<TModule, Position2D>> Grid { get; }
        
        public int Width => Grid.Width;

        public int Height => Grid.Height;

        public WfcSpaceGrid2D(int width, int height, List<TModule> availableModules)
        {
            Grid = new Grid2D<WfcCell<TModule, Position2D>>(
                width, height, 
                (x, y) => new WfcCell<TModule, Position2D>(new Position2D(x, y), availableModules));
        }

        public override WfcCell<TModule, Position2D>? GetCell(Position2D position)
        {
            return GetCell(position.X, position.Y);
        }

        public override bool GenerationFinished()
        {
            return !Grid.GetCellsAsList().Exists(cell => cell.AvailableModules.Count > 1);
        }

        protected override IEnumerable<WfcCell<TModule, Position2D>> GetCells()
        {
            return Grid.GetCells();
        }

        public override List<WfcCell<TModule, Position2D>> GetNeighbors(WfcCell<TModule, Position2D> cell)
        {
            int index = Grid.IndexOf(cell);
            (int centerX, int centerY) = Grid.GetPosition(index);

            List<WfcCell<TModule, Position2D>> neighbors = new();

            var child = GetCell(centerX + 1, centerY);
            if (child != null) neighbors.Add(child);
            
            child = GetCell(centerX - 1, centerY);
            if(child != null) neighbors.Add(child);
            
            child = GetCell(centerX, centerY + 1);
            if(child != null) neighbors.Add(child);
            
            child = GetCell(centerX, centerY - 1);
            if(child != null) neighbors.Add(child);
            
            return neighbors;
        }

        public WfcCell<TModule, Position2D>? GetCell(int x, int y)
        {
            if (!Grid.IsValid(x, y)) return null;

            return Grid.Get(x, y);
        }
    }
}