using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GDT.Algorithm.WFC;
using GDT.Algorithm.WFC.Implementation.Graph;
using GDT.Model;

namespace GDT.Utility.Visualization
{
    public class Debugging
    {
        public static void PrintGraphEntities(IEnumerable<Entity> entities, Func<Entity, int, string> toString, int depth = 0)
        {
            foreach (var entity in entities)
            {
                Console.WriteLine(toString.Invoke(entity, depth));

                PrintGraphEntities(entity.Children, toString, depth + 1);
            }
        }

        public static void PrintWfcGraphModules<TTileData>(IEnumerable<Entity> entities,
            string wfcComponentName = "WfcComponent", 
            string wfcCellName = "WfcCell") where TTileData: struct
        {
            PrintGraphEntities(entities, (entity, d) => DepthString(d) + WfcComponentToString<TTileData>(entity, wfcComponentName, wfcCellName));
        }
        
        public static void PrintGraph(IEnumerable<Entity> entities)
        {
            PrintGraphEntities(entities, (entity, d) => DepthString(d) + entity.Name);
        }

        private static string DepthString(int depth)
        {
            return (depth == 0) ? "> " : Enumerable.Repeat("-", depth).Aggregate((a, b) => a + b) + "> ";
        }

        private static string WfcComponentToString<TTileData>(Entity entity, string wfcComponentName, string wfcCellName)
                                where TTileData: struct
        {
            var cell = GetWfcCellFromEntity<TTileData>(entity, wfcComponentName, wfcCellName);
            if (cell == null) return "missing wfc component";

            string modules = "NO AVAILABLE MODULES";
            if (cell.AvailableModules.Count > 0)
            {
                modules = cell.AvailableModules
                    .ConvertAll(tile => tile.Graph.Name)
                    .Aggregate((first, second) => first + " | " + second);
            }

            return $"index: {cell.Position} modules: {modules}";
        }

        private static WfcCell<WfcGraphTile<TTileData>, int> GetWfcCellFromEntity<TTileData>(Entity entity, string wfcComponentName, string wfcCellName) 
                            where TTileData: struct
        {
            var component = entity.GetComponent(wfcComponentName);
            return component?.Get<WfcCell<WfcGraphTile<TTileData>, int>>(wfcCellName);
        }
    }
}