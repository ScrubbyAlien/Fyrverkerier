using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using QuikGraph;
using UnityEngine.Tilemaps;

public class GraphAsset : ScriptableObject
{
    [SerializeField, HideInInspector]
    private AdjacencyGraph<GridTileVertex, GridTileEdge> gridGraph;
    public AdjacencyGraph<GridTileVertex, GridTileEdge> GetGraphByValue() => gridGraph.Clone();
    
    public void GenerateGraph(Tilemap navigationalMap, NeighbourType neighbourType) {
        navigationalMap.CompressBounds();

        AdjacencyGraph<GridTileVertex, GridTileEdge> newGridGraph = new();

        foreach (Vector3Int coord in AllCoords(navigationalMap)) {
            Tile tile = navigationalMap.GetTile<Tile>(coord);
            if (!tile) continue;
            
            GridTileVertex newVertex = new GridTileVertex(tile, coord);
            newGridGraph.AddVertex(newVertex);
            
            foreach ((Tile, Vector3Int) neighbour in AllNeighbours(navigationalMap, neighbourType, coord)) {
                GridTileVertex neighbourVertex = new GridTileVertex(neighbour.Item1, neighbour.Item2);
                newGridGraph.AddVertex(neighbourVertex);
                newGridGraph.AddEdge(new GridTileEdge(newVertex, neighbourVertex));
            }
        }
        
        gridGraph = newGridGraph;
    }

    private static readonly Vector3Int[] cardinalNeighbours = new [] {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 1, 0),
    };
    
    private static readonly Vector3Int[] diagonalNeighbours = new [] {
        new Vector3Int(-1, -1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),
    };
    
    private IEnumerable AllNeighbours(Tilemap map, NeighbourType neighbourType, Vector3Int origin) {
        switch (neighbourType) {
            case NeighbourType.Diagonal:
                foreach (Vector3Int diagonalNeighbour in diagonalNeighbours) {
                    Tile neighbor = map.GetTile<Tile>(origin + diagonalNeighbour);
                    if (neighbor) yield return (neighbor, origin + diagonalNeighbour);
                }
                goto case NeighbourType.Cardinal;
            case NeighbourType.Cardinal:
                foreach (Vector3Int cardinalNeighbour in cardinalNeighbours) {
                    Tile neighbor = map.GetTile<Tile>(origin + cardinalNeighbour);
                    if (neighbor) yield return (neighbor, origin + cardinalNeighbour);
                }
                break;
        }
    }

    private IEnumerable AllCoords(Tilemap map) {
        for (int x = map.cellBounds.xMin; x < map.cellBounds.xMax; x++) {
            for (int y = map.cellBounds.yMin; y < map.cellBounds.yMax; y++) {
                yield return new Vector3Int(x, y, 0);
            }
        }
    }
}

public struct GridTileVertex
{
    public Tile tile;
    public Vector3Int coord;

    public GridTileVertex(Tile tile, Vector3Int coord) {
        this.tile = tile;
        this.coord = coord;
    }
}

public struct GridTileEdge : IEdge<GridTileVertex>
{
    public GridTileVertex Source => source;
    public GridTileVertex Target => target;
    private GridTileVertex source;
    private GridTileVertex target;
    
    public GridTileEdge(GridTileVertex source, GridTileVertex target) {
        this.source = source;
        this.target = target;
    }
    
}

public enum NeighbourType
{
    [LabelText("Cardinal", SdfIconType.ArrowsMove)] Cardinal, 
    [LabelText("Diagonal", SdfIconType.ArrowsFullscreen)] Diagonal
}
