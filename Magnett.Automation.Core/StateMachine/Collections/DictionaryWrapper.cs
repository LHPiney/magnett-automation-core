using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Magnett.Automation.Core.StateMachine.Collections
{
    public class DictionaryWrapper<TItem>
    {
        private readonly IDictionary<string, TItem> _values;

        public IReadOnlyList<TItem> Values => _values.Values.ToList().AsReadOnly();
        
        public int Count => _values.Count;

        protected DictionaryWrapper()
        {
            _values = new Dictionary<string, TItem>();
        }

        protected DictionaryWrapper(IDictionary<string, TItem> values)
        {
            _values = values 
                      ?? throw new ArgumentNullException(nameof(values));
        }

        public void Add(string key, TItem item)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            _values.Add(key, item);
        }

        public TItem Get(string key)
        {
            return _values[key];
        }

        public bool HasItem(string key)
        {
            return _values.ContainsKey(key);
        }
    }
}