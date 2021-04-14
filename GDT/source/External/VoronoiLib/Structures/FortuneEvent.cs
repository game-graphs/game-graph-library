using System;

namespace GDT.Algorithm.VoronoiLib.Structures
{
    interface FortuneEvent : IComparable<FortuneEvent>
    {
        double X { get; }
        double Y { get; }
    }
}
