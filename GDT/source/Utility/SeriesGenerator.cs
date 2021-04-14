using System;
using System.Collections.Generic;

namespace GDT.Utility
{
    public static class SeriesGenerator
    {
        /// <summary>
        /// Generates pairs from (start, start+1) up to (end - 1, end)
        /// </summary>
        /// <param name="start">the start value for the series</param>
        /// <param name="end">the last value in this series</param>
        /// <param name="connectEndToStart">If true, adds (end, start)</param>
        /// <returns>List of tuples from start to end</returns>
        public static IEnumerable<Tuple<int, int>> PairSeries(int start, int end, bool connectEndToStart = false)
        {
            for (int current = start; current < end; current++)
            {
                yield return new(current, current + 1);
            }

            if (connectEndToStart) yield return new(end, start);
        }

        public static List<Tuple<int, int>> PairWise(int[] values)
        {
            var pairs = new List<Tuple<int, int>>();

            for (int i = 0; i < values.Length - 1; i++)
            {
                pairs.Add(new Tuple<int, int>(values[i], values[i+1]));
            }
            
            return pairs;
        }
    }
}