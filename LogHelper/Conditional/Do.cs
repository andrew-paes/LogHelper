using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Do
    {
        public static bool If(bool boolean, Action action)
        {
            Throw.IfNull(() => action);

            if (boolean)
                action();

            return boolean;
        }
    }
}
