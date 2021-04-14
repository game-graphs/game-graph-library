using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using GDT.Algorithm.Sampler;
using GDT.Utility.Visualization;
using NUnit.Framework;

namespace GDT.Test.Tests.AlgorithmTests
{
    public class PoissonDiscTests
    {
        [Test]
        public void TestPoissonVisually()
        {
            Vector2 region = new Vector2(400, 400);
            var radius = 80;
            
            List<Vector2> points = PoissonDiscSampler.GeneratePoints(radius, region, 30);

            foreach (var point in points)
            {
                Assert.IsTrue(point.X >= 0 && point.X < region.X, $"Point {point.X} not in X region 0,{region.X}");
                Assert.IsTrue(point.Y >= 0 && point.Y < region.Y, $"Point {point.Y} not in Y region 0,{region.Y}");
            }
            
            ImageDrawing imageDrawing = new ImageDrawing((int) region.X, (int) region.Y);
            imageDrawing.Clear(Color.Gray);
            imageDrawing.FillCircles(points, radius, Color.DarkRed);
            imageDrawing.SaveImage("poisson_disc_tmp.png");
        }
        
        [Test]
        public void TestRandomVsPoissonVisually()
        {
            Vector2 region = new Vector2(400, 400);
            var radius = 20;
            
            List<Vector2> poissonPoints = PoissonDiscSampler.GeneratePoints(radius, region, 30);

            foreach (var point in poissonPoints)
            {
                Assert.IsTrue(point.X >= 0 && point.X < region.X, $"Point {point.X} not in X region 0,{region.X}");
                Assert.IsTrue(point.Y >= 0 && point.Y < region.Y, $"Point {point.Y} not in Y region 0,{region.Y}");
            }

            List<Vector2> randomPoints = RandomSampler.GeneratePoints(poissonPoints.Count, region);

            {   //draw poission samples
                ImageDrawing imageDrawing = new ImageDrawing((int) region.X, (int) region.Y);
                imageDrawing.Clear(Color.Transparent);
                imageDrawing.FillCircles(poissonPoints, radius, Color.Black);
                imageDrawing.SaveImage("comp_2_poisson_disc.png");
            }

            {   // draw random samples
                ImageDrawing imageDrawing = new ImageDrawing((int) region.X, (int) region.Y);
                imageDrawing.Clear(Color.Transparent);
                imageDrawing.FillCircles(randomPoints, radius, Color.Black);
                imageDrawing.SaveImage("comp_1_random.png");
            }
            
            Console.WriteLine($"Generated {poissonPoints.Count} points.");
        }
    }
}