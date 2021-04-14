using System;
using System.Numerics;
using GDT.External;

namespace GDT.Utility
{
    public class NoiseGenerator
    {
        public static int GenerateSeed()
        {
            return Environment.TickCount;
        }
        
        public static float[] GeneratePerlinNoise(
            int width, int height, int seed = -1, int octaves = 3, float lacunarity = 2.0f, float gain = 0.5f)
        {
            FastNoiseLite noiseGenerator = new FastNoiseLite();
            noiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            if (seed == -1) seed = GenerateSeed();
            noiseGenerator.SetSeed(seed);
            noiseGenerator.SetFractalOctaves(octaves);
            noiseGenerator.SetFractalLacunarity(lacunarity);
            noiseGenerator.SetFractalGain(gain);

            float[] noiseData = new float[width * height];
            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseData[index++] = noiseGenerator.GetNoise(x, y);
                }
            }

            return noiseData;
        }
    }
}