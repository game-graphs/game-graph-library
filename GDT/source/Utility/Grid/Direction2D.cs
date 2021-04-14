using System;

namespace GDT.Utility.Grid
{
    public enum Direction2D : int
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    public static class Direction2DMethods
    {
        public static Direction2D Opposite(this Direction2D dir)
        {
            switch (dir)
            {
                case Direction2D.Down: return Direction2D.Up;
                case Direction2D.Up: return Direction2D.Down;
                case Direction2D.Left: return Direction2D.Right;
                case Direction2D.Right: return Direction2D.Left;
            }

            throw new ArgumentException($"Invalid direction: {dir}");
        }  
        
        public static int Int(this Direction2D dir)
        {
            return (int) dir;
        }
    }

}