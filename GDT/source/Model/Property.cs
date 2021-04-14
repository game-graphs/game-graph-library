using System;

namespace GDT.Model
{
    public class Property
    {
        public string Name { get; }
        public Type Type { get; }
        public object Value { get; set; }

        public Property(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
        
        public Property(string name, object value) : this(name, value.GetType(), value)
        {}
    }
}