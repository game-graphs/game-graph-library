#nullable enable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using GDT.Algorithm.Sampler;
using GDT.Generation;
using GDT.Model;
using GDT.Generation.GenerationSteps;
using GDT.Generation.VisualizationSteps;
using GDT.Test.Tests.PipelineTests.Steps;
using GDT.Utility.Visualization;
using GDT.Utility;
using NUnit.Framework;

namespace GDT.Test.Tests.PipelineTests
{
    public class IslandsGenerationTests
    {
        [Test]
        public void IslandPipelineTest()
        {
            Pipeline pipeline = PipelineUtility.CreateEmptyPipelineUsingGraph("World Graph");

            Vector2 worldRegion = new Vector2(2000, 2000);
            float islandSize = 300;
            float voronoiRadius = islandSize * 0.15f;
            string zoneLayerName = "ZoneLayer";

            pipeline.AddStep(new PoissonLocationsAsEntitiesPipelineStep(islandSize, new(islandSize / 2, islandSize / 2),
                    worldRegion - new Vector2(islandSize, islandSize), null, 60));
            pipeline.AddStep(SetupIslandVisualizationStep(worldRegion, islandSize));
            pipeline.AddStep(new IslandTypeAssignment(worldRegion / 2));
            pipeline.AddStep(SetupIslandTypeVisualizationStep(worldRegion, islandSize));
            pipeline.AddStep(new IteratorPipelineStep<Entity>(
                graph => graph.Entities,
                node =>
                {
                    var (centerX, centerY) = ComponentUtility.GetPosition2DFromComponent(node);
                    Vector2 islandMin = new(centerX - islandSize / 2, centerY - islandSize / 2);
                    Vector2 islandMax = new(centerX + islandSize / 2, centerY + islandSize / 2);
                    return new VoronoiPipelineStep(
                        () => PoissonDiscSampler.GeneratePoints(voronoiRadius, new(islandSize, islandSize), islandMin),
                        zoneLayerName, islandMin, islandMax, node);
                }));
            pipeline.AddStep(new IteratorPipelineStep<Entity>(
                    graph => graph.Entities,
                    node =>
                    {
                        var (centerX, centerY) = ComponentUtility.GetPosition2DFromComponent(node);
                        return new IslandBiomeAssignmentStep(() => node.Children, new Vector2(centerX, centerY), voronoiRadius * 2);
                    }));
            pipeline.AddStep(SetupIslandShapeVisualizationStep(worldRegion, zoneLayerName));
            pipeline.AddStep(SetupIslandZonesVisualizationStep(worldRegion, zoneLayerName));

            Graph islandGraph = pipeline.Execute();
            Layer zoneLayer = islandGraph.GetLayer(zoneLayerName);
            Assert.NotNull(zoneLayer, "Expected a non null zone layer");
        }

        private static Color MapBiomeToColor(Biome biome)
        {
            switch (biome)
            {
                case Biome.Grassland: return Color.LimeGreen;
                case Biome.Mountain: return Color.LightBlue;
                case Biome.Sea: return Color.DodgerBlue;
                case Biome.Forest: return Color.DarkGreen;
                case Biome.Swamp: return Color.DarkGoldenrod;
                case Biome.Unassigned: return Color.Purple;
            }

            throw new InvalidEnumArgumentException($"Biome case not mapped for '{biome}'");
        }

        private PointVisualizationStep SetupIslandVisualizationStep(Vector2 region, float islandSize)
        {
            return new PointVisualizationStep(
                (int) region.X, (int) region.Y,
                Color.Green, islandSize,
                drawing => drawing.SaveImage("island_gen_1_locations.png"),
                backgroundColor: MapBiomeToColor(Biome.Sea));
        }
        
        private PointVisualizationStep SetupIslandTypeVisualizationStep(Vector2 region, float islandSize)
        {
            return new PointVisualizationStep(
                (int) region.X, (int) region.Y,
                node =>
                {
                    EntityComponent? islandComponent = node.GetComponent("IslandComponent");
                    Biome biome = islandComponent?.Get<Biome>("baseBiome") ?? Biome.Unassigned;
                    return MapBiomeToColor(biome);
                }, islandSize,
                drawing => drawing.SaveImage("island_gen_2_locations_with_biome.png"),
                backgroundColor: MapBiomeToColor(Biome.Sea));
        }

        private AreaVisualizationStep SetupIslandShapeVisualizationStep(Vector2 region, string layerName)
        {
            return new AreaVisualizationStep((int) region.X, (int) region.Y,
                node =>
                { 
                    EntityComponent? biomeComponent = node.GetComponent("BiomeComponent");
                    Biome biome = biomeComponent?.Get<Biome>("biome") ?? Biome.Sea;
                    if(biome == Biome.Sea) return Color.Transparent;

                    Entity parent;
                    if (node.Parent.TryGetTarget(out parent))
                    {
                        EntityComponent? islandComponent = parent.GetComponent("IslandComponent");
                        Biome islandBiome = islandComponent?.Get<Biome>("baseBiome") ?? Biome.Unassigned;
                        return MapBiomeToColor(islandBiome);
                    }
                    return MapBiomeToColor(Biome.Unassigned);
                },
                drawing => drawing.SaveImage("island_gen_3_island_shape.png"),
                graph =>
                {
                    Layer? zoneLayer = graph.GetLayer(layerName);
                    if(zoneLayer == null) throw new ArgumentNullException($"{zoneLayer} doesnt contain nodes");
                    
                    return zoneLayer.GetEntitiesInLayer().ToList();
                },
                backgroundColor: MapBiomeToColor(Biome.Sea));
        }
        
        private AreaVisualizationStep SetupIslandZonesVisualizationStep(Vector2 region, string layerName)
        {
            return new AreaVisualizationStep((int) region.X, (int) region.Y,
                node =>
                { 
                    EntityComponent? biomeComponent = node.GetComponent("BiomeComponent");
                    Biome biome = biomeComponent?.Get<Biome>("biome") ?? Biome.Sea;
                    if(biome == Biome.Sea) return Color.Transparent;
                    
                    return MapBiomeToColor(biome);
                },
                drawing => drawing.SaveImage("island_gen_4_island_final.png"),
                graph =>
                {
                    Layer? zoneLayer = graph.GetLayer(layerName);
                    if(zoneLayer == null) throw new ArgumentNullException($"{zoneLayer} doesnt contain nodes");
                    
                    return zoneLayer.GetEntitiesInLayer().ToList();
                },
                backgroundColor: MapBiomeToColor(Biome.Sea));
        }
    }
}