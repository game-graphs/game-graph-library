using System;
using System.Drawing;
using System.Drawing.Imaging;
using GDT.Algorithm.WFC;
using GDT.Algorithm.WFC.Implementation.Grid;
using GDT.Utility.Grid;
using GDT.Test.Tests.WFC;

namespace GDT.Utility.Visualization
{
    public class WfcImageGenerator
    {
        public Image CurrentFrame { get; }
        private Graphics _graphics;
        
        public WfcImageGenerator(int width, int height)
        {
            CurrentFrame = new Bitmap(width, height);
            _graphics = Graphics.FromImage(CurrentFrame);
        }

        public void DrawWfcGrid(WfcSpaceGrid2D<TileInfo2D> grid2D, Image tileSheet)
        {
            _graphics.Clear(Color.SkyBlue);
            for (int index = 0; index < grid2D.Grid.Count; index++)
            {
                (int x, int y) = grid2D.Grid.GetPosition(index);
                WfcCell<TileInfo2D, Position2D> cell = grid2D.GetCell(x, y);
                int renderingY = grid2D.Height - 1 - y; // Y axis is down when generating the image
                DrawCell(cell, tileSheet, x, renderingY);
            }
        }

        private void DrawCell(WfcCell<TileInfo2D, Position2D> cell, Image tileSheet, int cellX, int cellY)
        {
            if (cell.AvailableModules.Count > 0)
            {
                int elements = (int) Math.Ceiling(Math.Sqrt(cell.AvailableModules.Count));
                for (int moduleIndex = 0; moduleIndex < cell.AvailableModules.Count; moduleIndex++)
                {
                    var module = cell.AvailableModules[moduleIndex];
                    
                    Rectangle drawRect = new Rectangle(0,0, module.Region.Width, module.Region.Height);
                    var cellOffset = new Point(cellX * drawRect.Width, cellY * drawRect.Height);
                    var innerCellOffset = new Point( (moduleIndex % elements) * (drawRect.Width/elements),  (moduleIndex/elements) * (drawRect.Height/elements));
                    drawRect.Offset(cellOffset);
                    drawRect.Offset(innerCellOffset);
                    drawRect.Width = drawRect.Width / elements;
                    drawRect.Height = drawRect.Height / elements;
                    
                    DrawImageRegion(tileSheet, module.Region, drawRect);
                    _graphics.DrawRectangle(new Pen(Color.Black), drawRect);
                }
            }
        }

        private void DrawImageRegion(Image sourceImg, Rectangle sourceRect, Rectangle destinationRect)
        {
            _graphics.DrawImage(sourceImg, destinationRect, sourceRect, GraphicsUnit.Pixel);
        }

        public void SaveImage(string fileName, ImageFormat format)
        {
            CurrentFrame.Save(fileName, format);
        }
        
        public void SaveImage(string fileName)
        {
            SaveImage(fileName, ImageFormat.Png);
        }
    }
}