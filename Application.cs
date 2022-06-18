// See https://aka.ms/new-console-template for more information
using Raylib_cs;

namespace MapPathfinder
{
    public class Application
    {
        Map curMap;
        AStar pathfinder;

        public void Init()
        {
            Raylib.InitWindow(1000, 800, "A* Prototype");
            var mapStr = System.IO.File.ReadAllText(@"mapa_test.txt");
            Raylib.SetTargetFPS(60);

            curMap = new Map(mapStr, 50, 35);
            pathfinder = new AStar(curMap, (0,0), (49,34));
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

        // float x = 0, y = 0, speedX = 100, speedY = 100;
        float timer = 0.02f, timerMax = 0.02f;
        bool autoStep = false, drawCurrent = true, drawCosts = false;
        AStar.State execState;
        public void Update(float delta)
        {
            // x += speedX * delta;
            // y += speedY * delta;

            // if (x + 20 >= 800 || x < 0)
            // {
            //     x = speedX > 0 ? 780 : 0;
            //     speedX = -speedX;
            // }
            // if (y + 20 >= 480 || y < 0)
            // {
            //     y = speedY > 0 ? 460 : 0;
            //     speedY = -speedY;
            // }
            if (autoStep)
                timer = Math.Max(0, timer - delta);

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                autoStep = !autoStep;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_C))
                drawCosts = !drawCosts;

            if ((timer == 0 || Raylib.IsKeyPressed(KeyboardKey.KEY_N)) && (execState == AStar.State.NotStarted || execState == AStar.State.InExec))
            {
                (execState, _) = pathfinder.RunStep();
                timer = timerMax;
            }
        }

        public void Render(float delta)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            // Raylib.DrawRectangle((int) x, (int) y, 20, 20, Color.BLACK);

            curMap.DrawMap(0, 0, 20);
            pathfinder.DrawOverlay(0,0,20, drawCurrent, drawCosts);

            Raylib.EndDrawing();
        }

        public void Close()
        {
            Raylib.CloseWindow();
        }
    }
}
