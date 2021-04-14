#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;
using GDT.Generation;
using GDT.Model;
using GDT.Utility.Visualization;
using GDT.Utility;

namespace GDT.Test.Tests.PipelineTests.Steps
{
    public class IslandTypeAssignment : IPipelineStep
    {
        public string Name { get; } = nameof(IslandTypeAssignment);
        
        private readonly Func<Graph, IEnumerable<Entity>> _islandNodesEnumerator = graph => graph.Entities;
        private readonly Vector2 _regionCenter;
        private readonly Random _random;

        public IslandTypeAssignment(Vector2 regionCenter, Func<Graph, IEnumerable<Entity>>? islandNodesEnumerator = null)
        {
            _random = new Random();
            _regionCenter = regionCenter;
            if (islandNodesEnumerator != null) _islandNodesEnumerator = islandNodesEnumerator;
        }

        public Graph ExecuteStep(Graph graph)
        {
            List<KeyValuePair<Entity, float>> nodeDistancePercentageList = new();
            CreateSortedNodeAndDistancePercentageList(graph, nodeDistancePercentageList);

            SetupStartingIsland(nodeDistancePercentageList);
            SetupRemainingIslands(nodeDistancePercentageList);

            return graph;
        }

        private void CreateSortedNodeAndDistancePercentageList(Graph graph, List<KeyValuePair<Entity, float>> nodeDistancePercentageList)
        {
            foreach (var islandNode in _islandNodesEnumerator.Invoke(graph))
            {
                var (x, y) = ComponentUtility.GetPosition2DFromComponent(islandNode);
                var distancePercentage =
                    Math.Abs((new Vector2(x, y) - _regionCenter).LengthSquared() / (_regionCenter.X * _regionCenter.X));
                nodeDistancePercentageList.Add(new KeyValuePair<Entity, float>(islandNode, distancePercentage));
            }

            nodeDistancePercentageList.Sort((e1, e2) => (int) (e1.Value * 100 - e2.Value * 100));
        }

        private void SetupRemainingIslands(List<KeyValuePair<Entity, float>> nodeDistancePercentageList)
        {
            foreach (var (node, distancePercentage) in nodeDistancePercentageList)
            {
                List<Biome> baseBiomes = GetAvailableBaseBiomes(distancePercentage);
                Biome baseBiome = baseBiomes[_random.Next(0, baseBiomes.Count)];

                List<Biome> specialBiomes = GetAvailableSpecialBiomes(distancePercentage, baseBiome);
                Biome specialBiome = specialBiomes[_random.Next(0, specialBiomes.Count)];
                AddIslandTypeComponent(node, baseBiome, specialBiome);
            }
        }

        private static void SetupStartingIsland(List<KeyValuePair<Entity, float>> nodeDistancePercentageList)
        {
            var (startingIslandNode, _) = nodeDistancePercentageList[0];
            nodeDistancePercentageList.RemoveAt(0);
            AddIslandTypeComponent(startingIslandNode, Biome.Grassland, Biome.Forrest);
        }

        private static void AddIslandTypeComponent(Entity entity, Biome baseBiome, Biome specialBiome)
        {
            EntityComponent islandComponent = new("IslandComponent");

            islandComponent.SetProperty("baseBiome", baseBiome);
            islandComponent.SetProperty("specialBiome", specialBiome);

            entity.Components.Add(islandComponent);
        }

        private static List<Biome> GetAvailableBaseBiomes(float distancePercentage)
        {
            List<Biome> biomes = new() { Biome.Grassland };

            if (distancePercentage > 0.25f) 
                biomes.Add(Biome.Forrest);
            
            if (distancePercentage > 0.45f) 
                biomes.Add(Biome.Swamp);

            return biomes;
        }
        
        private static List<Biome> GetAvailableSpecialBiomes(float distancePercentage, Biome? baseBiome)
        {
            List<Biome> biomes = new();

            if (SpecialIsNotBase(Biome.Forrest, baseBiome)) biomes.Add(Biome.Forrest);
            else distancePercentage += 0.35f;

            if (distancePercentage > 0.35f && SpecialIsNotBase(Biome.Swamp, baseBiome)) 
                biomes.Add(Biome.Swamp);
            
            if (distancePercentage > 0.45f && SpecialIsNotBase(Biome.Mountain, baseBiome)) 
                biomes.Add(Biome.Mountain);
            
            return biomes;
        }

        private static bool SpecialIsNotBase(Biome specialBiome, Biome? baseBiome)
        {
            if(baseBiome == null) return true;

            return baseBiome != specialBiome;
        }
    }
}