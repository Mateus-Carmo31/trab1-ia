using MapPathfinder;

static class Program
{
    public static void Main()
    {
        // Dungeon 1 -> (14, 26) para (13, 3)
        // Dungeon 2 -> (13, 25) para (13, 2)
        // Dungeon 3 -> (14, 25) para (15, 19)
        // Casa pra LostWoods -> (24, 27) para (6, 5)
        // Dungeon 1 pra Dungeon 3 -> (5, 32) para (24, 1)
        // Dungeon 2 pra Dungeon 3 -> (39, 17) para (24, 1)
        var mapStr = System.IO.File.ReadAllText(@"hyrule.txt");
        Map curMap = new Map(mapStr, 42, 42);

        var algorithm = new Astar(curMap);

        List<Tile>? path = algorithm.execute(new Tile(39, 17), new Tile(24, 1));

        path?.ForEach((element) => {
            Console.WriteLine(element);
        });

        Console.WriteLine(path?.Count);

        

        // var app = new Application();
        // app.Init();

        // while(app.isRunning())
        // {
        //     app.MainLoop();
        // }

        // app.Close();
    }
}
