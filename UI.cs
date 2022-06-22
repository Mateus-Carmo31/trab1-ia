namespace MapPathfinder;

using Raylib_cs;
using System.Numerics;
using Rectangle = Raylib_cs.Rectangle;

public abstract class UI
{
    protected readonly Vector2 TILING = new Vector2(1,1);
    protected readonly Vector2 OFFSET = new Vector2(0,0);

    public (float x, float y) pos;
    public bool active = true;

    public UI(float x, float y) { pos.x = x; pos.y = y; }

    public virtual void Draw() {}
    public virtual void Update(float delta) {}
    public virtual void Cleanup() {}
}

public class Label : UI
{
    private string text = "Label";
    public int fontSize = 10;
    public bool centered = true;
    public Color textColor;

    public string Text { get => text; set => text = value; }

    public Label(float x, float y, string text, int fontSize, Color textColor) : base(x,y)
    {
        this.text = text;
        this.fontSize = fontSize;
        this.textColor = textColor;
    }

    public override void Draw()
    {
        if(centered)
            Raylib.DrawText(text, (int) (pos.x - Raylib.MeasureText(text, fontSize) / 2), (int) (pos.y - fontSize / 2), fontSize, textColor);
        else
            Raylib.DrawText(text, (int) pos.x, (int) pos.y, fontSize, textColor);
    }
}

public class Button : UI
{
    public event EventHandler? OnClick;
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
                OnClick?.Invoke(this, new EventArgs());
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
