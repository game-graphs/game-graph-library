using System;
using System.Collections.Generic;

namespace GDT.Utility
{
    public sealed class LuaHelper
    {
        public static Tuple<int, int> CreateIntTuple(int item1, int item2)
        {
            return new(item1, item2);
        }

        public static List<Tuple<int, int>> CreateIntTupleList()
        {
            return new();
        }
    }
}