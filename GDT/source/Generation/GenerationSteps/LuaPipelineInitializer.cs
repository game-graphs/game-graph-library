using System;
using System.Collections.Generic;
using System.Text;
using GDT.Generation.GenerationSteps;
using GDT.Model;
using NLua;

namespace GDT.Generation.GenerationSteps
{
    public class LuaPipelineInitialization : IPipelineInitialization
    {
        public string Name { get; }
        private readonly string _luaCode;
        private readonly string _luaFunctionName;
        private readonly List<NLuaImport> _luaCSharpImports;

        /// <summary>
        /// Executes a Lua function with the signature 'Initialize()'
        /// This method is expected to return a Graph object as the first return value
        /// </summary>
        /// <param name="luaCode">The code that contains the function</param>
        /// <param name="luaFunctionName">[Optional] Function name that returns a Graph</param>
        /// <param name="additionalImports">[Optional] A list of namespaces that is required by the Lua code</param>
        /// <param name="name">[Optional] Name of this step</param>
        public LuaPipelineInitialization(string luaCode, string luaFunctionName = "Initialize", List<NLuaImport> additionalImports = null, string name = "Lua Initialization")
        {
            Name = name;
            _luaCode = luaCode;
            _luaFunctionName = luaFunctionName;
            _luaCSharpImports = new List<NLuaImport>()
            {
                new("GDT", "GDT.Model")
            };
            
            if (additionalImports != null) _luaCSharpImports.AddRange(additionalImports);
        }
        
        public Graph Initialize()
        {
            using Lua lua = new Lua();
            lua.State.Encoding = Encoding.UTF8;
            lua.LoadCLRPackage();
            foreach (var nLuaImport in _luaCSharpImports)
            {
                lua.DoString($"import({nLuaImport.ToImportString()})");
            }
            lua.DoString(_luaCode);
            
            var executeStepFunc = lua.GetFunction(_luaFunctionName);
            var returnValues = executeStepFunc?.Call();
            if(returnValues != null && returnValues[0].GetType() == typeof(Graph))
                return returnValues[0] as Graph;

            throw new ArgumentException("The provided lua code did not return a Graph as the first return value!");
        }
    }
}