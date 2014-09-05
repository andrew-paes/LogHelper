using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Of
    {
        public static bool Type<T>(T item)
        {
            return item != null && typeof(T).Equals(item.GetType());
        }
    }
}
