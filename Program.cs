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

        List<Dungeon> dungeons = new List<Dungeon>();


        Map hyrule = new Map(System.IO.File.ReadAllText(@"hyrule.txt"), 42, 42);
        Map.Tile linksHome = new Map.Tile(24, 27);
        Map.Tile lostWoods = new Map.Tile(6, 5);

        dungeons.Add(new Dungeon(new Map.Tile(5, 32), new Map.Tile(14, 26), new Map.Tile(13, 3), "dungeon1.txt"));
        dungeons.Add(new Dungeon(new Map.Tile(39, 17), new Map.Tile(13, 25), new Map.Tile(13, 2), "dungeon2.txt"));
        dungeons.Add(new Dungeon(new Map.Tile(24, 1), new Map.Tile(14, 25), new Map.Tile(15, 19), "dungeon3.txt"));

        List<List<Dungeon>> permutations = Permutation(dungeons);

        
        int bestPermutationIndex = 0;
        int bestPermutationCost = int.MaxValue;

        for (int i = 0; i < permutations.Count; i++)
        {
            int currentCost = EvaluateCostOfPermutation(permutations[i], hyrule, linksHome);
            if (currentCost < bestPermutationCost)
            {
                bestPermutationIndex = i;
                bestPermutationCost = currentCost;
            }
        }

        // Melhor caminho entre as Dungeons
        List<Dungeon> dungeonsOrder = permutations[bestPermutationIndex];
    }

    public static List<List<Dungeon>> Permutation(List<Dungeon> list)
    {
        // Chama função recursiva de permutação
        return RecursivePermutation(list, new List<Dungeon>());
    }

    // Função recursiva que calcula uma lista de permutações a partir de
    // uma dada lista de elementos. O segundo parâmetro é utilizado na recursão,
    // e deve ser inicialmente dado como uma lista vazia.
    public static List<List<Dungeon>> RecursivePermutation(List<Dungeon> rest, List<Dungeon> prefix)
    {
        // Caso base da recursão
        if(rest.Count == 0)
        {
            return new List<List<Dungeon>>{prefix};
        }

        List<List<Dungeon>> permutations = new List<List<Dungeon>>();
        foreach(Dungeon d in rest)
        {
            List<Dungeon> rest_copy = rest.ToList();
            List<Dungeon> prefix_copy = prefix.ToList();

            rest_copy.Remove(d);
            prefix_copy.Add(d);
            
            // Chamada recursiva da função, passando "rest" sem "d" e "prefix" + "d"
            permutations.AddRange(RecursivePermutation(rest_copy, prefix_copy));
        }

        return permutations;
    }

    public static int EvaluateCostOfPermutation(List<Dungeon> permutation, Map hyrule, Map.Tile linksHome)
    {
        int costOfPermutation = 0;

        // Custo da Casa do Link para a primeira dungeon da permutação
        costOfPermutation += AStar.FindPath(
            hyrule,
            linksHome.GetTuple(), 
            permutation[0].GetHyruleLocation().GetTuple()
        )?.cost ?? 0;
        for (int i = 0; i < permutation.Count; i++)
        {
            // Custo para entrar na dungeon, indo até o pingente e voltar
            costOfPermutation += 2 * AStar.FindPath(
                permutation[i].GetMap(),
                permutation[i].GetInnerStartLocation().GetTuple(),
                permutation[i].GetPendantLocation().GetTuple()
            )?.cost ?? 0;

            // Se não for a última dungeon do trajeto, adicionar custo para a próxima dungeon
            if (i != permutation.Count - 1)
            {
                costOfPermutation += AStar.FindPath(
                    hyrule,
                    permutation[i].GetHyruleLocation().GetTuple(),
                    permutation[i + 1].GetHyruleLocation().GetTuple()
                )?.cost ?? 0;
            }
        }
        // Custo da última dungeon até casa do Link
        costOfPermutation += AStar.FindPath(
            hyrule,
            permutation.Last().GetHyruleLocation().GetTuple(),
            linksHome.GetTuple()
        )?.cost ?? 0;

        return costOfPermutation;
    }

}
