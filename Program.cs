using System;

using PerlinNoise.Engine;

namespace PerlinNoise
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(640, 640);
            game.Run();
        }
    }
}
