namespace MapPathfinder;

public class Astar
{
    Map map;

    public void astar() 
    {
        Tile start = new Tile(1, 1);
        Tile goal = new Tile(15, 10);
        PriorityQueue<Tile, int?> frontier = new PriorityQueue<Tile, int?>();
        frontier.Enqueue(start, 0);

        Dictionary<Tile, Tile?> cameFrom  = new Dictionary<Tile, Tile?>();
        Dictionary<Tile, int> costSoFar  = new Dictionary<Tile, int>();
        cameFrom[start] = null;
        costSoFar[start] = 0;

        while(frontier.Count == 0)
        {
            Tile current = frontier.Dequeue();

            if (current == goal)
            {
                break;                
            }

            foreach (Tile next in neighbours(current))
            {
                int newCost = costSoFar[current] + GraphCost();
                if (costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + ManhattanDistance(goal, next);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
    }

    public int ManhattanDistance(Tile goal, Tile origin)
    {
        return Math.Abs(goal.x - origin.x) + Math.Abs(goal.y - origin.y);
    }

    public int GraphCost()
    {
        return 1;
    }

    public List<Tile> neighbours(Tile current)
    {
        var neighbours = new List<Tile> {new Tile(1, 0), new Tile(0, 1), new Tile(-1, 0), new Tile(0, -1)};
        neighbours.ForEach((Tile tile) => {
            tile.x += current.x;
            tile.y += current.y;
            });
        neighbours = neighbours.FindAll(
            (Tile tile) =>
            (tile.x >= 0 && tile.x <= map.sizeX &&
             tile.y >= 0 && tile.y <= map.sizeY)
            );
        return neighbours;
    }
}

public record struct Tile(int x, int y);
