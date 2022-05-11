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
        public int Width; //Ширина
        public int Height; //Высота
        public string Filename; //Название файла
        public Image Image; //Объект картинки для текстуры

        public Texture(string filename)
        {
            Filename = filename;
            Image = new Image(Game.AssetsFolder + "textures\\" + filename);
            Width = (int)Image.Size.X;
            Height = (int)Image.Size.Y;
        }
    }
}
