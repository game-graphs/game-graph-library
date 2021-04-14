using System;
using System.Runtime.CompilerServices;
using GDT.Model;

namespace GDT.IO
{
    public interface IGraphStorage
    {
        public static string SaveGraph(Graph graph)
        {
            throw new NotImplementedException($"Classes that extend {nameof(IGraphStorage)} have to implement the SaveGraph(Graph) => string method.");
        }
        
        public static Graph LoadGraph(string content)
        {
            throw new NotImplementedException($"Classes that extend {nameof(IGraphStorage)} have to implement the LoadGraph(string) => Graph method.");
        }
    }
}