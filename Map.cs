using Raylib_cs;

public class Map
{
    public char[] mapData;
    public static readonly Dictionary<char, int> TileCosts = new Dictionary<char, int>()
        {
            {'.', 10},
            {'_', 20},
            {'T', 100},
            {'^', 150},
            {'~', 180},
            {'#', int.MaxValue},
            {' ', 10}
        };

    public class Tileset
    {
        private Dictionary<char, Texture2D> sprites = new Dictionary<char, Texture2D>();

        public void SetSprite(char tile, string imagePath)
        {
            Image image = Raylib.LoadImage(imagePath);
            Texture2D texture = Raylib.LoadTextureFromImage(image);
            Raylib.UnloadImage(image);

            if (!sprites.ContainsKey(tile))
            {
                sprites.Add(tile, texture);
                return;
            }

            Raylib.UnloadTexture(sprites[tile]);
            sprites[tile] = texture;
        }

        public Texture2D GetSprite(char tile)
        {
            return sprites[tile];
        }

        public void Clear()
        {
            Raylib.TraceLog(TraceLogLevel.LOG_INFO, $"TILESET: Clearing tileset ({sprites.Count} textures)");
            foreach (var tex in sprites.Values)
            {
                Raylib.UnloadTexture(tex);
            }
            Raylib.TraceLog(TraceLogLevel.LOG_INFO, $"TILESET: Tileset cleared.");
            sprites.Clear();
        }

        ~Tileset()
        {
            Clear();
        }
    }

    public record struct Tile(int x, int y) {
        public (int x, int y) GetTuple() => (x, y);
    }

    public int sizeX, sizeY;

    public Map(int mapX, int mapY)
    {
        this.mapData = new char[mapX * mapY];
        this.sizeX = 42;
        this.sizeY = 42;
    }

    public Map(string mapStr, int mapX, int mapY)
    {
        this.sizeX = mapX;
        this.sizeY = mapY;
        this.mapData = new char[mapX * mapY];

        int y = 0, x = 0;
        foreach (var c in mapStr)
        {
            if (c == '\n')
            {
                x = 0;
                y++;
                continue;
            }
            this.mapData[x + y * mapX] = c;
            x++;
        }
    }

    public char this[int x, int y]
    {
        get { return mapData[x + y * sizeX]; }
        set { mapData[x + y * sizeX] = value; }
    }

    public int GetCostAt(int x, int y)
    {
        return TileCosts[this[x,y]];
    }

    public List<Tile> Neighbours(Tile t)
    {
        // Lista de posições relativas adjacentes ao current
        var neighbours = new List<Tile> {new Tile(1, 0), new Tile(0, -1), new Tile(-1, 0), new Tile(0, 1)};

        // Lista de posições absolutas adjacentes ao current no mapa
        neighbours = neighbours.ConvertAll<Tile>((Tile tile) => {
            tile.x += t.x;
            tile.y += t.y;
            return tile;
        });

        // Filtragem para não pegar posições fora do mapa
        neighbours = neighbours.FindAll((Tile tile) => {
            return (tile.x >= 0 && tile.x < this.sizeX && tile.y >= 0 && tile.y < this.sizeY);
        });

        return neighbours;
    }
}
