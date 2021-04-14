using System;
using System.Collections.Generic;

namespace GDT.Utility.Grid
{
    public class Grid2D<T>
    {
        public int Width { get; }
        public int Height { get; }
        public int Count => _data.Count;

        private List<T> _data;

        public Grid2D(int width, int height, Func<int, int, T> constructor)
        {
            Width = width;
            Height = height;
            _data = new List<T>();
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _data.Add(constructor.Invoke(x, y));
                }
            } 
            
        }

        public T Get(int x, int y)
        {
            return _data[Index(x, y)];
        }
        
        public int Index(int x, int y)
        {
            #if DEBUG
            if (!IsValid(x, y)) throw new ArgumentOutOfRangeException($"Invalid position: X={x}, Y={y} (Index={x + y * Width})");
            #endif
            
            return x + y * Width;
        }

        public (int x, int y) GetPosition(int index)
        {
            #if DEBUG
            if (index < 0 || index >= Width * Height) throw new ArgumentException($"index: {index} - should be between 0 and {Width * Height - 1}");
            #endif
            return (index % Width, index/Width);
        }

        public bool IsValid(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<T> GetCells()
        {
            return _data;
        }
        
        public List<T> GetCellsAsList()
        {
            return _data;
        }

        public int IndexOf(T element)
        {
            return _data.IndexOf(element);
        }

        public int FindIndex(Predicate<T> match, int startIndex = 0)
        {
            return _data.FindIndex(startIndex, match);
        }

        public void Replace(T newObject, int index)
        {
            _data[index] = newObject;
        }
    }
}