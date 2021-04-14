using System;
using GDT.Model;

namespace GDT.IO
{
    public interface IGraphVisualizerStorage
    {
        public static string SaveGraph(Graph graph)
        {
            throw new NotImplementedException($"Classes that extend {nameof(IGraphVisualizerStorage)} have to implement the SaveGraph(Graph) => string method.");
        }
    }
}