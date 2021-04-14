namespace GDT.Generation.GenerationSteps
{
    public readonly struct NLuaImport
    {
        private readonly string _assemblyName;
        private readonly string _namespaceName;

        public NLuaImport(string assemblyName, string namespaceName)
        {
            _assemblyName = assemblyName;
            _namespaceName = namespaceName;
        }

        public NLuaImport(string namespaceName) : this("", namespaceName)
        {}

        public string ToImportString()
        {
            if (_assemblyName.Trim().Length > 0)
            {
                return $"'{_assemblyName}', '{_namespaceName}'";
            }
            
            return $"'{_namespaceName}'";
        }
    }
}