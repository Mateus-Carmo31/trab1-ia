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
    public bool showExpandedTiles = false;

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
        link = new Sprite(world.Home.x, world.Home.y, "assets/link.png");
    }

    public float TileSize { get => tileSize; set => tileSize = value; }
    public Map.Tileset? Tileset { get => tileset; set => tileset = value; }

    public void ChangeMap(int newId, bool setupPath = false)
    {
        currentMapID = newId;
        pathfinder = new AStar(world.GetMapByID(newId));
    }

    public void SetAStarPoints((int x, int y)? newStart = null, (int x, int y)? newGoal = null)
    {
        pathfinder?.SetPoints(newStart, newGoal);
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
        // TODO: font size scaling doesn't work
        if (showExpandedTiles)
        {
            var costsSoFar = pathfinder?.GetCostsSoFar();
            costsSoFar?.Keys.ToList().ForEach((Map.Tile t) => {
                        Raylib.DrawRectangle((int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) tileSize, (int) tileSize, expandedTileColor);
                        if (costsSoFar[t] != int.MaxValue)
                            Raylib.DrawText($"{costsSoFar[t]}", (int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) (tileSize / 5), Color.BLACK);
                });
        }

        // Draws final path (when found)
        if (showPath)
        {
            pathfinder?.FinalPath?.tiles.ForEach((Map.Tile t) => {
                    Raylib.DrawRectangle((int) (pos.x + t.x * tileSize - offset.x), (int) (pos.y + t.y * tileSize - offset.y), (int) tileSize, (int) tileSize, pathColor);
                });
        }
    }

    public override void Update(float delta)
    {
        // if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
        //     showPath = !showPath;

        // if (Raylib.IsKeyPressed(KeyboardKey.KEY_C))
        //     showExpandedTiles = !showExpandedTiles;

        if (currentMapLabel != null)
            currentMapLabel.Text = currentMapID == -1 ? "Overworld" : $"Dungeon {currentMapID+1}";

        if (isPlaying)
            timer = Math.Max(timer - delta, 0);

        if (timer == 0)
        {
            var (currentState, path) = pathfinder.RunStep();

            // Normal timer
            timer = stepTime;

            if (actionSequence.Count > 0 && currentState == AStar.State.Success)
            {
                var (action, delay) = actionSequence.Dequeue();
                action.Invoke();
                timer = delay;
            }
            else if(currentState == AStar.State.Success) // There are no more actions, so stop the timer.
            {
                isPlaying = false;
                Raylib.TraceLog(TraceLogLevel.LOG_INFO, "ACTIONSEQ: Action sequence finished.");
            }
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
