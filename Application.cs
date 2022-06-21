namespace MapPathfinder;
using Raylib_cs;

using Tile = Map.Tile;
using Dungeon = World.Dungeon;

public class Application
{
    List<UI> menuLayer = new List<UI>();
    List<UI> mapLayer = new List<UI>();

    public void Init()
    {
        Raylib.InitWindow(1000, 800, "A* Prototype");
        Raylib.SetTargetFPS(60);

        var tileset = new Map.Tileset();
        tileset.SetSprite('.', "assets/grass.png");
        tileset.SetSprite('_', "assets/sand.png");
        tileset.SetSprite('T', "assets/forest.png");
        tileset.SetSprite('^', "assets/mountain.png");
        tileset.SetSprite('~', "assets/water.png");
        tileset.SetSprite('#', "assets/wall.png");
        tileset.SetSprite(' ', "assets/ground.png");

        World world = new World(
            @"hyrule.txt",
            new Tile(24, 27),
            new Tile(6, 5),
            new Dungeon(new Map.Tile(5, 32), new Map.Tile(14, 26), new Map.Tile(13, 3), "dungeon1.txt"),
            new Dungeon(new Map.Tile(39, 17), new Map.Tile(13, 25), new Map.Tile(13, 2), "dungeon2.txt"),
            new Dungeon(new Map.Tile(24, 1), new Map.Tile(14, 25), new Map.Tile(15, 19), "dungeon3.txt")
        );

        var worldViewer = new WorldViewer(0,0,16, world);
        worldViewer.Tileset = tileset;
        worldViewer.SetAStarPoints((24,27), (5,32));
        worldViewer.actionSequence.Enqueue(() => {
            worldViewer.ChangeMap(0);
            worldViewer.SetAStarPoints(world.GetDungeon(0).GetStartPoint().GetTuple(), world.GetDungeon(0).GetObjetive().GetTuple());
            });

        var tb = new ToggleButton(700, 300, 50, 50);
        tb.OnToggle += (object? o, bool pressed) => { worldViewer.showExpandedTiles = pressed; };

        var b = new Button(700, 200, 100, 50);
        b.OnClick += (_, _) => { worldViewer.isPlaying = !worldViewer.isPlaying; };

        menuLayer.Add(tb);
        menuLayer.Add(b);
        mapLayer.Add(worldViewer);
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
