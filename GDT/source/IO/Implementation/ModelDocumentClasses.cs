using System;
using System.Collections.Generic;
using SharpYaml.Serialization;

namespace GDT.IO.Implementation
{
    public class GraphDocument
    {
        [YamlMember(0)] public Guid id { get; set; }
        [YamlMember(1)] public string name { get; set; }
        [YamlMember(2)] public List<EntityDocument> entities { get; set; }
        [YamlMember(3)] public List<LayerDocument> layers { get; set; }
    }

    public class EntityDocument
    {
        [YamlMember(0)] public Guid id { get; set; }
        [YamlMember(1)] public string name { get; set; }
        [YamlMember(2)] public List<EntityDocument> children { get; set; }
        [YamlMember(3)] public List<ComponentDocument> components { get; set; }
    }

    public class ComponentDocument
    {
        [YamlMember(0)] public Guid id { get; set; } 
        [YamlMember(1)] public string name { get; set; }
        [YamlMember(2)] public List<PropertyDocument> attributes { get; set; }
    }
    
    public class PropertyDocument
    {
        [YamlMember(1)] public string name { get; set; }
        [YamlMember(2)] public object value { get; set; }
        [YamlMember(3)] public string type_name { get; set; }
        [YamlMember(4)] public string type_assembly { get; set; }
    }
    
    public class LayerDocument
    {
        [YamlMember(0)] public Guid id { get; set; }
        [YamlMember(1)] public string name { get; set; }
        [YamlMember(2)] public List<RelationDocument> relations { get; set; }
    }
    
    public class RelationDocument
    {
        [YamlMember(0)] public Guid id { get; set; }
        [YamlMember(1)] public string name { get; set; }
        [YamlMember(2)] public List<Guid> referenced_entities { get; set; }
        [YamlMember(3)] public List<PropertyDocument> properties { get; set; }
    }
}