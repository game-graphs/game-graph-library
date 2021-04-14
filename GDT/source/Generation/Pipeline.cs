using System.Collections.Generic;
using GDT.Generation.GenerationSteps;
using GDT.Model;

namespace GDT.Generation
{
    public class Pipeline
    {
        private readonly IPipelineInitialization _pipelineInitialization;
        private readonly List<IPipelineStep> _pipelineSteps;

        public Pipeline(IPipelineInitialization pipelineInitialization, List<IPipelineStep> pipelineSteps)
        {
            _pipelineInitialization = pipelineInitialization;
            _pipelineSteps = pipelineSteps;
        }
        
        public Pipeline(IPipelineInitialization pipelineInitialization)
            : this(pipelineInitialization, new List<IPipelineStep>()) 
        {}

        public Pipeline AddStep(IPipelineStep step)
        {
            _pipelineSteps.Add(step);
            return this;
        }

        public Graph Execute()
        {
            Graph graph = InitializePipeline();
            return ExecutePipeline(graph);
        }

        private Graph InitializePipeline()
        {
            return _pipelineInitialization.Initialize();
        }

        private Graph ExecutePipeline(Graph graph)
        {
            for (int i = 0; i < _pipelineSteps.Count; i++)
            {
                graph = _pipelineSteps[i].ExecuteStep(graph);
            }

            return graph;
        }
    }
}