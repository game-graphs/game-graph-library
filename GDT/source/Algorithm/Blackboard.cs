using System;
using System.Collections.Generic;

#nullable enable

namespace GDT.Algorithm
{
    public class Blackboard
    {

        private Dictionary<string, object> _data;

        public Blackboard()
        {
            _data = new Dictionary<string, object>();
        }
        
        // CORE FUNCTIONS

        public T Get<T>(string key)
        {
            return (T) _data[key];
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _data.Add(key, value);
        }

        public void Update(string key, object value)
        {
            _data[key] = value;
        }

        /// <summary>
        /// Adds or updates the value based on the key in the blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(string key, object value)
        {
            if (HasKey(key))
            {
                Update(key, value);
            }
            else
            {
                Add(key, value);
            }
        }

        public Type? TypeOf(string key)
        {
            if (!HasKey(key)) return null;
            return _data[key].GetType();
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }
        
        
        // CONVENIENCE FUNCTIONS 
        
        public void AddOrIncrement(string key, int increment)
        {
            if (HasKey(key))
                Increment(key, increment);
            else
                Add(key, increment);
        }
        
        public void AddOrIncrement(string key, double increment)
        {
            if (HasKey(key))
                Increment(key, increment);
            else
                Add(key, increment);
        }

        public bool Increment(string key, int increment)
        {
            Type? type = TypeOf(key);
            if (type == null || type != typeof(int)) return false;

            int currentValue = Get<int>(key);
            Update(key, currentValue + increment);

            return true;
        }
        
        public bool Increment(string key, double increment)
        {
            Type? type = TypeOf(key);
            if (type == null || type != typeof(double)) return false;

            double currentValue = Get<double>(key);
            Update(key, currentValue + increment);

            return true;
        }


        /// <summary>
        /// Checks whether the key exists and then whether it's value is greater than 0.
        /// NOTE: Currently supports only int and double.
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <returns>true if the value behind the key is greater than 0</returns>
        public bool IsGreaterZero(string key)
        {
            if (!HasKey(key)) return false;

            if (TypeOf(key) == typeof(int))
            {
                var value = Get<int>(key);
                return value > 0;
            }
            
            if (TypeOf(key) == typeof(double))
            {
                var value = Get<double>(key);
                return value > 0;
            }
            
            Console.WriteLine($"Unsupported datatype of key '{key}': {TypeOf(key)?.Name}");
            return false;
        }

        public Blackboard Copy()
        {
            Blackboard blackboard = new Blackboard();
            foreach (var(key, value) in _data)
            {
                blackboard.Add(key, value);
            }
            
            return blackboard;
        }
    }
}
