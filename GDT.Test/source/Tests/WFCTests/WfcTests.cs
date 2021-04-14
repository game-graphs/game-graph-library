using System;
using System.Collections.Generic;
using System.IO;
using GDT.Algorithm.WFC;
using GDT.Algorithm.WFC.Implementation.Grid;
using GDT.Algorithm;
using GDT.Utility.Visualization;
using NUnit.Framework;

#nullable enable

namespace GDT.Test.Tests.WFC
{
    public class WfcTests
    {
        private TileManager _tileManager = null!;
        private Random _random = null!;
        
        [SetUp]
        public void Setup()
        {
            _random = new Random((int) DateTime.Now.ToFileTimeUtc());
            
            _tileManager = new TileManager("platformer_tilesheet.png", 64);
            _tileManager.AddTileInfo("Soil", 0, 0, TileSockets.Soil, TileSockets.Soil, TileSockets.Soil, TileSockets.Soil);
            _tileManager.AddTileInfo("Soil 2", 0, 2, TileSockets.Soil, TileSockets.Soil, TileSockets.Soil, TileSockets.Soil);
            _tileManager.AddTileInfo("Level-Ground", 2, 0, TileSockets.Ground | TileSockets.InnerGroundLeft, TileSockets.Ground | TileSockets.InnerGroundRight, TileSockets.Air | TileSockets.Foliage, TileSockets.Soil);
            _tileManager.AddTileInfo("Level-Ground-2", 6, 0, TileSockets.Ground | TileSockets.InnerGroundLeft, TileSockets.Ground, TileSockets.Air | TileSockets.Foliage, TileSockets.Soil);
            _tileManager.AddTileInfo("Slope-Right-Up", 1, 1, TileSockets.Air, TileSockets.InnerGroundLeft, TileSockets.Air, TileSockets.InnerGroundRight);
            _tileManager.AddTileInfo("Slope-Right-Down", 2, 1, TileSockets.InnerGroundRight, TileSockets.Air, TileSockets.Air, TileSockets.InnerGroundLeft);
            _tileManager.AddTileInfo("Slope-Connector-Left", 2, 2, TileSockets.Soil, TileSockets.InnerGroundLeft, TileSockets.InnerGroundLeft, TileSockets.Soil);
            _tileManager.AddTileInfo("Slope-Connector-Right", 1, 2, TileSockets.InnerGroundRight, TileSockets.Soil, TileSockets.InnerGroundRight, TileSockets.Soil);
            _tileManager.AddTileInfo("Air", 20, 4, TileSockets.Air, TileSockets.Air, TileSockets.Air, TileSockets.Air);
            _tileManager.AddTileInfo("Flower", 9, 2, TileSockets.Air, TileSockets.Air, TileSockets.Air, TileSockets.Foliage);
            _tileManager.AddTileInfo("Flower2", 10, 3, TileSockets.Air, TileSockets.Air, TileSockets.Air, TileSockets.Foliage);
        }

        [Test]
        public void TestGenerateImage()
        {
            WfcSpaceGrid2D<TileInfo2D> wfcWorld = new(5, 3, new List<TileInfo2D>(_tileManager.Tiles));
            GifGenerator gifGenerator = new("generated.gif");
            WfcImageGenerator wfcImageGenerator = new(wfcWorld.Width * _tileManager.PixelPerTile, wfcWorld.Height * _tileManager.PixelPerTile);
            Directory.CreateDirectory("wfc");
            
            int i = 0;
            while (!wfcWorld.GenerationFinished())
            {
                wfcImageGenerator.DrawWfcGrid(wfcWorld, _tileManager.TileSheetImage);
                gifGenerator.AddFrame(wfcImageGenerator.CurrentFrame);
                wfcImageGenerator.SaveImage($"wfc/wfc_step_{i}.png");

                var selectCell = wfcWorld.SelectCell(cell => Int32.MaxValue - cell.AvailableModules.Count);
                wfcWorld.Collapse(selectCell, (space, info2D) => Console.WriteLine($"Selected {info2D.Name} at {wfcWorld.Grid.GetPosition(wfcWorld.Grid.FindIndex(cell => cell == selectCell))}"));;
                i++;
            }
            
            wfcImageGenerator.DrawWfcGrid(wfcWorld, _tileManager.TileSheetImage);
            gifGenerator.AddFrame(wfcImageGenerator.CurrentFrame);
            wfcImageGenerator.SaveImage("wfc_image_full.png");
        }

    }
}