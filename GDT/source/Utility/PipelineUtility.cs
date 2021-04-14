using System.Collections.Generic;
using GDT.Generation;
using GDT.Generation.GenerationSteps;
using GDT.Model;

namespace GDT.Utility
{
    public static class PipelineUtility
    {
        public static Pipeline CreateEmptyPipelineWithYamlGraph(string fileContent)
        {
            return new(new YamlGraphPipelineInitializer(fileContent));
        }
        
        public static Pipeline CreateEmptyPipelineWithLuaGraph(string fileContent)
        {
            return new(new LuaPipelineInitialization(fileContent));
        }
        
        public static Pipeline CreateEmptyPipelineWithLuaGraph(string fileContent, List<NLuaImport> additionalImports)
        {
            return new(new LuaPipelineInitialization(fileContent, additionalImports: additionalImports));
        }
        
        public static Pipeline CreateEmptyPipelineWithLuaGraph(string fileContent, string initializationMethodName)
        {
            return new(new LuaPipelineInitialization(fileContent, initializationMethodName));
        }
        
        public static Pipeline CreateEmptyPipelineUsingGraph(string graphName)
        {
            return new(new GraphPipelineInitializer(graphName));
        }
        
        public static Pipeline CreateEmptyPipelineUsingGraph(Graph graph)
        {
            return new(new GraphPipelineInitializer(graph));
        }
    }
}