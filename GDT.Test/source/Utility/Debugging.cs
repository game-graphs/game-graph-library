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
        public static void PrintGraphNodes(IEnumerable<Entity> nodes, Func<Entity, int, string> toString, int depth = 0)
        {
            foreach (var node in nodes)
            {
                Console.WriteLine(toString.Invoke(node, depth));

                PrintGraphNodes(node.Children, toString, depth + 1);
            }
        }

        public static void PrintWfcGraphModules<TTileData>(IEnumerable<Entity> nodes,
            string wfcComponentName = "WfcComponent", 
            string wfcCellName = "WfcCell") where TTileData: struct
        {
            PrintGraphNodes(nodes, (node, d) => DepthString(d) + WfcComponentToString<TTileData>(node, wfcComponentName, wfcCellName));
        }
        
        public static void PrintGraph(IEnumerable<Entity> nodes)
        {
            PrintGraphNodes(nodes, (node, d) => DepthString(d) + node.Name);
        }

        private static string DepthString(int depth)
        {
            return (depth == 0) ? "> " : Enumerable.Repeat("-", depth).Aggregate((a, b) => a + b) + "> ";
        }

        private static string WfcComponentToString<TTileData>(Entity entity, string wfcComponentName, string wfcCellName)
                                where TTileData: struct
        {
            var cell = GetWfcCellFromNode<TTileData>(entity, wfcComponentName, wfcCellName);
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

        private static WfcCell<WfcGraphTile<TTileData>, int> GetWfcCellFromNode<TTileData>(Entity entity, string wfcComponentName, string wfcCellName) 
                            where TTileData: struct
        {
            var component = entity.GetComponent(wfcComponentName);
            return component?.Get<WfcCell<WfcGraphTile<TTileData>, int>>(wfcCellName);
        }
    }
}