using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModpackCalculator
{
    public static class ExtensionMethods
    {
        public static bool HasFlags(this Enum testThing, params Enum[] flags)
        {
            foreach (var flag in flags)
                if (!testThing.HasFlag(flag))
                    return false;
            return true;
        }
    }
}
