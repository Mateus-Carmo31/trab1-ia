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

        var showCostsToggleButton = new ToggleButton(250, 830, "CUSTOS");
        showCostsToggleButton.OnToggle += (object? o, bool pressed) => { 
            worldViewer.showExpandedCosts = pressed;
        };

        Button playPauseButton = new Button(22, 830, 150, 50, "PLAY", Color.GREEN);
        playPauseButton.OnClick += (_, _) => { 
            worldViewer.PlayPauseSequence();
            playPauseButton.buttonText = worldViewer.isPlaying ? "PAUSE" : "PLAY";
            playPauseButton.buttonColor = worldViewer.isPlaying ? Color.RED : Color.GREEN;
        };

        Button slowSpeedButton = new Button(578, 830, 60, 50, "0.5X", Color.BLUE);
        Button normalSpeedButton = new Button(648, 830, 60, 50, "1X", Color.BLUE);
        Button fastSpeedButton = new Button(718, 830, 60, 50, "2X", Color.BLUE);

        var mapName = new Label(400, 20, "Map", 25, Color.BLACK);
        worldViewer.currentMapLabel = mapName;

        var costsLabel = new Label(400, 45, "Costs", 20, Color.BLACK);
        worldViewer.costDisplayLabel = costsLabel;

        menuLayer.Add(showCostsToggleButton);
        menuLayer.Add(playPauseButton);
        menuLayer.Add(slowSpeedButton);
        menuLayer.Add(normalSpeedButton);
        menuLayer.Add(fastSpeedButton);
        menuLayer.Add(mapName);
        menuLayer.Add(costsLabel);
        mapLayer.Add(worldViewer);
    }

    private void DummySequence(WorldViewer wv)
    {
        wv.AddAction(() => {
            wv.linkPath = new List<Map.Tile>{new Tile(24, 27), new Tile(23,27), new Tile(22,27), new Tile(21,27), new Tile(20,27), new Tile(19,27), new Tile(18,27), new Tile(17,27), new Tile(16,27), new Tile(15,27),new Tile(14,27) ,new Tile(13,27), new Tile(12,27), new Tile(11,27), new Tile(10,27),new Tile(9,27),new Tile(8,27),new Tile(7,27)};
            wv.isLinkWalking = true;
            }, 3.0f);

        wv.AddAction(() => {
            wv.linkWhere = 0.0f;
            wv.linkPath = new List<Map.Tile>{new Tile(24, 27), new Tile(23,27), new Tile(22,27), new Tile(21,27), new Tile(20,27), new Tile(19,27), new Tile(18,27), new Tile(17,27), new Tile(16,27), new Tile(15,27),new Tile(14,27) ,new Tile(13,27), new Tile(12,27), new Tile(11,27), new Tile(10,27),new Tile(9,27),new Tile(8,27),new Tile(7,27)};
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
    
    public void Update(float delta)
    {
        mapLayer.ForEach((UI ui) => {if (ui.active) ui.Update(delta);} );
        menuLayer.ForEach((UI ui) => {if (ui.active) ui.Update(delta);} );
    }

    public void Render(float delta)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);

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
