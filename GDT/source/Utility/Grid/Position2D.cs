using System;

namespace GDT.Utility.Grid
{
    public record Position2D
    {
        public int X { get; }
        public int Y { get; }

        public Position2D(int x, int y) => (X, Y) = (x, y);

        public Direction2D Delta(Position2D to)
        {
            if (X < to.X)
                return Direction2D.Right;
            if (X > to.X)
                return Direction2D.Left;
            if (Y < to.Y)
                return Direction2D.Up;
            if (Y > to.Y)
                return Direction2D.Down;

            return Direction2D.None;
        }
    }
}