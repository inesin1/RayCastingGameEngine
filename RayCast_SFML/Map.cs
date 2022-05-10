using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCast_SFML
{
    internal class Map
    {
        //Поля
        public int Width;
        public int Height;
        public int BlockSize;

        public int[,] Bitmap;

        public Map(int[,] bitmap, int blockSize = 16)
        {
            Bitmap = bitmap;
            Width = bitmap.GetLength(1);
            Height = bitmap.GetLength(0);
            BlockSize = blockSize;
        }
    }
}
