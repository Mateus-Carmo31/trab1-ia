namespace MapPathfinder;

using Raylib_cs;
using Rectangle = Raylib_cs.Rectangle;

// Encapsulates the map display
public class WorldViewer : UI
{
    // Constants
    public static readonly float slow = 0.3f;
    public static readonly float normal = 0.1f;
    public static readonly float fast = 0.01f;

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
    public bool showPendant = true;

    // A* update controls
    public bool isPlaying = false;
    public float stepTime = normal;
    private float timer;

    // Sequence of actions (actions are changing map, setting AStar points, etc.)
    // Each action has an associated delay in seconds before the action happens
    public Queue<(Action action, float delay)> actionSequence = new Queue<(Action, float)>();

    // Event that is triggered when action sequence finishes
    public event EventHandler? OnSequenceFinished;

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

    public void ChangeMap(int newId)
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

    public void StartActionSequence()
    {
        if (actionSequence.Count == 0 || isPlaying)
            return;

        isPlaying = true;
        Action action;
        (action, timer) = actionSequence.Dequeue();
        action.Invoke();
    }

    public void Reset(World? w = null)
    {
        isPlaying = false;
        isLinkWalking = false;
        linkWhere = 0;
        linkPath = null;
        actionSequence.Clear();
        timer = stepTime;
        showPendant = true;
        showExpandedCosts = false;

        if (w != null)
        {
            world = w;
        }
        ChangeMap(-1);
        link.pos = world.Home;
        costDisplayLabel!.Text = "";
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
        if (currentMapID == -1)
        {
            // Draw lost woods entrance
            Raylib.DrawTextureQuad(tileset.GetSprite('L'), TILING, OFFSET, new Rectangle(pos.x + world.LostWoods.x*tileSize - offset.x, pos.y + world.LostWoods.y*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);

            // Draw house
            Raylib.DrawTextureQuad(tileset.GetSprite('H'), TILING, OFFSET, new Rectangle(pos.x + world.Home.x*tileSize - offset.x, pos.y + world.Home.y*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);

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
            if (showPendant)
            {
                Raylib.DrawTextureQuad(
                    tileset.GetSprite('P'),
                    TILING, OFFSET,
                    new Rectangle(pos.x + dgn.GetObjetive().x*tileSize - offset.x, pos.y + dgn.GetObjetive().y*tileSize - offset.y, tileSize, tileSize), Color.RAYWHITE);
            }
        }

        // Draw expanded tiles over map
        var costsSoFar = pathfinder?.GetCostsSoFar();
        costsSoFar?.Keys.ToList().ForEach((Map.Tile t) => {
                    Raylib.DrawRectangle((int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) tileSize, (int) tileSize, expandedTileColor);
                    if (costsSoFar[t] != int.MaxValue && showExpandedCosts)
                        Raylib.DrawText($"{costsSoFar[t]}", (int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) (tileSize / 5), Color.BLACK);
            });

        // Draws final path (if/when found)
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

        if (currentMapLabel != null)
            currentMapLabel.Text = currentMapID == -1 ? "Overworld" : $"Dungeon {currentMapID+1}";

        if(isLinkWalking && isPlaying)
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
            else if(currentState == AStar.State.Failure)
            {
                // Failed to find some path, so dump the action sequence and report the failure to the user.
                actionSequence.Clear();
                NextAction();
                costDisplayLabel!.Text = "Couldn't find path!";
            }
        }
    }

    private void NextAction()
    {
        if (actionSequence.Count == 0)
        {
            isPlaying = false;
            isLinkWalking = false;
            OnSequenceFinished?.Invoke(this, new EventArgs());
            Raylib.TraceLog(TraceLogLevel.LOG_INFO, "ACTIONSEQ: Action sequence finished.");
            return;
        }

        var (action, delay) = actionSequence.Dequeue();
        action.Invoke();
        timer = delay;
    }

    // Link movement based on a path
    // Takes the current linkWhere, and finds where on the path link should be by rounding and linearly interpolating between two tiles.
    private void LinkWalk(float delta)
    {
        if (linkPath == null || linkWhere == linkPath.Count)
            return;

        var fract = (linkWhere) - Math.Floor(linkWhere);
        int currentTile = (int) Math.Floor(linkWhere);
        int nextTile = (int) Math.Ceiling(linkWhere);
        if (Math.Abs(fract) <= 0.001f)
            nextTile = currentTile + 1;

        if (nextTile > linkPath.Count-1)
            nextTile = linkPath.Count-1;

        var map = world.GetMapByID(currentMapID);
        (float x, float y) offset = (map.sizeX * tileSize / 2, map.sizeY * tileSize / 2);
        (float x, float y) position;
        position.x = (float) ((1-fract) * linkPath[currentTile].x + fract * linkPath[nextTile].x);
        position.y = (float) ((1-fract) * linkPath[currentTile].y + fract * linkPath[nextTile].y);

        link.pos.x = pos.x + position.x * tileSize - offset.x;
        link.pos.y = pos.y + position.y * tileSize - offset.y;
        // Console.WriteLine($"Link position: ({link.pos.x},{link.pos.y})");

        linkWhere += delta * 15f;

        if(linkWhere > linkPath.Count)
        {
            linkWhere = linkPath.Count;
            NextAction();
        }
    }

    public override void Cleanup()
    {
        currentMapLabel = null;
        costDisplayLabel = null;
        link.Cleanup();
        tileset?.Clear();
    }
}
