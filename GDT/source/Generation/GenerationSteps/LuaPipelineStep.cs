#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDT.Model;
using NLua;

namespace GDT.Generation.GenerationSteps
{
    public class LuaPipelineStep : IPipelineStep
    {
        public string Name { get; }
        private readonly string _luaCode;
        private readonly string _luaFunctionName;
        private readonly List<string> _luaCSharpImports;

        /// <summary>
        /// Executes a Lua function with the signature 'ExecuteStep(graph)'
        /// </summary>
        /// <param name="luaCode">The code that contains the function</param>
        /// <param name="luaFunctionName">[Optional] Function name that takes a Graph as it's only parameter</param>
        /// <param name="additionalImports">[Optional] A list of namespaces that is required by the Lua code</param>
        /// <param name="name">[Optional] Name of this step</param>
        public LuaPipelineStep(string luaCode, string luaFunctionName = "ExecuteStep", List<string> additionalImports = null, string name = "Lua Step")
        {
            Name = name;
            _luaCode = luaCode;
            _luaFunctionName = luaFunctionName;
            _luaCSharpImports = new List<string>()
            {
                "GDT", "GDT.Model"
            };
            
            if (additionalImports != null) _luaCSharpImports.AddRange(additionalImports);
        }
        
        public Graph ExecuteStep(Graph graph)
        {
            using Lua lua = new Lua();
            lua.State.Encoding = Encoding.UTF8;
            lua.LoadCLRPackage();
            string imports = _luaCSharpImports.Select(s => $"'{s}'").Aggregate((s1, s2) => $"{s1}, {s2}");
            lua.DoString($"import({imports})");
            lua.DoString(_luaCode);
            var executeStepFunc = lua[_luaFunctionName] as LuaFunction;
            executeStepFunc?.Call(graph);

            return graph;
        }
    }
}