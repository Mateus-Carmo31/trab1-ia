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

    public string buttonText;

    public Color buttonColor;

    public Button(float x, float y, float sizeX, float sizeY, string buttonText, Color buttonColor) : base(x,y)
    {
        size.x = sizeX;
        size.y = sizeY;
        this.buttonText = buttonText ?? "";
        this.buttonColor = buttonColor;
    }

    public override void Draw()
    {
        var border = 5;
        int fontSize = 22;
        // Draw Border
        Raylib.DrawRectangle((int) pos.x, (int) pos.y, (int) size.x, (int) size.y, Color.BLACK);
        // Draw Inner Button Color
        Raylib.DrawRectangle((int) pos.x+border, (int) pos.y+border, (int) size.x-border*2, (int) size.y-border*2, buttonColor);

        Raylib.DrawText(buttonText,(int) (pos.x + ((size.x - Raylib.MeasureText(buttonText, fontSize)) / 2)), (int) (pos.y + (size.y - fontSize) / 2), fontSize, Color.WHITE);
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

    public string text;

    public ToggleButton(float x, float y, string text) : base(x,y)
    {
        size.x = 50;
        size.y = 50;
        this.text = text;
    }

    public override void Draw()
    {   
        int outerBorder = 5;
        int innerBorder = 5;
        int totalBorder = outerBorder + innerBorder;
        int fontSize = 22;

        // Draw Border
        Raylib.DrawRectangle((int) pos.x, (int) pos.y, (int) size.x, (int) size.y, Color.BLACK);
        // Draw Inner Button Color
        Raylib.DrawRectangle((int) pos.x + outerBorder, (int) pos.y + outerBorder, (int) size.x - outerBorder * 2, (int) size.y - outerBorder * 2, Color.WHITE);
        if (pressed)
        {
            Raylib.DrawRectangle((int) pos.x + totalBorder, (int) pos.y + totalBorder, (int) size.x - totalBorder * 2, (int) size.y - totalBorder * 2, Color.GREEN);
        }

        if (text.Length != 0)
        {
            Raylib.DrawText(text, (int) (pos.x + size.x + 8), (int) (pos.y + (size.y - fontSize) / 2), fontSize, Color.BLACK);
        }
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

    public Sprite(float x, float y, float sizeX, float sizeY) : base(x,y)
    {
        this.size = new Vector2(sizeX, sizeY);
    }

    public Sprite(float x, float y, float sizeX, float sizeY, string path) : base(x,y)
    {
        this.size = new Vector2(sizeX, sizeY);
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
