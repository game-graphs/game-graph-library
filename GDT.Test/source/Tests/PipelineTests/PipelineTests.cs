using GDT.Generation;
using GDT.Generation.GenerationSteps;
using GDT.Model;
using GDT.Utility.Visualization;
using GDT.Utility;
using NUnit.Framework;

namespace GDT.Test.Tests.PipelineTests
{
    public class PipelineTests
    {
        [Test]
        public void LuaPipelineTest()
        {
            string luaScript = FileUtilities.LoadFile("pipeline_test.lua");
            
            Pipeline pipeline = PipelineUtility.CreateEmptyPipelineWithLuaGraph(luaScript);
            pipeline.AddStep(new LuaPipelineStep(luaScript));

            Graph resultGraph = pipeline.Execute();
            
            Assert.NotNull(resultGraph, "There should be a graph object");
            Assert.AreEqual("Lua Test Graph", resultGraph.Name, "Expected the graph to be called 'Lua Test Graph'");
            Assert.AreEqual("Lua Test Node", resultGraph.Entities[0].Name, "Expected the graph to have a child node named 'Lua Test Node'");
        }
        
        [Test]
        public void LuaPipeline_WithAlternativeFunctionNamesTest()
        {
            string luaScript = FileUtilities.LoadFile("pipeline_test.lua");
            
            Pipeline pipeline = new Pipeline(new LuaPipelineInitialization(luaScript, "InitializeAlternative"));
            pipeline.AddStep(new LuaPipelineStep(luaScript, "ExecuteStepAlternative"));

            Graph resultGraph = pipeline.Execute();
            
            Assert.NotNull(resultGraph, "There should be a graph object");
            Assert.AreEqual("Alternative Lua Test Graph", resultGraph.Name, "Expected the graph to be called 'Lua Test Graph'");
            Assert.AreEqual("Alternative Lua Test Node", resultGraph.Entities[0].Name, "Expected the graph to have a child node named 'Lua Test Node'");
        }
    }
}