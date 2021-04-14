using GDT.Model;

namespace GDT.Generation
{
    public interface IPipelineInitialization
    {
        public string Name { get; }

        public Graph Initialize();
    }
    
    public interface IPipelineStep
    {
        public string Name { get; }

        public Graph ExecuteStep(Graph graph);
    }
}