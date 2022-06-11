// See https://aka.ms/new-console-template for more information
using Raylib_cs;

namespace HelloWorld
{
    static class Program
    {
        public static void Main()
        {
            Raylib.InitWindow(800, 480, "Hello World");

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                var mapStr = System.IO.File.ReadAllText(@"dungeon3.txt");

                var map = new Map(mapStr, 28, 28);

                map.DrawMap(0, 0, 10);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
