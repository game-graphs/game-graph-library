using GDT.IO.Implementation;
using GDT.Model;

namespace GDT.Generation.GenerationSteps
{
    public class YamlGraphPipelineInitializer : IPipelineInitialization
    {
        public string Name { get; }
        private readonly string _fileContent;

        public YamlGraphPipelineInitializer(string fileContent)
        {
            _fileContent = fileContent;
            Name = $"Yaml Initializer ({_fileContent})";
        }
        public Graph Initialize()
        {
            return YamlLoader.LoadGraph(_fileContent);
        }
    }
}