using GDT.Generation;
using GDT.Model;

namespace GDT.Generation.GenerationSteps
{
    public class GraphPipelineInitializer : IPipelineInitialization
    {
        public string Name { get; } = nameof(GraphPipelineInitializer);

        private Graph _graph;

        public GraphPipelineInitializer(Graph graph)
        {
            _graph = graph;
        }

        public GraphPipelineInitializer(string graphName)
        {
            _graph = new Graph(graphName);
        }

        public Graph Initialize()
        {
            return _graph;
        }
    }
}