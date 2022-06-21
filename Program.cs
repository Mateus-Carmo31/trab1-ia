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
        // var mapStr = System.IO.File.ReadAllText(@"hyrule.txt");
        // Map curMap = new Map(mapStr, 42, 42);

        // var algorithm = new Astar(curMap);

        // List<Tile>? path = algorithm.execute(new Tile(39, 17), new Tile(24, 1));

        // path?.ForEach((element) => {
        //     Console.WriteLine(element);
        // });

        // Console.WriteLine(path?.Count);

        var app = new Application();
        app.Init();

        while(app.isRunning())
        {
            app.MainLoop();
        }

        app.Close();


        // CÓDIGO DE BUSCA DA MELHOR SEQUÊNCIA DE CAMINHOS COMEÇA AQUI

        // var d1 = new World.Dungeon(new Map.Tile(5, 32), new Map.Tile(14, 26), new Map.Tile(13, 3), "dungeon1.txt");
        // var d2 = new World.Dungeon(new Map.Tile(39, 17), new Map.Tile(13, 25), new Map.Tile(13, 2), "dungeon2.txt");
        // var d3 = new World.Dungeon(new Map.Tile(24, 1), new Map.Tile(14, 25), new Map.Tile(15, 19), "dungeon3.txt");

        // World world = new World(@"hyrule.txt", new Map.Tile(24, 27), new Map.Tile(6, 5), d1, d2, d3);
        // var (perm, cost) = world.FindBestPath();
        // foreach (var elem in perm)
        // {
        //     Console.WriteLine(elem);
        // }
        // Console.WriteLine(cost);
    }
}
