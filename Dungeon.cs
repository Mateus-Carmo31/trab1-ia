public class Dungeon
{
  private Map.Tile hyruleLocation;
  private Map.Tile innerStartLocation;
  private Map.Tile pendantLocation;
  private Map map;

  public Dungeon(Map.Tile locationOnMap, Map.Tile startInside, Map.Tile pendant, string mapData)
  {
    this.hyruleLocation = locationOnMap;
    this.innerStartLocation = startInside;
    this.pendantLocation = pendant;
    string mapString = System.IO.File.ReadAllText($@"{mapData}");
    this.map = new Map(mapString, 28, 28);
  }

  public Map.Tile GetHyruleLocation() => this.hyruleLocation;

  public Map.Tile GetInnerStartLocation() => this.innerStartLocation;

  public Map.Tile GetPendantLocation() => this.pendantLocation;

  public Map GetMap() => this.map;

  public override string ToString()
  {
    return $"{this.hyruleLocation.x}/{this.hyruleLocation.y}";
  }
}