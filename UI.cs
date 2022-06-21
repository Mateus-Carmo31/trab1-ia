namespace MapPathfinder;

using Raylib_cs;
using System.Numerics;
using Rectangle = Raylib_cs.Rectangle;

public abstract class UI
{
    protected readonly Vector2 TILING = new Vector2(1,1);
    protected readonly Vector2 OFFSET = new Vector2(0,0);

    public (float x, float y) pos;

    public UI(float x, float y) { pos.x = x; pos.y = y; }

    public abstract void Draw();
    public virtual void Update(float delta) {}
    public virtual void Cleanup() {}
}

public class Label : UI
{
    public string text = "Label";
    public int fontSize = 10;
    public Color textColor;

    public Label(float x, float y, string text, int fontSize, Color textColor) : base(x,y)
    {
        this.text = text;
        this.fontSize = fontSize;
        this.textColor = textColor;
    }

    public override void Draw()
    {
        Raylib.DrawText(text, (int) pos.x, (int) pos.y, fontSize, textColor);
    }
}

public class Button : UI
{
    public event EventHandler OnClick;
    private bool pressed = false;
    public (float x, float y) size;

    public Button(float x, float y, float sizeX, float sizeY) : base(x,y)
    {
        size.x = sizeX;
        size.y = sizeY;
    }

    public override void Draw()
    {
        Raylib.DrawRectangle((int) pos.x, (int) pos.y, (int) size.x, (int) size.y, pressed ? Color.BLACK : Color.GRAY);
    }

    public override void Update(float delta)
    {
        var mouse = Raylib.GetMousePosition();
        var bounds = new Rectangle(pos.x, pos.y, size.x, size.y);

        if (mouse.X >= pos.x && mouse.X <= pos.x + size.x && mouse.Y >= pos.y && mouse.Y <= pos.y + size.y && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            pressed = true;

        if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
        {
            if (mouse.X >= pos.x && mouse.X <= pos.x + size.x && mouse.Y >= pos.y && mouse.Y <= pos.y + size.y)
            {
                OnClick.Invoke(this, new EventArgs());
                Raylib.TraceLog(TraceLogLevel.LOG_INFO, $"BUTTON: Button pressed");
            }
            pressed = false;
        }
    }
}

public class ToggleButton : UI
{
    public event EventHandler<bool>? OnToggle;
    public bool pressed = false;
    public (float x, float y) size;

    public ToggleButton(float x, float y, float sizeX, float sizeY) : base(x,y)
    {
        size.x = sizeX;
        size.y = sizeY;
    }

    public override void Draw()
    {
        // if (!txButtonOff.HasValue || !txButtonOn.HasValue)
        //     return;

        // Raylib.DrawTextureQuad(pressed ? txButtonOn.Value : txButtonOff.Value, TILING, OFFSET, new Rectangle(pos.x, pos.y, size.x, size.y), Color.RAYWHITE);
        Raylib.DrawRectangle((int) pos.x, (int) pos.y, (int) size.x, (int) size.y, pressed ? Color.BLACK : Color.GRAY);
    }

    public override void Update(float delta)
    {
        var mouse = Raylib.GetMousePosition();
        var bounds = new Rectangle(pos.x, pos.y, size.x, size.y);

        if (mouse.X >= pos.x && mouse.X <= pos.x + size.x && mouse.Y >= pos.y && mouse.Y <= pos.y + size.y && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        {
            pressed = !pressed;
            OnToggle?.Invoke(this, pressed);
            Raylib.TraceLog(TraceLogLevel.LOG_INFO, $"BUTTON: ToggleButton pressed ({pressed})");
        }
    }
}

// Encapsulates the map display
// TODO: link sprite moving over map when path is found and transitioning between maps
public class WorldViewer : UI
{
    // References
    private World world;
    private int currentMapID = -1;
    private AStar pathfinder;
    private Map.Tileset? tileset = null;

    // Visual Controls
    private float tileSize;
    public bool showPath = true;
    public bool showExpandedTiles = false;

    // A* update controls
    public bool isPlaying = false;
    public float stepTime = 0.01f, waitTime = 3f;
    private float timer;

    // Sequence of actions (actions are changing map, setting AStar points, etc.)
    public Queue<Action> actionSequence = new Queue<Action>();

    private static Color expandedTileColor = new Color(255, 71, 71, 127);
    private static Color pathColor = new Color(17, 255, 0, 200);

    public WorldViewer(float x, float y, float tileSize, World world) : base(x,y)
    {
        this.world = world;
        this.pathfinder = new AStar(world.GetMapByID(currentMapID));
        this.tileSize = tileSize;
        timer = stepTime;
    }

    public float TileSize { get => tileSize; set => tileSize = value; }
    public Map.Tileset? Tileset
    {
        get => tileset;
        set
        {
            tileset = value;
        }
    }

    public void ChangeMap(int newId, bool setupPath = false)
    {
        currentMapID = newId;
        pathfinder = new AStar(world.GetMapByID(newId));
    }

    public void SetAStarPoints((int x, int y)? newStart = null, (int x, int y)? newGoal = null)
    {
        pathfinder?.SetPoints(newStart, newGoal);
    }

    public override void Draw()
    {
        if (tileset == null)
            return;

        var map = world.GetMapByID(currentMapID);

        // Draws map tiles
        for(int j = 0; j < map.sizeY; j++)
        {
            for(int i = 0; i < map.sizeX; i++)
            {
                Raylib.DrawTextureQuad(tileset.GetSprite(map[i,j]), TILING, OFFSET, new Rectangle(pos.x + i*tileSize, pos.y + j*tileSize, tileSize, tileSize), Color.RAYWHITE);
            }
        }

        // Draws expanded tiles over map
        // TODO: font size scaling doesn't work
        if (showExpandedTiles)
        {
            var costsSoFar = pathfinder?.GetCostsSoFar();
            costsSoFar?.Keys.ToList().ForEach((Map.Tile t) => {
                        Raylib.DrawRectangle((int) (pos.x + t.x * tileSize), (int) (pos.y + t.y * tileSize), (int) tileSize, (int) tileSize, expandedTileColor);
                        if (costsSoFar[t] != int.MaxValue)
                            Raylib.DrawText($"{costsSoFar[t]}", (int) (pos.x + t.x * tileSize), (int) (pos.y + t.y * tileSize), (int) (tileSize / 4), Color.BLACK);
                });
        }

        // Draws final path (when found)
        if (showPath)
        {
            pathfinder?.FinalPath?.tiles.ForEach((Map.Tile t) => {
                    Raylib.DrawRectangle((int) (pos.x + t.x * tileSize), (int) (pos.y + t.y * tileSize), (int) tileSize, (int) tileSize, pathColor);
                });
        }
    }

    AStar.State lastState;
    public override void Update(float delta)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
            showPath = !showPath;

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_C))
            showExpandedTiles = !showExpandedTiles;

        if (isPlaying)
            timer = Math.Max(timer - delta, 0);

        if (timer == 0)
        {
            var (currentState, _) = pathfinder.RunStep();

            // Timer finished and AStar ended. Waiting before doing an action...
            if (currentState == AStar.State.Success && lastState != AStar.State.Success)
            {
                timer = waitTime;
            }
            else if(lastState == AStar.State.Success) // Timer finished and pathfinder was already done. So do next action...
            {
                // TODO: check for empty before dequeueing, moron
                actionSequence.Dequeue().Invoke();
                lastState = AStar.State.NotStarted;
                timer = stepTime;
                return;
            }
            else // Normal timer
            {
                timer = stepTime;
            }

            lastState = currentState;
        }
    }

    public override void Cleanup()
    {
        tileset?.Clear();
    }
}

public class Sprite : UI
{
    private Texture2D? texture = null;
    private Vector2 size;

    public Sprite(float x, float y) : base(x,y) {}
    public Sprite(float x, float y, string path) : base(x,y)
    {
        SetTexture(path);
    }

    public void SetTexture(string path)
    {
        if (texture.HasValue)
            Raylib.UnloadTexture(texture.Value);

        var img = Raylib.LoadImage(path);
        texture = Raylib.LoadTextureFromImage(img);
        Raylib.UnloadImage(img);
    }

    public override void Draw()
    {
        if (texture.HasValue)
            Raylib.DrawTextureQuad(texture.Value, TILING, OFFSET, new Rectangle(pos.x, pos.y, size.X, size.Y), Color.RAYWHITE);
    }

    public override void Cleanup()
    {
        if (texture.HasValue)
            Raylib.UnloadTexture(texture.Value);
        texture = null;
    }
}
