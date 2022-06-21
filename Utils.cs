using static MapPathfinder.Map;

public static class Utils
{
  public static int ManhattanDistance(Tile goal, Tile origin)
  {
    return Math.Abs(goal.x - origin.x) + Math.Abs(goal.y - origin.y);
  }

  public static float EuclideanDistance(Tile goal, Tile origin)
  {
    return (float) Math.Sqrt((double) ((goal.x - origin.x) * (goal.x - origin.x) + (goal.y - origin.y) * (goal.y - origin.y)));
  }
}
