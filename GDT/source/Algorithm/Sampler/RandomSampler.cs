using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using GDT.Utility.Grid;

namespace GDT.Algorithm.Sampler
{
    public class RandomSampler
    {
        private static Random _random = new();
        
        public static List<Vector2> GeneratePoints(int count, Vector2 sampleRegionSize, Vector2 offset)
        {
            List<Vector2> generatedPoints = new();

            for (int i = 0; i < count; i++)
            {
                generatedPoints.Add(new Vector2(sampleRegionSize.X * (float) _random.NextDouble(), sampleRegionSize.Y * (float) _random.NextDouble()));
            }
            
            if(offset.Length() > 0.0f) generatedPoints = generatedPoints.Select(p => p + offset).ToList();
            
            return generatedPoints;
        }
        
        public static List<Vector2> GeneratePoints(int count, Vector2 sampleRegionSize)
        {
            return GeneratePoints(count, sampleRegionSize, Vector2.Zero);
        }
        
    }
}