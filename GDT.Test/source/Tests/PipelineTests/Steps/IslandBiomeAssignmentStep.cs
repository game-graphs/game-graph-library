using System;
using System.Collections.Generic;
using System.Numerics;
using GDT.Model;
using GDT.Generation.GenerationSteps;
using GDT.Utility.Visualization;

namespace GDT.Test.Tests.PipelineTests.Steps
{
    public class IslandBiomeAssignmentStep : BaseBiomeAssignmentPipelineStep
    {
        private readonly Vector2 _islandCenter;
        private readonly float _islandRadius;
        private readonly Random _random;

        public IslandBiomeAssignmentStep(Func<List<Entity>> entitiesToEvaluate, Vector2 islandCenter, float islandRadius) : base(entitiesToEvaluate)
        {
            _random = new Random();
            _islandCenter = islandCenter;
            _islandRadius = islandRadius;
        }

        protected override void AssignBiomes(List<Entity> nodes)
        {
            int specialZonesCount = 0;
            float specialZonesTarget = nodes.Count * 0.15f;
            
            for (int i = 0; i < nodes.Count; i++)
            {
                Entity zoneEntity = nodes[i];
                Vector2 location = GetLocationFromLocationComponent(zoneEntity);
                
                var biomeComponent = AddBiomeCombonent(zoneEntity);
                if (IsInCircle(location))
                {
                    bool isSpecialBiome = IsSpecialBiome(nodes, i, specialZonesTarget, specialZonesCount);
                    if (isSpecialBiome) specialZonesCount++;
                    
                    Entity parent;
                    if (!zoneEntity.Parent.TryGetTarget(out parent)) 
                        throw new ArgumentNullException("Expected to find a parent with the available biomes");
                    
                    var (baseBiome, specialBiome) = GetBiomesFromComponent(parent);
                    var selectedBiome = (isSpecialBiome) ? specialBiome : baseBiome;
                    biomeComponent.SetProperty("biome", selectedBiome);
                }
                else
                {
                    biomeComponent.SetProperty("biome", Biome.Sea);
                }
            }
        }

        private bool IsSpecialBiome(List<Entity> nodes, int i, float specialZonesTarget, int specialZonesCount)
        {
            int remainingNodes = nodes.Count - 1 - i;
            float specialNodesDiff = specialZonesTarget - specialZonesCount;
            float specialChance = specialNodesDiff / remainingNodes;

            return _random.NextDouble() < specialChance;
        }

        private Vector2 GetLocationFromLocationComponent(Entity entity)
        {
            EntityComponent areaComponent = entity.GetComponent("Position2DComponent");
            var x = areaComponent?.Get<float>("x");
            var y = areaComponent?.Get<float>("y");
            
            if (x == null || y == null) throw new ArgumentNullException($"Expected {entity.Name} to have an Position2DComponent with x and y");
            
            return new Vector2(x.Value, y.Value);
        }

        private (Biome, Biome) GetBiomesFromComponent(Entity islandEntity)
        {
            var islandComponent = islandEntity.GetComponent("IslandComponent");
            if (islandComponent == null) throw new ArgumentNullException($"Expected {islandEntity.Name} to contain an IslandComponent");

            Biome baseBiome = islandComponent.Get<Biome>("baseBiome");
            Biome specialBiome = islandComponent.Get<Biome>("specialBiome");

            return (baseBiome, specialBiome);
        }

        private bool IsInCircle(Vector2 location)
        {
            return (location - _islandCenter).LengthSquared() < _islandRadius * _islandRadius;
        }
    }
}