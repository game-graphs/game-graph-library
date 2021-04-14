using System;

namespace GDT.Test.Tests.WFC
{
    [Flags]
    public enum TileSockets : int
    {
        None = 0,
        Air = 1 << 0,
        Ground = 1 << 1,
        Soil = 1 << 2,
        Foliage = 1 << 3,
        InnerGroundLeft = 1 << 4,
        InnerGroundRight = 1 << 5,
        
        Everything = Int32.MaxValue
    }
}