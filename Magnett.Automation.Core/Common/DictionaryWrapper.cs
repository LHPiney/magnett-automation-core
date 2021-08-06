using System;
using System.Collections.Generic;

namespace Magnett.Automation.Core.Common
{
    public abstract class DictionaryWrapper<TItem>
    {
        private const int Zero = 0;
            
        private readonly IDictionary<CommonNamedKey, TItem> _dictionary;
        
        public int Count => _dictionary.Count;

        protected DictionaryWrapper()
        {
            _dictionary = new Dictionary<CommonNamedKey, TItem>();
        }

        protected DictionaryWrapper(IDictionary<CommonNamedKey, TItem> dictionary)
        {
            _dictionary = dictionary
                          ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public virtual void Add(CommonNamedKey key, TItem item)
        {
            _dictionary.Add(key, item);
        }

        public virtual void Set(CommonNamedKey key, TItem item)
        {
            _dictionary[key] = item;
        }
        
        public virtual TItem Get(CommonNamedKey key)
        {
            return _dictionary[key];
        }

        public virtual bool HasItem(CommonNamedKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public virtual bool IsEmpty()
        {
            return _dictionary.Count == Zero;
        }
    }
}