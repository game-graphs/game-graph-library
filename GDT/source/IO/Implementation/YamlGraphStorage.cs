using GDT.Model;

namespace GDT.IO.Implementation
{
    public sealed class YamlGraphStorage : IGraphStorage
    {
        public static string SaveGraph(Graph graph)
        {
            return YamlStorage.SaveGraph(graph);
        }
        
        public static Graph LoadGraph(string content)
        {
            return YamlLoader.LoadGraph(content);
        }
    }
}