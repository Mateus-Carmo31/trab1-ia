namespace MapPathfinder;

using Raylib_cs;
using Rectangle = Raylib_cs.Rectangle;

// Encapsulates the map display
// TODO: link sprite moving over map when path is found and transitioning between maps
public class WorldViewer : UI
{
    // References
    private World world;
    private int currentMapID = -1;
    private AStar pathfinder;
    private Map.Tileset? tileset = null;
    private Sprite link;

    // References to other UI elements
    public Label? currentMapLabel;
    public Label? costDisplayLabel;

    // Visual Controls
    private float tileSize;
    public bool showPath = true;
    public bool showExpandedCosts = false;

    // A* update controls
    public bool isPlaying = false;
    public float stepTime = 0.01f;
    private float timer;

    // Sequence of actions (actions are changing map, setting AStar points, etc.)
    // Each action has an associated delay in seconds before the action happens
    public Queue<(Action action, float delay)> actionSequence = new Queue<(Action, float)>();

    private static Color expandedTileColor = new Color(255, 71, 71, 127);
    private static Color pathColor = new Color(17, 255, 0, 200);

    public WorldViewer(float x, float y, float tileSize, World world) : base(x,y)
    {
        this.world = world;
        this.pathfinder = new AStar(world.GetMapByID(currentMapID));
        this.tileSize = tileSize;
        timer = stepTime;
        link = new Sprite(world.Home.x, world.Home.y, tileSize, tileSize, "assets/link.png");
    }

    public float TileSize { get => tileSize; set => tileSize = value; }
    public Map.Tileset? Tileset { get => tileset; set => tileset = value; }
    public World World { get => world; }
    public Map.Path? FinalPath { get => pathfinder?.FinalPath; }

    public void ChangeMap(int newId, bool setupPath = false)
    {
        currentMapID = newId;
        pathfinder = new AStar(world.GetMapByID(newId));
    }

    public void SetAStarPoints((int x, int y)? newStart = null, (int x, int y)? newGoal = null)
    {
        pathfinder?.SetPoints(newStart, newGoal);
    }

    public void AddAction(Action act, float delay)
    {
        actionSequence.Enqueue((act, delay));
    }

    public void PlayPauseSequence()
    {
        isPlaying = !isPlaying;
    }

    public void StartActionSequence()
    {
        if (actionSequence.Count == 0 || isPlaying)
            return;

        isPlaying = true;
        Action action;
        (action, timer) = actionSequence.Dequeue();
        action.Invoke();
    }

    public override void Draw()
    {
        if (tileset == null)
            return;

        var map = world.GetMapByID(currentMapID);
        (float x, float y) offset = (map.sizeX * tileSize / 2, map.sizeY * tileSize / 2);

        // Draw map tiles
        for(int j = 0; j < map.sizeY; j++)
        {
            for(int i = 0; i < map.sizeX; i++)
            {
                Raylib.DrawTextureQuad(tileset.GetSprite(map[i,j]), TILING, OFFSET, new Rectangle(pos.x + i*tileSize - offset.x, pos.y + j*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);
            }
        }

        // Draw extra map objects
        // TODO: make this prettier
        if (currentMapID == -1)
        {
            // Draw lost woods entrance
            Raylib.DrawTextureQuad(tileset.GetSprite('L'), TILING, OFFSET, new Rectangle(pos.x + world.LostWoods.x*tileSize - offset.x, pos.y + world.LostWoods.y*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);

            // Draw each dungeon entrance
            foreach (var dgn in world.Dungeons)
                Raylib.DrawTextureQuad(tileset.GetSprite('E'), TILING, OFFSET, new Rectangle(pos.x + dgn.GetOverworldPoint().x*tileSize - offset.x, pos.y + dgn.GetOverworldPoint().y*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);
        }
        else
        {
            var dgn = world.Dungeons[currentMapID];
            // Draw dgn entrance
            Raylib.DrawTextureQuad(
                tileset.GetSprite('C'),
                TILING, OFFSET,
                new Rectangle(pos.x + dgn.GetStartPoint().x*tileSize - offset.x, pos.y + dgn.GetStartPoint().y*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);
            // Draw pendant
            Raylib.DrawTextureQuad(
                tileset.GetSprite('P'),
                TILING, OFFSET,
                new Rectangle(pos.x + dgn.GetObjetive().x*tileSize - offset.x, pos.y + dgn.GetObjetive().y*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);
        }

        // Draw expanded tiles over map
        var costsSoFar = pathfinder?.GetCostsSoFar();
        costsSoFar?.Keys.ToList().ForEach((Map.Tile t) => {
                    Raylib.DrawRectangle((int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) tileSize, (int) tileSize, expandedTileColor);
                    if (costsSoFar[t] != int.MaxValue && showExpandedCosts)
                        Raylib.DrawText($"{costsSoFar[t]}", (int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) (tileSize / 5), Color.BLACK);
            });

        // Draws final path (when found)
        if (showPath)
        {
            pathfinder?.FinalPath?.tiles.ForEach((Map.Tile t) => {
                    Raylib.DrawRectangle((int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) tileSize, (int) tileSize, pathColor);
                });
        }

        // Draw link sprite
        link.Draw();
    }

    public List<Map.Tile>? linkPath;
    public bool isLinkWalking = false;
    public float linkWhere = 0.0f;
    public override void Update(float delta)
    {
        // if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
        //     showPath = !showPath;

        // if (Raylib.IsKeyPressed(KeyboardKey.KEY_C))
        //     showExpandedTiles = !showExpandedTiles;

        if (!isPlaying)
            return;

        if (currentMapLabel != null)
            currentMapLabel.Text = currentMapID == -1 ? "Overworld" : $"Dungeon {currentMapID+1}";

        if(isLinkWalking)
            LinkWalk(delta);

        if (isPlaying)
            timer = Math.Max(timer - delta, 0);

        if (timer == 0)
        {
            var (currentState, path) = pathfinder.RunStep();

            // Normal timer
            timer = stepTime;

            if (currentState == AStar.State.Success)
                NextAction();
            else if(currentState == AStar.State.NotReady)
                NextAction();
        }
    }

    private void NextAction()
    {
        if (actionSequence.Count == 0)
        {
            isPlaying = false;
            Raylib.TraceLog(TraceLogLevel.LOG_INFO, "ACTIONSEQ: Action sequence finished.");
            return;
        }

        var (action, delay) = actionSequence.Dequeue();
        action.Invoke();
        timer = delay;
    }

    // TODO: CLEAN THIS SHIT UP YOU FUCKIN IDIOT
    private void LinkWalk(float delta)
    {
        if (linkPath == null || linkWhere == 1.0f)
            return;

        var fract = (linkWhere * linkPath.Count) - Math.Floor(linkWhere * linkPath.Count);
        int currentTile = (int) Math.Floor(linkWhere * linkPath.Count);
        int nextTile = (int) Math.Ceiling(linkWhere * linkPath.Count);
        if (Math.Abs(fract) <= 0.001f)
            nextTile = currentTile + 1;

        if (nextTile > linkPath.Count-1)
            return;

        var map = world.GetMapByID(currentMapID);
        (float x, float y) offset = (map.sizeX * tileSize / 2, map.sizeY * tileSize / 2);
        (float x, float y) position;
        position.x = (float) ((1-fract) * linkPath[currentTile].x + fract * linkPath[nextTile].x);
        position.y = (float) ((1-fract) * linkPath[currentTile].y + fract * linkPath[nextTile].y);

        link.pos.x = pos.x + position.x * tileSize - offset.x;
        link.pos.y = pos.y + position.y * tileSize - offset.y;
        // Console.WriteLine($"Link position: ({link.pos.x},{link.pos.y})");

        linkWhere += delta;

        if(linkWhere > 1.0f)
            linkWhere = 1.0f;
    }

    public override void Cleanup()
    {
        currentMapLabel = null;
        costDisplayLabel = null;
        link.Cleanup();
        tileset?.Clear();
    }
}
