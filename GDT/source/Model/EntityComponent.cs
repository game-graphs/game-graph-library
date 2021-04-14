using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace GDT.Model
{
    public class EntityComponent
    {
        public Guid ID { get; }
        public string Name { get; }
        public Int64 TypeID { get; private set; }
        public Dictionary<string, Property> Properties { get; }

        public EntityComponent(Guid id, string name, Dictionary<string, Property> properties)
        {
            ID = id;
            Name = name;
            Properties = properties;
            TypeID = CalculateTypeIdHash();
        }

        public EntityComponent(string name) 
            : this(Guid.NewGuid(), name, new Dictionary<string, Property>())
        {}

        public void SetProperty(string name, object value)
        {
            if (Properties.ContainsKey(name))
            {
                Properties[name].Value = value;
            }
            else
            {
                Properties.Add(name, new Property(name, value));
                TypeID = CalculateTypeIdHash();
            }
        }

        public T Get<T>(string propertyName)
        {
            Property property = Properties[propertyName];
            
            #if DEBUG
            if (!property.Type.Equals(typeof(T)))
            {
                throw new ArgumentException($"{property.Type} does not match the requested type {typeof(T)}");
            }
            #endif
            
            return (T) property.Value;
        }
        
        private Int64 CalculateTypeIdHash()
        {
            string strText = Name;
            if (Properties.Count > 0)
            {
                var keys = Properties.Keys.ToList();
                keys.Sort();
                strText += keys.Aggregate((i, j) => i + j);
            }
            
            Int64 hashCode = 0;
            if (!string.IsNullOrEmpty(strText))
            {
                //Unicode Encode Covering all characterset
                byte[] byteContents = Encoding.Unicode.GetBytes(strText);
                System.Security.Cryptography.SHA256 hash =
                    new System.Security.Cryptography.SHA256CryptoServiceProvider();
                byte[] hashText = hash.ComputeHash(byteContents);
                //32Byte hashText separate
                //hashCodeStart = 0~7  8Byte
                Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
                //hashCodeMedium = 8~23  8Byte
                Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 8);
                //hashCodeEnd = 24~31  8Byte
                Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 24);
                //and Fold
                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }
            return hashCode;
        }

        public EntityComponent Clone()
        {
            EntityComponent component = new EntityComponent(ID, Name, new Dictionary<string, Property>());

            foreach (var property in Properties.Values)
            {
                component.SetProperty(property.Name, property.Value);
            }

            return component;
        }

        public EntityComponent CreateDuplicate()
        {
            EntityComponent component = new EntityComponent(Guid.NewGuid(), Name, new Dictionary<string, Property>());

            foreach (var property in Properties.Values)
            {
                component.SetProperty(property.Name, property.Value);
            }

            return component;
        }

        public override string ToString()
        {
            return $"{Name} ({Properties.Keys.Aggregate((s1, s2) => s1 + ", " + s2).ToString()})";
        }
    }

}