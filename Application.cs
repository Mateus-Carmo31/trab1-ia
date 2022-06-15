// See https://aka.ms/new-console-template for more information
using Raylib_cs;

namespace MapPathfinder
{
    public class Application
    {
        Map curMap;

        public void Init()
        {
            Raylib.InitWindow(800, 480, "Hello World");
            var mapStr = System.IO.File.ReadAllText(@"dungeon3.txt");
            curMap = new Map(mapStr, 28, 28);
            Raylib.SetTargetFPS(60);
        }

        public bool isRunning()
        {
            return !Raylib.WindowShouldClose();
        }

        public void MainLoop()
        {
            var dt = Raylib.GetFrameTime();
            Update(dt);
            Render(dt);
        }

        float x = 0, y = 0, speedX = 100, speedY = 100;
        public void Update(float delta)
        {
            x += speedX * delta;
            y += speedY * delta;

            if (x + 20 >= 800 || x < 0)
            {
                x = speedX > 0 ? 780 : 0;
                speedX = -speedX;
            }
            if (y + 20 >= 480 || y < 0)
            {
                y = speedY > 0 ? 460 : 0;
                speedY = -speedY;
            }
        }

        public void Render(float delta)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            Raylib.DrawRectangle((int) x, (int) y, 20, 20, Color.BLACK);

            // curMap.DrawMap(0, 0, 10);

            Raylib.EndDrawing();
        }

        public void Close()
        {
            Raylib.CloseWindow();
        }
    }
}
