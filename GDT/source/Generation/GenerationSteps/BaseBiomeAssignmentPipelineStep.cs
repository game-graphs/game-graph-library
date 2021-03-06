using System;
using System.Collections.Generic;
using GDT.Model;

namespace GDT.Generation.GenerationSteps
{
    public abstract class BaseBiomeAssignmentPipelineStep : IPipelineStep
    {
        public string Name { get; } = nameof(BaseBiomeAssignmentPipelineStep);
        
        private readonly Func<List<Entity>> _entitiesToEvaluate;

        protected BaseBiomeAssignmentPipelineStep(Func<List<Entity>> entitiesToEvaluate)
        {
            _entitiesToEvaluate = entitiesToEvaluate;
        }

        public Graph ExecuteStep(Graph graph)
        {
            AssignBiomes(_entitiesToEvaluate.Invoke());

            return graph;
        }

        protected abstract void AssignBiomes(List<Entity> nodes);

        protected EntityComponent GetBiomeComponent(Entity entity)
        {
            return entity.GetComponent("BiomeComponent");
        }

        protected bool HasBiomeComponent(Entity entity)
        {
            return GetBiomeComponent(entity) != null;
        }
        
        protected EntityComponent AddBiomeCombonent(Entity entity)
        {
            EntityComponent biomeComponent = new EntityComponent("BiomeComponent");
            entity.Components.Add(biomeComponent);
            
            return biomeComponent;
        }
    }
}