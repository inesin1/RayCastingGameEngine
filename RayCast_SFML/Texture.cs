using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace RayCast_SFML
{
    internal class Texture
    {
        //Поля
        public int Width;
        public int Height;
        public string Source;
        public Image Image;

        public Texture(string source)
        {
            Source = source;
            Image = new Image(source);
            Width = (int)Image.Size.X;
            Height = (int)Image.Size.Y;
        }
    }
}
