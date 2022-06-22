namespace MapPathfinder;

using Tile = Map.Tile;
using Path = Map.Path;

public class AStar
{
    Map map;

    PriorityQueue<Tile, float?> frontier = new PriorityQueue<Tile, float?>();
    Dictionary<Tile, Tile?> cameFrom  = new Dictionary<Tile, Tile?>();
    Dictionary<Tile, int> costSoFar  = new Dictionary<Tile, int>();
    Tile start;
    Tile goal;
    Tile current;

    public enum State { NotReady, NotStarted, InExec, Success, Failure }
    State currentState = State.NotStarted;

    Path? finalPath = null;

    public Path? FinalPath { get => finalPath; }

    // Creates AStar without start/goal. Is set to failure to avoid mistakes when running.
    public AStar(Map map)
    {
        this.map = map;
        currentState = State.NotReady;
    }

    public AStar(Map map, (int x, int y) start, (int x, int y) goal)
    {
        this.map = map;
        this.start = new Tile(start.x, start.y);
        this.goal = new Tile(goal.x, goal.y);
        frontier.Enqueue(this.start, 0);
        cameFrom[this.start] = null;
        costSoFar[this.start] = 0;
    }

    // Does a single step of A*. Use for demonstrative purposes.
    // Returns a tuple (state, path, cost) where state is the current AStar state, path can be null (if no path was found) and cost is the total cost of the path.
    public (State, Path?) RunStep()
    {
        // Astar was already run to completion, so return result
        if (currentState == State.Success || currentState == State.Failure || currentState == State.NotReady)
        {
            return (currentState, finalPath);
        }

        if (currentState == State.NotStarted)
            currentState = State.InExec;

        current = frontier.Dequeue();

        // Finished, so return result
        if (current == goal)
        {
            finalPath = BacktrackPath(goal);
            currentState = State.Success;
            return (currentState, finalPath);
        }

        foreach (Tile next in map.Neighbours(current))
        {
            int costToNext = CostToNext(next);
            int newCost;

            if (costToNext == int.MaxValue || costSoFar[current] == int.MaxValue)
            {
                newCost = costToNext;
            }
            else
            {
                newCost = costToNext + costSoFar[current];
            }

            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
            {
                costSoFar[next] = newCost;
                float priority = newCost == int.MaxValue ? newCost : newCost + Utils.ManhattanDistance(goal, next);
                frontier.Enqueue(next, priority);
                cameFrom[next] = current;
            }
        }

        // Frontier is empty, which means that there's nothing left to explore
        // Since we got here, it means we never found the goal, so return null path.
        if (frontier.Count == 0)
        {
            currentState = State.Failure;
            return (State.Failure, null);
        }

        return (State.InExec, null);
    }

    // Does the full A*. Use for computation
    // Returns a tuple (path, cost) where path can be null in the case that no path was found.
    public Path? RunFull()
    {
        Path? path;
        do
        {
            (_, path) = RunStep();
        }
        while (currentState == State.InExec);
        return path;
    }

    public static Path? FindPath(Map map, (int x, int y) start, (int x, int y) goal)
    {
        AStar pathfinder = new AStar(map, start, goal);
        return pathfinder.RunFull();
    }

    public static Path? FindPath(Map map, Tile start, Tile goal)
    {
        return FindPath(map, (start.x, start.y), (goal.x, goal.y));
    }

    // Resets the AStar instance (without changing start and goal)
    public void Reset()
    {
        frontier = new PriorityQueue<Tile, float?>();
        cameFrom = new Dictionary<Tile, Tile?>();
        costSoFar = new Dictionary<Tile, int>();
        currentState = State.NotReady;
        finalPath = null;
    }

    // Sets new start and/or goal (and resets instance)
    public void SetPoints((int x, int y)? newStart = null, (int x, int y)? newGoal = null)
    {
        Reset();
        if(newStart.HasValue)
        {
            start = new Tile(newStart.Value.x, newStart.Value.y);
            frontier.Enqueue(start, 0);
            cameFrom[start] = null;
            costSoFar[start] = 0;
        }
        if(newGoal.HasValue)
            goal = new Tile(newGoal.Value.x, newGoal.Value.y);

        if(newStart.HasValue && newGoal.HasValue)
            currentState = State.NotStarted;
    }

    // Backtracks from tile t until the beginning.
    // Returns null if there is no path to t.
    public Path? BacktrackPath(Tile t)
    {
        // The tile was never reached, so there's no path.
        if (cameFrom.ContainsKey(t) == false)
        {
            return null;
        }

        List<Tile> path = new List<Tile>();
        Tile auxTile = t;

        // Iteratively build path backwards from the tile.
        while(true)
        {
            path.Insert(0, auxTile);
            if (cameFrom[auxTile] == null)
            {
                break;
            }
            auxTile = cameFrom[auxTile]!.Value;
        }

        return new Path(path, costSoFar[t]);
    }

    public Dictionary<Tile, int> GetCostsSoFar()
    {
        return costSoFar;
    }

    private int CostToNext(Tile next)
    {
        return map.GetCostAt(next.x, next.y);
    }
}
