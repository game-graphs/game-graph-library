using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;


namespace GDT.Utility.Visualization
{
    public class ImageDrawing
    {
        public Image Image { get; }
        private readonly Graphics _graphics;
        
        public ImageDrawing(int width, int height)
        {
            Image = new Bitmap(width, height);
            _graphics = Graphics.FromImage(Image);
        }

        public void Clear(Color color)
        {
            _graphics.Clear(color);
        }

        public void DrawLine(PointF p1, PointF p2, Color color, float width = 1.0f)
        {
            var pen = new Pen(color, width);
            _graphics.DrawLine(pen, p1, p2);
            pen.Dispose();
        }

        public void FillPoly(List<PointF> points, Color color)
        {
            var brush = new SolidBrush(color);
            _graphics.FillPolygon(brush, points.ToArray());
        }

        public void FillCircles(List<Vector2> points, float radius, Color color)
        {
            var brush = new SolidBrush(color);
            points.ForEach(point => _graphics.FillEllipse(brush, point.X-radius/2, point.Y-radius/2, radius, radius));
        }
        
        public void FillCircle(Vector2 point, float radius, Color color)
        {
            var brush = new SolidBrush(color);
            _graphics.FillEllipse(brush, point.X-radius/2, point.Y-radius/2, radius, radius);
        }

        public void SaveImage(string fileName, ImageFormat format)
        {
            Image.Save(fileName, format);
        }
        
        public void SaveImage(string fileName)
        {
            SaveImage(fileName, ImageFormat.Png);
        }
    }
}