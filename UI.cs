namespace MapPathfinder;

using Raylib_cs;
using System.Numerics;
using Rectangle = Raylib_cs.Rectangle;

public abstract class UI
{
    public static Texture2D? txButtonOn;
    public static Texture2D? txButtonOff;

    protected readonly Vector2 TILING = new Vector2(1,1);
    protected readonly Vector2 OFFSET = new Vector2(0,0);

    public (float x, float y) pos;

    public UI(float x, float y) { pos.x = x; pos.y = y; }

    public abstract void Draw();
    public virtual void Update() {}
    public abstract Rectangle GetBoundingBox();
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

    public override Rectangle GetBoundingBox()
    {
        throw new NotImplementedException();
    }
}

public class Button
{
    // Normal click button
}

public class ToggleButton : UI
{
    public bool pressed = false;
    public (float x, float y) size;

    public ToggleButton(float x, float y, float sizeX, float sizeY) : base(x,y)
    {
        size.x = sizeX;
        size.y = sizeY;
    }

    public override void Draw()
    {
        if (!txButtonOff.HasValue || !txButtonOn.HasValue)
            return;

        Raylib.DrawTextureQuad(pressed ? txButtonOn.Value : txButtonOff.Value, TILING, OFFSET, new Rectangle(pos.x, pos.y, size.x, size.y), Color.RAYWHITE);
    }

    public override void Update()
    {
        // Check for mouse position and click
    }

    public override Rectangle GetBoundingBox()
    {
        throw new NotImplementedException();
    }
}

public class MapViewer : UI
{
    private float tileSize;
    private Map? map;
    private Map.Tileset? tileset;

    public MapViewer(float x, float y, float tileSize) : base(x,y)
    {
        this.tileSize = tileSize;
    }

    public float TileSize { get => tileSize; set => tileSize = value; }
    public Map? Map { get => map; set => map = value; }
    public Map.Tileset? Tileset
    {
        get => tileset;
        set
        {
            tileset?.ClearTileset();
            tileset = value;
        }
    }

    public override void Draw()
    {
        if (map == null || tileset == null)
            return;

        for(int j = 0; j < map.sizeY; j++)
        {
            for(int i = 0; i < map.sizeX; i++)
            {
                Raylib.DrawTextureQuad(tileset.GetSprite(map[i,j]), TILING, OFFSET, new Rectangle(pos.x + i*tileSize, pos.y + j*tileSize, tileSize, tileSize), Color.RAYWHITE);
            }
        }
    }

    public override Rectangle GetBoundingBox()
    {
        if (map != null)
            return new Rectangle(pos.x, pos.y, map.sizeX * tileSize, map.sizeY * tileSize);
        else
            return new Rectangle(pos.x, pos.y, 0, 0);
    }
}
