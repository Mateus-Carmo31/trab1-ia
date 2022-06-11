using Raylib_cs;

public class Map
{
    public char[] mapData;
    public Dictionary<char, int> tileCosts;

    public int mapX, mapY;

    public Map()
    {
        this.mapData = new char[42 * 42];
        this.tileCosts = new Dictionary<char, int>();
        this.mapX = 42;
        this.mapY = 42;
    }

    public Map(string mapStr, int mapX, int mapY)
    {
        this.mapX = mapX;
        this.mapY = mapY;
        this.mapData = new char[mapX * mapY];

        int i = 0, j = 0;
        foreach (var c in mapStr)
        {
            if (c == '\n')
            {
                j = 0;
                i++;
                continue;
            }
            this.mapData[j + i * mapX] = c;
            j++;
        }

        this.tileCosts = new Dictionary<char, int>();
    }

    public void DrawMap(int posX, int posY, int tileSize)
    {
        for(int i = 0; i < mapY; i++)
        {
            for(int j = 0; j < mapX; j++)
            {
                var (c, cost) = GetPos(j, i);
                Raylib.DrawRectangle(posX + j * tileSize, posY + i * tileSize, tileSize, tileSize, c == ' ' ? Color.GRAY : Color.BLACK);
            }
        }
    }

    public (char, int) GetPos(int x, int y)
    {
        char posTile = mapData[x + y * mapX];
        return (posTile, tileCosts.GetValueOrDefault(posTile, -1));
    }
}
