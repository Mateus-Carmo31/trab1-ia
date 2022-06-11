using Raylib_cs;

public class Map
{
    public char[] mapData;
    private static readonly Dictionary<char, int> TileCosts = new Dictionary<char, int>()
        {
            {'.', 10},
            {'_', 20},
            {'T', 100},
            {'^', 150},
            {'~', 180},
            {'#', int.MaxValue},
            {' ', 10}
        };

    public int mapX, mapY;

    public Map()
    {
        this.mapData = new char[42 * 42];
        this.mapX = 42;
        this.mapY = 42;
    }

    public Map(string mapStr, int mapX, int mapY)
    {
        this.mapX = mapX;
        this.mapY = mapY;
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

    public void DrawMap(int posX, int posY, int tileSize)
    {
        for(int j = 0; j < mapY; j++)
        {
            for(int i = 0; i < mapX; i++)
            {
                var (c, cost) = GetPos(i, j);
                Raylib.DrawRectangle(posX + i * tileSize, posY + j * tileSize, tileSize, tileSize, c == ' ' ? Color.GRAY : Color.BLACK);
            }
        }
    }

    public (char, int) GetPos(int x, int y)
    {
        char posTile = mapData[x + y * mapX];
        return (posTile, TileCosts.GetValueOrDefault(posTile, -1));
    }
}
