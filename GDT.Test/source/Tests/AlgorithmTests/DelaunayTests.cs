using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using GDT.Algorithm.Sampler;
using GDT.Algorithm.VoronoiLib;
using GDT.Algorithm.VoronoiLib.Structures;
using GDT.Utility.Visualization;
using NUnit.Framework;

namespace GDT.Test.Tests.AlgorithmTests
{
    public class DelaunayTests
    {
        private static readonly Random Random = new();
        
        [Test]
        public void TestDelaunayVisually()
        {
            Vector2 region = new Vector2(800, 600);
            var radius = 77;
            var drawDelaunay = true;
            var greyScale = false;

            List<Vector2> points = PoissonDiscSampler.GeneratePoints(radius, region);

            var fortuneSites = points.Select(p => new FortuneSite(p.X, p.Y)).ToList();
            FortunesAlgorithm.Run(fortuneSites, -100, -100, region.X + 100, region.Y + 100);

            ImageDrawing imageDrawing = new ImageDrawing((int) region.X, (int) region.Y);
            imageDrawing.Clear(Color.White);
            
            foreach (var fortuneSite in fortuneSites)
            {
                Color color = GetRandomColor(greyScale);
                imageDrawing.FillPoly(fortuneSite.Points.Select(p => new PointF((float) p.X, (float) p.Y)).ToList(), color);
                
                if(drawDelaunay)
                {
                    foreach (var fortuneSiteNeighbor in fortuneSite.Neighbors)
                    {
                        imageDrawing.DrawLine(new PointF((float)fortuneSiteNeighbor.X,(float) fortuneSiteNeighbor.Y), new PointF((float)fortuneSite.X,(float) fortuneSite.Y), Color.Pink);
                    }
                }
            }
            
            imageDrawing.FillCircles(points, 4, Color.Black);

            imageDrawing.SaveImage("voronoi_tmp.png");
        }

        private static Color GetRandomColor(bool greyScale = false)
        {
            Color color = Color.FromArgb((int) (Random.NextDouble() * 255), (int) (Random.NextDouble() * 255), (int) (Random.NextDouble() * 255));
            if (!greyScale) 
                return color;
            
            int grayScale = (int)((color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11));
            return Color.FromArgb(color.A, grayScale, grayScale, grayScale);
        }
    }
}