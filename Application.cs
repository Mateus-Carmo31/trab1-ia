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
        tileset.SetSprite('H', "assets/house.png");

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
        var showCostsToggleButton = new ToggleButton(250, 830, "CUSTOS");
        Button startButton = new Button(22, 830, 150, 50, "START", Colors.green);
        Button playPauseButton = new Button(22, 830, 150, 50, "PAUSE", Colors.red);
        Button restartButton = new Button(180, 830, 50, 50, "R", Colors.red);
        Button slowSpeedButton = new Button(578, 830, 60, 50, "0.5X", Colors.lightBlue);
        Button normalSpeedButton = new Button(648, 830, 60, 50, "1X", Colors.middleBlue);
        Button fastSpeedButton = new Button(718, 830, 60, 50, "2X", Colors.lightBlue);
        var mapName = new Label(400, 20, "Map", 25, Color.BLACK);
        var costsLabel = new Label(400, 45, "", 20, Color.BLACK);

        SetupActionSequence(worldViewer);

        worldViewer.currentMapLabel = mapName;
        worldViewer.costDisplayLabel = costsLabel;
        worldViewer.OnSequenceFinished += (_,_) => {
            playPauseButton.active = false;
            startButton.active = false;
            };

        showCostsToggleButton.OnToggle += (object? o, bool pressed) => {
            worldViewer.showExpandedCosts = pressed;
        };

        playPauseButton.active = false;
        playPauseButton.OnClick += (_, _) => {
            worldViewer.isPlaying = !worldViewer.isPlaying;
            playPauseButton.buttonText = worldViewer.isPlaying ? "PAUSE" : "PLAY";
            playPauseButton.buttonColor = worldViewer.isPlaying ? Colors.red : Colors.green;
        };

        startButton.OnClick += (_, _) => {
            startButton.active = false;
            playPauseButton.active = true;
            worldViewer.StartActionSequence();
        };

        restartButton.OnClick += (_,_) => {
            World w = new World(
                @"assets/hyrule.txt",
                new Tile(24, 27),
                new Tile(6, 5),
                new Dungeon(new Map.Tile(5, 32), new Map.Tile(14, 26), new Map.Tile(13, 3), "assets/dungeon1.txt"),
                new Dungeon(new Map.Tile(39, 17), new Map.Tile(13, 25), new Map.Tile(13, 2), "assets/dungeon2.txt"),
                new Dungeon(new Map.Tile(24, 1), new Map.Tile(14, 25), new Map.Tile(15, 19), "assets/dungeon3.txt")
            );

            startButton.active = true;
            playPauseButton.active = false;
            worldViewer.Reset(w);
            SetupActionSequence(worldViewer);
        };

        slowSpeedButton.OnClick += (_, _) => {
            worldViewer.stepTime = WorldViewer.slow;
            slowSpeedButton.buttonColor = Colors.middleBlue;
            normalSpeedButton.buttonColor = Colors.lightBlue;
            fastSpeedButton.buttonColor = Colors.lightBlue;
        };

        normalSpeedButton.OnClick += (_, _) => {
            worldViewer.stepTime = WorldViewer.normal;
            slowSpeedButton.buttonColor = Colors.lightBlue;
            normalSpeedButton.buttonColor = Colors.middleBlue;
            fastSpeedButton.buttonColor = Colors.lightBlue;
        };

        fastSpeedButton.OnClick += (_, _) => {
            worldViewer.stepTime = WorldViewer.fast;
            slowSpeedButton.buttonColor = Colors.lightBlue;
            normalSpeedButton.buttonColor = Colors.lightBlue;
            fastSpeedButton.buttonColor = Colors.middleBlue;
        };

        menuLayer.Add(showCostsToggleButton);
        menuLayer.Add(playPauseButton);
        menuLayer.Add(startButton);
        menuLayer.Add(restartButton);
        menuLayer.Add(slowSpeedButton);
        menuLayer.Add(normalSpeedButton);
        menuLayer.Add(fastSpeedButton);
        menuLayer.Add(mapName);
        menuLayer.Add(costsLabel);
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

    // This adds a series of actions to the WorldViewer
    // Namely:
    // - Shows the AStar finding the best path in each dungeon
    // - Shows the AStar finding the best path between each dungeon in the overworld
    // - Shows Link moving from place to place.
    // in this order.
    private void SetupActionSequence(WorldViewer wv)
    {
        // Go to first dungeon and generate path.
        wv.AddAction(() => {
            wv.ChangeMap(0);
            wv.costDisplayLabel!.Text = "Finding cost of dungeon 1...";
            wv.SetAStarPoints(wv.World.Dungeons[0].GetStartPoint(), wv.World.Dungeons[0].GetObjetive());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost} x 2";
            }, 3.0f);

        // Go to second dungeon and generate path
        wv.AddAction(() => {
            wv.ChangeMap(1);
            wv.costDisplayLabel!.Text = "Finding cost of dungeon 2...";
            wv.SetAStarPoints(wv.World.Dungeons[1].GetStartPoint(), wv.World.Dungeons[1].GetObjetive());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost} x 2";
            }, 3.0f);

        // Go to third dungeon and generate path
        wv.AddAction(() => {
            wv.ChangeMap(2);
            wv.costDisplayLabel!.Text = "Finding cost of dungeon 3...";
            wv.SetAStarPoints(wv.World.Dungeons[2].GetStartPoint(), wv.World.Dungeons[2].GetObjetive());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost} x 2";
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
            wv.SetAStarPoints(wv.World.Dungeons[0].GetOverworldPoint(), wv.World.Dungeons[2].GetOverworldPoint());
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Draw path from Link's House to Lost Woods
        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = "Finding cost Link's Home -> Lost Woods...";
            wv.SetAStarPoints(wv.World.Home, wv.World.LostWoods);
            }, 0.0f);

        wv.AddAction(() => {
            wv.costDisplayLabel!.Text = $"Cost of path: {wv.FinalPath?.Cost}";
            }, 3.0f);

        // Show Link travelling around the dungeons!
        var (bestOrder, bestCost) = wv.World.FindBestPath();

        wv.AddAction(() => {
            wv.ChangeMap(-1);
            wv.isLinkWalking = true;
            wv.costDisplayLabel!.Text = $"Best path = Dungeon {bestOrder[0]+1} -> Dungeon {bestOrder[1]+1} -> Dungeon {bestOrder[2]+1} (Total cost: {bestCost})";
            wv.linkPath = AStar.FindPath(wv.World.Overworld, wv.World.Home, wv.World.Dungeons[bestOrder[0]].GetOverworldPoint())!.tiles;
            wv.linkWhere = 0;
            }, 2.0f);

        wv.AddAction(() => {
            wv.showPendant = true;
            wv.ChangeMap(bestOrder[0]);
            wv.linkPath = wv.World.Dungeons[bestOrder[0]].crossingPath!.tiles;
            wv.linkWhere = 0;
            }, 4.0f);

        wv.AddAction(() => {
            wv.showPendant = false;
            wv.linkPath!.Reverse();
            wv.linkWhere = 0;
            }, 4.0f);

        wv.AddAction(() => {
            wv.ChangeMap(-1);
            wv.linkPath = AStar.FindPath(wv.World.Overworld, wv.World.Dungeons[bestOrder[0]].GetOverworldPoint(), wv.World.Dungeons[bestOrder[1]].GetOverworldPoint())!.tiles;
            wv.linkWhere = 0;
            }, 3.0f);

        wv.AddAction(() => {
            wv.showPendant = true;
            wv.ChangeMap(bestOrder[1]);
            wv.linkPath = wv.World.Dungeons[bestOrder[1]].crossingPath!.tiles;
            wv.linkWhere = 0;
            }, 3.0f);

        wv.AddAction(() => {
            wv.showPendant = false;
            wv.linkPath!.Reverse();
            wv.linkWhere = 0;
            }, 3.0f);

        wv.AddAction(() => {
            wv.ChangeMap(-1);
            wv.linkPath = AStar.FindPath(wv.World.Overworld, wv.World.Dungeons[bestOrder[1]].GetOverworldPoint(), wv.World.Dungeons[bestOrder[2]].GetOverworldPoint())!.tiles;
            wv.linkWhere = 0;
            }, 3.0f);

        wv.AddAction(() => {
            wv.showPendant = true;
            wv.ChangeMap(bestOrder[2]);
            wv.linkPath = wv.World.Dungeons[bestOrder[2]].crossingPath!.tiles;
            wv.linkWhere = 0;
            }, 3.0f);

        wv.AddAction(() => {
            wv.showPendant = false;
            wv.linkPath!.Reverse();
            wv.linkWhere = 0;
            }, 3.0f);

        wv.AddAction(() => {
            wv.ChangeMap(-1);
            wv.linkPath = AStar.FindPath(wv.World.Overworld, wv.World.Dungeons[bestOrder[2]].GetOverworldPoint(), wv.World.Home)!.tiles;
            wv.linkWhere = 0;
            }, 3.0f);

        wv.AddAction(() => {
            wv.linkPath = AStar.FindPath(wv.World.Overworld, wv.World.Home, wv.World.LostWoods)!.tiles;
            wv.linkWhere = 0;
            }, 3.0f);
    }

    private void DoCleanup()
    {
        mapLayer.ForEach((UI ui) => ui.Cleanup());
        menuLayer.ForEach((UI ui) => ui.Cleanup());
    }
}
