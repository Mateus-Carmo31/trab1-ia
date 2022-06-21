namespace MapPathfinder;

using Tile = Map.Tile;

public class World
{
    public class Dungeon
    {
        private Tile overworldPoint;
        private Tile startPoint;
        private Tile objective;
        private Map map;
        public int? crossingCost;

        public Dungeon(Tile locationOnMap, Tile startInside, Tile pendant, string mapData)
        {
            this.overworldPoint = locationOnMap;
            this.startPoint = startInside;
            this.objective = pendant;
            string mapString = System.IO.File.ReadAllText($@"{mapData}");
            this.map = new Map(mapString, 28, 28);
        }

        public Tile GetOverworldPoint() => this.overworldPoint;
        public Tile GetStartPoint() => this.startPoint;
        public Tile GetObjetive() => this.objective;
        public Map GetMap() => this.map;

        public override string ToString()
        {
            return $"{this.overworldPoint.x}/{this.overworldPoint.y}";
        }
    }


    Map overworld;
    List<Dungeon> dungeons;

    Tile linksHome;
    Tile lostWoodsEntrance;

    public World(string overworldMapFile, Tile linksHome, Tile lostWoodsEntrance, params Dungeon[] dungeons)
    {
        this.overworld = new Map(System.IO.File.ReadAllText(overworldMapFile), 42, 42);
        this.linksHome = linksHome;
        this.lostWoodsEntrance = lostWoodsEntrance;
        this.dungeons = dungeons.ToList();
    }

    public Tile Home { get => linksHome; }
    public Tile LostWoods { get => lostWoodsEntrance; }
    public Map Overworld { get => overworld; }
    public List<Dungeon> Dungeons { get => dungeons; }

    public Map GetMapByID(int id)
    {
        if(id == -1)
            return overworld;

        return dungeons[id].GetMap();
    }

    public (int[], int) FindBestPath()
    {
        List<int[]> permutations = new List<int[]>();
        Permutations(Enumerable.Range(0, dungeons.Count).ToArray(), 0, dungeons.Count-1, ref permutations);

        int bestOrderId = 0;
        int bestOrderCost = int.MaxValue;

        for (int i = 0; i < permutations.Count; i++)
        {
            int currentCost = EvaluateCost(permutations[i]);
            if (currentCost < bestOrderCost)
            {
                bestOrderId = i;
                bestOrderCost = currentCost;
            }
        }

        return (permutations[bestOrderId], bestOrderCost);
    }

    //
    private void Permutations(int[] dungeonIds, int l, int r, ref List<int[]> permutationsOut)
    {
        if (l == r)
            permutationsOut.Add(dungeonIds.ToArray());
        else
        {
            for (int i = l; i <= r; i++)
            {
                // Permutates two elements
                var temp = dungeonIds[l];
                dungeonIds[l] = dungeonIds[i];
                dungeonIds[i] = temp;

                // Calls the recursion
                Permutations(dungeonIds, l+1, r, ref permutationsOut);

                // Undoes the permutation (backtracking)
                temp = dungeonIds[l];
                dungeonIds[l] = dungeonIds[i];
                dungeonIds[i] = temp;
            }
        }
    }

    private int EvaluateCost(int[] perm)
    {
        PopulateDungeonCosts();
        int costOfPermutation = 0;
        Map.Path? path;

        // Custo da Casa do Link para a primeira dungeon da permutação
        path = AStar.FindPath(
            overworld,
            linksHome,
            Dungeons[perm[0]].GetOverworldPoint()
        );

        if (path == null)
            return int.MaxValue;

        costOfPermutation += path.Cost;

        for (int i = 0; i < perm.Length; i++)
        {
            // Custo para entrar na dungeon, indo até o pingente e voltar
            costOfPermutation += 2 * Dungeons[perm[i]].crossingCost!.Value;

            // Se não for a última dungeon do trajeto, adicionar custo para a próxima dungeon
            if (i != perm.Length - 1)
            {
                path = AStar.FindPath(
                    overworld,
                    Dungeons[perm[i]].GetOverworldPoint(),
                    Dungeons[perm[i+1]].GetOverworldPoint()
                );

                if (path == null)
                    return int.MaxValue;

                costOfPermutation += path.Cost;
            }
        }

        // Custo da última dungeon até casa do Link
        path = AStar.FindPath(
            overworld,
            Dungeons[perm.Last()].GetOverworldPoint(),
            linksHome
        );

        if (path == null)
            return int.MaxValue;

        costOfPermutation += path.Cost;

        return costOfPermutation;
    }

    private void PopulateDungeonCosts()
    {
        for(int i = 0; i < dungeons.Count; i++)
        {
            dungeons[i].crossingCost = AStar.FindPath(
                dungeons[i].GetMap(),
                dungeons[i].GetStartPoint(),
                dungeons[i].GetObjetive()
            )?.Cost;
        }
    }
}
