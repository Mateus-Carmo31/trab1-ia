using MapPathfinder;

static class Program
{
    public static void Main()
    {
        var mapStr = System.IO.File.ReadAllText(@"dungeon3.txt");
        Map curMap = new Map(mapStr, 28, 28);

        var algorithm = new Astar(curMap);

        List<Tile>? path = algorithm.execute(new Tile(14, 25), new Tile(15, 19));

        path?.ForEach((element) => {
            Console.WriteLine(element);
        });

        

        // var app = new Application();
        // app.Init();

        // while(app.isRunning())
        // {
        //     app.MainLoop();
        // }

        // app.Close();
    }
}
