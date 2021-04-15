using System;
using System.Linq;
using GDT.Model;

namespace GDT.Utility
{
    public static class GraphExtensions
    {
        private static readonly Random Random = new();
        
        public static Relation GetRandomRelation(this Layer layer)
        {
            return layer.Relations[Random.Next(0, layer.Relations.Count)];
        }
        
        public static Relation GetSomewhatRandomRelationWithDegreeSmaller(this Layer layer, int maxDegree)
        {
            var result = layer.Relations.Where(relation =>
                {
                    var deg0 = layer.CountNeighbors(relation.Entities[0]);
                    var deg1 = layer.CountNeighbors(relation.Entities[1]);
                    
                    if(deg0 >= maxDegree) return false;
                    if(deg1 >= maxDegree) return false;
                    // prefers new locations and not max locations
                    if (deg0 == maxDegree - 1 && deg1 == maxDegree - 1) return false;
                    

                    return true;
                })
                .ToList();
            int selOpt = Random.Next(0, result.Count);
            #if DEBUG
            if(result.Count == 0)
                throw new ArgumentException($"ERROR: No relations available!");
            #endif
            return result[selOpt];
        }
    }
}