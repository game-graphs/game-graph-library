using System;
using System.Collections.Generic;
using System.Linq;
using GDT.Algorithm.WFC;
using GDT.Algorithm.WFC.Implementation.Graph;
using GDT.Model;
using GDT.Algorithm;
using GDT.Utility.Visualization.DataStructures;

namespace GDT.Utility.Visualization
{
    public static class PokeWorldGraphTiles
    {

        public static List<WfcGraphTile<WorldData>> GetPocketWorldGraphTiles(Blackboard initialBlackboard, int numberOfRoads = 33, int numberOfCities = 8, int numberOfSpecials = 2)
        {
            List<WfcGraphTile<WorldData>> pokeTiles = new List<WfcGraphTile<WorldData>>();

            for (int i = 0; i < numberOfRoads; i++)
            {
                pokeTiles.Add(CreateRoadTile($"Route {i}", initialBlackboard));
            }

            List<string> cityNames = new List<string>()
            {
                "Pallet Town", "Saffronia City", "Lavender Town", "Alabastia City", "Viridian City", "Fuchsia City", 
                "Cerulean City", "Pewter City"
            };
            for(int i = 0; i < Math.Min(cityNames.Count, numberOfCities); i++)
            {
                pokeTiles.Add(CreateCityTile(cityNames[i], initialBlackboard));
            }

            List<string> specialNames = new List<string>()
            {
                "Top 4", "Bills House"
            };
            for(int i = 0; i < Math.Min(specialNames.Count, numberOfSpecials); i++)
            {
                pokeTiles.Add(CreateSpecialTile(specialNames[i], initialBlackboard));
            }
            
            return pokeTiles;
        }

        public static WfcGraphTile<WorldData> CreateRoadTile(string roadName, Blackboard initialBlackboard)
        {
            Graph roadGraph = Graph.CreatePlaceholder(roadName);
            Entity roadEntity = new Entity(roadName, roadGraph);
            roadGraph.Entities.Add(roadEntity);
            var blackboardRoadCountKey = IncrementBlackboardCount(roadName, initialBlackboard);

            WfcGraphTile<WorldData> roadTile = new WfcGraphTile<WorldData>(roadGraph, (thisModule, thisCell, wfcSpace) =>
            {
                bool isTileAvailable = wfcSpace.Blackboard.IsGreaterZero(blackboardRoadCountKey);
                
                var neighbors = wfcSpace.GetNeighbors(thisCell);
                if (!CanConnect(thisModule, neighbors)) return false;

                return isTileAvailable;
            });
            roadTile.TileDescriptor.AddBorderNode(roadEntity, 2, 2);
            roadTile.TileDescriptor.AdditionalTileData = new WorldData(TileType.Road);

            return roadTile;
        }

        private static string IncrementBlackboardCount(string key, Blackboard initialBlackboard)
        {
            string blackboardCountKey = $"{key}_count";
            initialBlackboard.AddOrIncrement(blackboardCountKey, 1);
            return blackboardCountKey;
        }

        public static WfcGraphTile<WorldData> CreateCityTile(string cityName, Blackboard initialBlackboard)
        {
            var cityGraph = Graph.CreatePlaceholder(cityName);
            var cityNode = new Entity(cityName, cityGraph);
            cityGraph.Entities.Add(cityNode);
            var blackboardCityCountKey = IncrementBlackboardCount(cityName, initialBlackboard);
            
            var cityTile = new WfcGraphTile<WorldData>(cityGraph, (thisModule, thisCell, wfcSpace) =>
            {
                if (!wfcSpace.Blackboard.IsGreaterZero(blackboardCityCountKey)) return false;

                var neighbors = wfcSpace.GetNeighbors(thisCell);
                if (!CanConnect(thisModule, neighbors)) return false;
                
                foreach (var neighbor in neighbors)
                {
                    if(!NeighborHasRoadLikeStructure(neighbor))  return false;
                }
                
                return true;
            });
            cityTile.TileDescriptor.AddBorderNode(cityNode, 1, 3);
            cityTile.TileDescriptor.AdditionalTileData = new WorldData(TileType.City);

            return cityTile;
        }
        
        public static WfcGraphTile<WorldData> CreateSpecialTile(string tileName, Blackboard initialBlackboard, uint mandatoryConnections = 1, uint optionalConnections = 0)
        {
            var specialGraph = Graph.CreatePlaceholder(tileName);
            var specialNode = new Entity(tileName, specialGraph);
            specialGraph.Entities.Add(specialNode);
            var blackboardTileCountKey = IncrementBlackboardCount(tileName, initialBlackboard);
            
            var specialTile = new WfcGraphTile<WorldData>(specialGraph, (thisModule, thisCell, wfcSpace) =>
            {
                if (!wfcSpace.Blackboard.IsGreaterZero(blackboardTileCountKey)) return false;

                var neighbors = wfcSpace.GetNeighbors(thisCell);
                if (!CanConnect(thisModule, neighbors)) return false;
                
                foreach (var neighbor in neighbors)
                {
                    if(!NeighborHasRoadLikeStructure(neighbor))  return false;
                }
                
                return true;
            });
            specialTile.TileDescriptor.AddBorderNode(specialNode, mandatoryConnections, optionalConnections);
            specialTile.TileDescriptor.AdditionalTileData = new WorldData(TileType.City);

            return specialTile;
        }

        private static bool CanConnect(WfcGraphTile<WorldData> thisModule, List<WfcCell<WfcGraphTile<WorldData>, int>> neighbors)
        {
            uint availableTotalConnections = thisModule.TileDescriptor.BorderNodes
                                                            .Select(bn => bn.TotalConnections)
                                                            .Aggregate((c1, c2) => c1 + c2);
            if (neighbors.Count > availableTotalConnections) return false;
            
            uint availableMandatoryConnections = thisModule.TileDescriptor.BorderNodes
                .Select(bn => bn.MandatoryConnections)
                .Aggregate((c1, c2) => c1 + c2);
            if (neighbors.Count < availableMandatoryConnections) return false;
            
            return true;
        }

        private static bool NeighborHasRoadLikeStructure(WfcCell<WfcGraphTile<WorldData>, int> neighbor)
        {
            return NeighborHasOneOfTheseTiles(neighbor, new() { TileType.Forrest, TileType.Road, TileType.Cave });
        }
        
        private static bool NeighborHasOneOfTheseTiles(WfcCell<WfcGraphTile<WorldData>, int> neighbor, List<TileType> possibleTiles)
        {
            return neighbor.AvailableModules.Exists(tile =>
            {
                WorldData worldData = tile.TileDescriptor.AdditionalTileData;
                
                return possibleTiles.Contains(worldData.TileType);
            });
        }
    }
}