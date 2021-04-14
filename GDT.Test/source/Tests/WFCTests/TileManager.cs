using System;
using System.Collections.Generic;
using System.Drawing;
using GDT.Utility.Grid;
using GDT.Utility.Visualization;

namespace GDT.Test.Tests.WFC
{
    public class TileManager
    {
        public List<TileInfo2D> Tiles { get; }
        
        public Image TileSheetImage { get; }
        public int PixelPerTile { get; }

        public TileManager(string fileName, int pixelPerTile)
        {
            TileSheetImage = FileUtilities.LoadImage(fileName);
            Tiles = new List<TileInfo2D>();
            PixelPerTile = pixelPerTile;
        }

        public void AddTileInfo(string name, int indexX, int indexY, TileSockets left, TileSockets right, TileSockets up, TileSockets down)
        {
            TileInfo2D tileInfo2D = new TileInfo2D()
            {
                Name = name,
                Region =
                {
                    X = indexX * PixelPerTile, Y = indexY * PixelPerTile,
                    Width = PixelPerTile, Height = PixelPerTile, 
                },
                AllowedSockets = new Dictionary<Direction2D, TileSockets>()
                {
                    { Direction2D.Left, left },
                    { Direction2D.Right, right },
                    { Direction2D.Up, up },
                    { Direction2D.Down, down }
                },
            };
            Tiles.Add(tileInfo2D);
        }
        
        public List<TileInfo2D> FindMatchingTiles(Predicate<TileInfo2D> predicate)
        {
            return Tiles.FindAll(predicate);
        }
    }
}