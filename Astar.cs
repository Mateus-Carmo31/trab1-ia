namespace MapPathfinder;

public class Astar
{
    Map map;

    public Astar(Map map)
    {
        this.map = map;
    }

    public List<Tile>? execute(Tile start, Tile goal) 
    {
        PriorityQueue<Tile, int?> frontier = new PriorityQueue<Tile, int?>();
        frontier.Enqueue(start, 0);

        Dictionary<Tile, Tile?> cameFrom  = new Dictionary<Tile, Tile?>();
        Dictionary<Tile, int> costSoFar  = new Dictionary<Tile, int>();
        cameFrom[start] = null;
        costSoFar[start] = 0;

        while(frontier.Count != 0)
        {
            Tile current = frontier.Dequeue();

            if (current == goal)
            {
                break;                
            }

            foreach (Tile next in neighbours(current))
            {  
                int newCost = costSoFar[current] + CostToNext(next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + ManhattanDistance(goal, next);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // Nunca chegou no objetivo, portanto não há caminho
        if (cameFrom.ContainsKey(goal) == false)
        {
            return null;
        }

        List<Tile> path = new List<Tile>();
        // Tile auxTile = goal;

        // Console.WriteLine();

        // while(true)
        // {
        //     path.Add(auxTile);
        //     if (cameFrom[auxTile] == null)
        //     {   
        //         break;
        //     }
        //     auxTile = cameFrom[auxTile] ?? new Tile(0, 0);
        // }

        // path.Reverse();

        return path;
    }

    public int ManhattanDistance(Tile goal, Tile origin)
    {
        return Math.Abs(goal.x - origin.x) + Math.Abs(goal.y - origin.y);
    }

    public int CostToNext(Tile next)
    {
        char tileSymbol = map[next.x, next.y];
        var cost = Map.TileCosts[tileSymbol];
        return cost;
    }

    public List<Tile> neighbours(Tile current)
    {
        // Lista de posições relativas adjacentes ao current
        var neighbours = new List<Tile> {new Tile(1, 0), new Tile(0, -1), new Tile(-1, 0), new Tile(0, 1)};

        // Lista de posições absolutas adjacentes ao current no mapa
        neighbours = neighbours.ConvertAll<Tile>((Tile tile) => {
            tile.x += current.x;
            tile.y += current.y;
            return tile;
        });

        // Filtragem para não pegar posições fora do mapa
        neighbours = neighbours.FindAll((Tile tile) => {
            return (tile.x >= 0 && tile.x < map.sizeX && tile.y >= 0 && tile.y < map.sizeY);
        });

        return neighbours;
    }
}

public record struct Tile(int x, int y);
