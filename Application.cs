// See https://aka.ms/new-console-template for more information
using Raylib_cs;

namespace MapPathfinder
{
    public class Application
    {
        Map curMap;
        List<UI> menuLayer = new List<UI>();
        List<UI> mapLayer = new List<UI>();

        public void Init()
        {
            Raylib.InitWindow(1000, 800, "A* Prototype");
            var mapStr = System.IO.File.ReadAllText(@"hyrule.txt");
            Raylib.SetTargetFPS(60);

            curMap = new Map(mapStr, 42, 42);
            var tileset = new Map.Tileset();
            tileset.SetSprite('.', "assets/grass.png");
            tileset.SetSprite('_', "assets/sand.png");
            tileset.SetSprite('T', "assets/forest.png");
            tileset.SetSprite('^', "assets/mountain.png");
            tileset.SetSprite('~', "assets/water.png");
            tileset.SetSprite('#', "assets/wall.png");
            tileset.SetSprite(' ', "assets/ground.png");

            var mapViewer = new MapViewer(0,0, 16);
            mapViewer.Map = curMap;
            mapViewer.Tileset = tileset;
            mapViewer.SetAStarPoints((24,27), (5,32));

            var tb = new ToggleButton(700, 300, 50, 50);
            tb.OnToggle += (object? o, bool pressed) => { mapViewer.showExpandedTiles = pressed; };

            var b = new Button(700, 200, 100, 50);
            b.OnClick += (_, _) => {};
            // pathfinder = new AStar(curMap, (0,0), (49,34));

            menuLayer.Add(tb);
            menuLayer.Add(b);
            mapLayer.Add(mapViewer);
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
        // float timer = 0.02f, timerMax = 0.02f;
        // bool autoStep = false, drawCurrent = true, drawCosts = false;
        // AStar.State execState;
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

            // if (autoStep)
            //     timer = Math.Max(0, timer - delta);

            // if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
            //     autoStep = !autoStep;

            // if (Raylib.IsKeyPressed(KeyboardKey.KEY_C))
            //     drawCosts = !drawCosts;

            // if ((timer == 0 || Raylib.IsKeyPressed(KeyboardKey.KEY_N)) && (execState == AStar.State.NotStarted || execState == AStar.State.InExec))
            // {
            //     (execState, _) = pathfinder.RunStep();
            //     timer = timerMax;
            // }
            mapLayer.ForEach((UI ui) => ui.Update(delta));
            menuLayer.ForEach((UI ui) => ui.Update(delta));
        }

        public void Render(float delta)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            // Raylib.DrawRectangle((int) x, (int) y, 20, 20, Color.BLACK);

            mapLayer.ForEach((UI ui) => ui.Draw());
            menuLayer.ForEach((UI ui) => ui.Draw());

            Raylib.EndDrawing();
        }

        public void Close()
        {
            DoCleanup();
            Raylib.CloseWindow();
        }

        private void DoCleanup()
        {
            mapLayer.ForEach((UI ui) => ui.Cleanup());
            menuLayer.ForEach((UI ui) => ui.Cleanup());
        }
    }
}
