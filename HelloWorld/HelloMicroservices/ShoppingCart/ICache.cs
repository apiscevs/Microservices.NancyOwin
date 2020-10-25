using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public interface ICache
    {
        void Add(string key, object value, TimeSpan ttl);
        object Get(string key);
    }
}
