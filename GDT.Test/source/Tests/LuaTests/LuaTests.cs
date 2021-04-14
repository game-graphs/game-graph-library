using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDT.Model;
using GDT.Utility.Visualization;
using NLua;
using NUnit.Framework;

namespace GDT.Test.Tests.LuaTests
{
    public class LuaTests
    {
        [Test]
        public void LuaAddNodeToGraphTest()
        {
            Graph graph = new Graph("World Graph");

            using (Lua luaState = new Lua())
            {
                luaState.State.Encoding = Encoding.UTF8;
                luaState.LoadCLRPackage();
                luaState.DoString(@"import('GDT', 'GDT.Model')");
                luaState["graph"] = graph;
                luaState.DoString(@"local entity = Entity('Lua Node', graph)
                                        graph.Entities:Add(entity)");
            }

            Assert.AreEqual(1, graph.Entities.Count, "Expected one node added using Lua");
            Assert.AreEqual("Lua Node", graph.Entities[0]?.Name, "Expected the node to be named 'Lua Node'");
        }
        
        [Test]
        public void LuaAddNodeToGraphViaFunctionTest()
        {
            Graph graph = new Graph("World Graph");

            using (Lua luaState = new Lua())
            {
                luaState.State.Encoding = Encoding.UTF8;
                luaState.LoadCLRPackage();
                luaState.DoString(@"import('GDT', 'GDT.Model')");
                luaState.DoString(@"
                        function ExecuteStep(graph)
                            local entity = Entity('Lua Node', graph)
                            graph.Entities:Add(entity)
                        end
                        ");
                var executeStepFunc = luaState["ExecuteStep"] as LuaFunction;
                executeStepFunc?.Call(graph);
            }

            Assert.AreEqual(1, graph.Entities.Count, "Expected one node added using Lua");
            Assert.AreEqual("Lua Node", graph.Entities[0]?.Name, "Expected the node to be named 'Lua Node'");
        }
        
        [Test]
        public void LuaAddNodeToGraphViaFunctionFromFileTest()
        {
            Graph graph = new Graph("World Graph");

            using (Lua luaState = new Lua())
            {
                luaState.State.Encoding = Encoding.UTF8;
                luaState.LoadCLRPackage();
                luaState.DoString(@"import('GDT', 'GDT.Model')");
                luaState.DoString(FileUtilities.LoadFile("pipeline_test.lua"));
                var executeStepFunc = luaState["ExecuteStep"] as LuaFunction;
                executeStepFunc?.Call(graph);
            }

            Assert.AreEqual(1, graph.Entities.Count, "Expected one node added using Lua");
            Assert.AreEqual("Lua Test Node", graph.Entities[0]?.Name, "Expected the node to be named 'Lua Node'");
        }
        
        [Test]
        public void LuaCreateGraphViaFunctionTest()
        {
            using Lua luaState = new Lua();
            luaState.State.Encoding = Encoding.UTF8;
            luaState.LoadCLRPackage();
            luaState.DoString(@"import('GDT', 'GDT.Model')");
            luaState.DoString(@"
                        function Initialize()
                            local graph = Graph('Lua Graph')
                            return graph
                        end
                        ");
            var executeStepFunc = luaState.GetFunction("Initialize");
            var returnValues = executeStepFunc?.Call();
            Assert.AreEqual(1, returnValues?.Length, "Expected 1 return value (the graph)");
            Assert.AreSame(typeof(Graph), returnValues?[0].GetType(), "Expected the type to be Graph");
            var graph = returnValues?[0] as Graph;
            Assert.AreEqual("Lua Graph", graph?.Name, "Expected one node added using Lua");
        }
        
    }
}