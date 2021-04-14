using System;
using System.Collections.Generic;
using GDT.Generation;
using GDT.Model;

namespace GDT.Generation.GenerationSteps
{
    public class IteratorPipelineStep<T> : IPipelineStep
    {
        public string Name { get; } = nameof(IteratorPipelineStep<T>);

        private readonly Func<Graph, IEnumerable<T>> _enumerableGenerator;
        private readonly Func<T, IPipelineStep> _pipelineStepGenerator;

        public IteratorPipelineStep(Func<Graph, IEnumerable<T>> enumerableGenerator, Func<T, IPipelineStep> pipelineStepGenerator)
        {
            _enumerableGenerator = enumerableGenerator;
            _pipelineStepGenerator = pipelineStepGenerator;
        }

        public Graph ExecuteStep(Graph graph)
        {
            foreach (var item in _enumerableGenerator.Invoke(graph))
            {
                var pipelineStep = _pipelineStepGenerator.Invoke(item);
                graph = pipelineStep.ExecuteStep(graph);
            }
            
            return graph;
        }
    }
}