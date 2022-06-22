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
        Raylib.InitWindow(800, 900, "A* Prototype");
        Raylib.SetTargetFPS(60);

        var tileset = new Map.Tileset();
        tileset.SetSprite('.', "assets/grass.png");
        tileset.SetSprite('_', "assets/sand.png");
        tileset.SetSprite('T', "assets/forest.png");
        tileset.SetSprite('^', "assets/mountain.png");
        tileset.SetSprite('~', "assets/water.png");
        tileset.SetSprite('#', "assets/wall.png");
        tileset.SetSprite(' ', "assets/ground.png");
        tileset.SetSprite('E', "assets/entrance.png");
        tileset.SetSprite('C', "assets/cave.png");
        tileset.SetSprite('P', "assets/pendant.png");
        tileset.SetSprite('L', "assets/lostwoods.png");

        World world = new World(
            @"assets/hyrule.txt",
            new Tile(24, 27),
            new Tile(6, 5),
            new Dungeon(new Map.Tile(5, 32), new Map.Tile(14, 26), new Map.Tile(13, 3), "assets/dungeon1.txt"),
            new Dungeon(new Map.Tile(39, 17), new Map.Tile(13, 25), new Map.Tile(13, 2), "assets/dungeon2.txt"),
            new Dungeon(new Map.Tile(24, 1), new Map.Tile(14, 25), new Map.Tile(15, 19), "assets/dungeon3.txt")
        );

        var worldViewer = new WorldViewer(400,440,18,world);
        worldViewer.Tileset = tileset;

        DummySequence(worldViewer);

        var tb = new ToggleButton(350, 830, 50, 50);
        tb.OnToggle += (object? o, bool pressed) => { worldViewer.showExpandedCosts = pressed; };

        var b = new Button(50, 830, 100, 50);
        b.OnClick += (_, _) => { worldViewer.StartActionSequence(); b.active = false; };

        var mapName = new Label(400, 20, "Map", 25, Color.BLACK);
        worldViewer.currentMapLabel = mapName;

        var costsLabel = new Label(400, 45, "Costs", 20, Color.BLACK);
        worldViewer.costDisplayLabel = costsLabel;

        menuLayer.Add(tb);
        menuLayer.Add(b);
        menuLayer.Add(mapName);
        menuLayer.Add(costsLabel);
        mapLayer.Add(worldViewer);
    }

    private void DummySequence(WorldViewer wv)
    {
        wv.AddAction(() => {
            wv.linkPath = new List<Map.Tile>{new Tile(24, 27), new Tile(23,27), new Tile(22,27), new Tile(21,27), new Tile(21,26)};
            wv.isLinkWalking = true;
            }, 3.0f);

        wv.AddAction(() => {
            wv.linkWhere = 0.0f;
            wv.linkPath = new List<Map.Tile>{new Tile(24, 27), new Tile(23,27), new Tile(22,27), new Tile(21,27), new Tile(21,26)};
            wv.isLinkWalking = true;
            }, 3.0f);
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
        mapLayer.ForEach((UI ui) => {if (ui.active) ui.Update(delta);} );
        menuLayer.ForEach((UI ui) => {if (ui.active) ui.Update(delta);} );
    }

    public void Render(float delta)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);

        // Raylib.DrawRectangle((int) x, (int) y, 20, 20, Color.BLACK);

        mapLayer.ForEach((UI ui) => {if (ui.active) ui.Draw();} );
        menuLayer.ForEach((UI ui) => {if (ui.active) ui.Draw();} );

        Raylib.EndDrawing();
    }

    public void Close()
    {
        DoCleanup();
        Raylib.CloseWindow();
    }

    private void SetupActionSequence(WorldViewer wv)
    {
        // Go to first dungeon and generate path.
        wv.AddAction(() => {
            wv.ChangeMap(0);
            wv.costDisplayLabel!.Text = "Finding cost of dungeon 1...";
            wv.SetAStarPoints(wv.World.Dungeons[0].GetStartPoint(), wv.World.Dungeons[0].GetObjetive());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to second dungeon and generate path
        wv.AddAction(() => {
            wv.ChangeMap(1);
            wv.costDisplayLabel!.Text = "Finding cost of dungeon 2...";
            wv.SetAStarPoints(wv.World.Dungeons[1].GetStartPoint(), wv.World.Dungeons[1].GetObjetive());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to third dungeon and generate path
        wv.AddAction(() => {
            wv.ChangeMap(2);
            wv.costDisplayLabel!.Text = "Finding cost of dungeon 3...";
            wv.SetAStarPoints(wv.World.Dungeons[2].GetStartPoint(), wv.World.Dungeons[2].GetObjetive());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to overworld and draw path from home to dungeon 0
        wv.AddAction(() => {
            wv.ChangeMap(-1);
            wv.costDisplayLabel!.Text = "Finding cost Home -> Dungeon 1...";
            wv.SetAStarPoints(wv.World.Home, wv.World.Dungeons[0].GetOverworldPoint());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to overworld and draw path from home to dungeon 1
        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = "Finding cost Home -> Dungeon 2...";
            wv.SetAStarPoints(wv.World.Home, wv.World.Dungeons[1].GetOverworldPoint());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to overworld and draw path from home to dungeon 2
        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = "Finding cost Home -> Dungeon 3...";
            wv.SetAStarPoints(wv.World.Home, wv.World.Dungeons[2].GetOverworldPoint());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to overworld and draw path from dungeon 0 to dungeon 1
        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = "Finding cost Dungeon 1 -> Dungeon 2...";
            wv.SetAStarPoints(wv.World.Dungeons[0].GetOverworldPoint(), wv.World.Dungeons[1].GetOverworldPoint());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to overworld and draw path from dungeon 1 to dungeon 2
        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = "Finding cost Dungeon 2 -> Dungeon 3...";
            wv.SetAStarPoints(wv.World.Dungeons[1].GetOverworldPoint(), wv.World.Dungeons[2].GetOverworldPoint());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Go to overworld and draw path from dungeon 0 to dungeon 2
        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = "Finding cost Dungeon 1 -> Dungeon 3...";
            wv.SetAStarPoints(wv.World.Dungeons[1].GetOverworldPoint(), wv.World.Dungeons[2].GetOverworldPoint());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        wv.AddAction(() => {

            }, 0.0f);
    }

    private void DoCleanup()
    {
        mapLayer.ForEach((UI ui) => ui.Cleanup());
        menuLayer.ForEach((UI ui) => ui.Cleanup());
    }
}
