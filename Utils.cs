using Raylib_cs;
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

    public static Tile ToTile(this (int, int) tuple)
    {
        return new Tile(tuple.Item1, tuple.Item2);
    }
}

public static class Colors {
    public static Color green = new Color(144, 224, 117, 255);
    public static Color lightBlue = new Color(144, 224, 243, 255);
    public static Color middleBlue = new Color(144, 171, 255, 255);
    public static Color red = new Color(255, 76, 0, 255);
}
