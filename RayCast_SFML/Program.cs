using System;

namespace RayCast_SFML
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Game game = new Game("Raycast", 640, 480);
            game.Run();
        }
    }
}
