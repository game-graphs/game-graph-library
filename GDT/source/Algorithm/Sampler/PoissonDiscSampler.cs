using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using GDT.Utility.Grid;

namespace GDT.Algorithm.Sampler
{
    public class PoissonDiscSampler
    {
        private static Random _random = new Random();
        
        public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, Vector2 offset, int rejectionThreshold = 30)
        {
            List<Vector2> generatedPoints = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2>();

            float cellSize = radius / MathF.Sqrt(2);
            Grid2D<int> grid = new Grid2D<int>((int) MathF.Ceiling(sampleRegionSize.X / cellSize), (int) MathF.Ceiling(sampleRegionSize.Y / cellSize), (x, y) => 0);
            
            spawnPoints.Add(new Vector2(sampleRegionSize.X/2, sampleRegionSize.Y/2));

            while (spawnPoints.Any())
            {
                var newLocationIndex = _random.Next(0, spawnPoints.Count);
                Vector2 spawnCenter = spawnPoints[newLocationIndex];

                var pointGenerated = false;
                for (int i = 0; i < rejectionThreshold; i++)
                {
                    var angle = (float) (_random.NextDouble() * MathF.PI * 2);
                    Vector2 direction = new Vector2(MathF.Sin(angle), MathF.Cos(angle));

                    Vector2 candidateLocation = spawnCenter + direction * radius * (1 + (float) _random.NextDouble());
                    if (IsValid(candidateLocation, grid, sampleRegionSize, radius, cellSize, generatedPoints))
                    {
                        generatedPoints.Add(candidateLocation);
                        spawnPoints.Add(candidateLocation);
                        grid.Replace(generatedPoints.Count, grid.Index((int) (candidateLocation.X/cellSize), (int) (candidateLocation.Y/cellSize)));
                        pointGenerated = true;
                        break;
                    }
                }

                if (!pointGenerated) { spawnPoints.RemoveAt(newLocationIndex); }
            }
            
            if(offset.Length() > 0.0f) generatedPoints = generatedPoints.Select(p => p + offset).ToList();
            
            return generatedPoints;
        }
        
        public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int rejectionThreshold = 30)
        {
            return GeneratePoints(radius, sampleRegionSize, Vector2.Zero, rejectionThreshold);
        }

        private static bool IsValid(Vector2 candidateLocation, Grid2D<int> grid, Vector2 sampleRegionSize, float radius, float cellSize, List<Vector2> generatedPoints)
        {
            // not inside box?
            if (candidateLocation.X < 0.0f || candidateLocation.X >= sampleRegionSize.X
                || candidateLocation.Y < 0.0f || candidateLocation.Y >= sampleRegionSize.Y) return false;
            
            // check the grid around the cell
            int cellX = (int) (candidateLocation.X / cellSize);
            int cellY = (int) (candidateLocation.Y / cellSize);
            for (int x = Math.Max(0, cellX - 2); x < Math.Min(cellX + 2, grid.Width); x++)
            {
                for (int y = Math.Max(0, cellY - 2); y < Math.Min(cellY + 2, grid.Height); y++)
                {
                    int pointIndex = grid.Get(x, y) - 1;
                    if (pointIndex != -1)
                    {
                        float distanceSquared = (candidateLocation - generatedPoints[pointIndex]).LengthSquared();
                        if (distanceSquared < radius*radius)
                        {
                            return false;
                        }
                    }
                }
            }
            
            return true;
        }
    }
}