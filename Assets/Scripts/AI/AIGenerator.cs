using System.Collections;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles all AI related procedural content generation.
/// </summary>
public class AIGenerator : MonoBehaviour
{
    [SerializeField] private MapGenerator roomGenerator;

    private PointGraph currentPointGraph;

    private MapGenerator.GridTile[,] gridTiles;

    /// <summary>
    /// Removes the current navmesh if present.
    /// </summary>
    public void ClearGraph()
    {
        if (!AstarPath.active) return;
        
        // This holds all graph data
        AstarData data = AstarPath.active.data;
        
        // This creates a Point Graph
        currentPointGraph ??= data.AddGraph(typeof(PointGraph)) as PointGraph;

        currentPointGraph.nodes = null;
    }
    
    /// <summary>
    /// Initializes the AI generator.
    /// </summary>
    /// <param name="roomGenerator">Procedural content generator which generates a room</param>
    public void Init(MapGenerator roomGenerator)
    {
        this.roomGenerator = roomGenerator;
    }
    
    /// <summary>
    /// Generates a navigation point graph to be used by the astar path.
    /// </summary>
    public void GenerateGraph()
    {
        // This holds all graph data
        AstarData data = AstarPath.active.data;

        // This creates a Point Graph
        currentPointGraph ??= data.AddGraph(typeof(PointGraph)) as PointGraph;

        currentPointGraph.nodes = null;
        
        AstarPath.active.Scan(currentPointGraph);

        // Make sure we only modify the graph when all pathfinding threads are paused
        AstarPath.active.AddWorkItem(new AstarWorkItem(_ => 
        {
            // Do all of the generation here
            gridTiles = roomGenerator.GetGrid();
            var nodes = new PointNode[gridTiles.GetLength(0)][];
            for (int index = 0; index < gridTiles.GetLength(0); index++)
            {
                nodes[index] = new PointNode[gridTiles.GetLength(1)];
            }

            // add the point nodes
            for (int i = 0; i < gridTiles.GetLength(0); i++)
            {
                for (int j = 0; j < gridTiles.GetLength(1); j++)
                {
                    var tile = gridTiles[i, j];
                    PointNode node = null;
                    if (tile.Empty)
                    {
                        node = currentPointGraph.AddNode(
                            (Int3)roomGenerator.GetSmallGridTileWorldCoordinates(gridTiles[i, j].XCoord, gridTiles[i, j].YCoord));
                    }
                    nodes[i][j] = node;
                }
            }
            
            // add connections
            var rowCount = nodes.Length;
            var columnCount = nodes[0].Length;
            
            // horizontal
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 1; j < columnCount; j++)
                {
                    // if both this and prev tile empty (i.e. nodes aren't null)
                    if (nodes[i][j - 1] != null && nodes[i][j] != null)
                    {
                        // add connection
                        var cost = (uint)(nodes[i][j - 1].position - nodes[i][j].position).costMagnitude;
                        nodes[i][j].AddConnection(nodes[i][j - 1], cost);
                        nodes[i][j - 1].AddConnection(nodes[i][j], cost);
                    }
                }
            }
            // vertical
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 1; j < rowCount; j++)
                {
                    // if both this and prev tile empty (i.e. nodes aren't null)
                    if (nodes[j - 1][i] != null && nodes[j][i] != null)
                    {
                        // add connection
                        var cost = (uint)(nodes[j - 1][i].position - nodes[j][i].position).costMagnitude;
                        nodes[j][i].AddConnection(nodes[j - 1][i], cost);
                        nodes[j - 1][i].AddConnection(nodes[j][i], cost);
                    }
                }
            }
        }));

        // Run the above work item immediately
        AstarPath.active.FlushWorkItems();
    }

    /// <summary>
    /// Randomly generates enemy spawn points in the current room.
    /// </summary>
    /// <param name="numEnemies">The number of spawn points to generate</param>
    /// <returns>The generated spawn points</returns>
    public Vector3[] GenerateEnemySpawnPoints(int numEnemies)
    {
        if (gridTiles == null)
        {
            Debug.LogError("Generate navmesh first, before trying to generate enemy spawn points");
            return null;
        }
        
        var spawnPoints = new Vector3[numEnemies];

        var pointsGenerated = 0;
        while (pointsGenerated < numEnemies)
        {
            // get a random tile coords = throw dart
            var x = Random.Range(0, gridTiles.GetLength(0));
            var y = Random.Range(0, gridTiles.GetLength(1));
            
            // if tile is empty, place enemy
            if (IsNeighbourhoodEmpty(x, y))
            {
                spawnPoints[pointsGenerated] = roomGenerator.GetSmallGridTileWorldCoordinates(
                    gridTiles[x, y].XCoord, 
                    gridTiles[x, y].YCoord
                    );
                
                // fill the tile
                gridTiles[x, y].Empty = false;
                
                ++pointsGenerated;
            }
        }

        return spawnPoints;
    }

    private bool IsNeighbourhoodEmpty(int x, int y, int radius = 1)
    {
        var rowMin = Mathf.Max(0, x - radius);
        var rowMax = Mathf.Min(x + radius, gridTiles.GetLength(0));
            
        var colMin = Mathf.Max(0, y - radius);
        var colMax = Mathf.Min(y + radius, gridTiles.GetLength(1));

        for (int i = rowMin; i < rowMax; i++)
        {
            for (int j = colMin; j < colMax; j++)
            {
                if (!gridTiles[i, j].Empty) return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// Removes the current astar path from the scene - should be called before instantiating a prefab which
    /// already has an astar path component.
    /// </summary>
    public void CleanUp(UnityAction onEnd)
    {
        StartCoroutine(CleanUpInternal(onEnd));
    }

    private IEnumerator CleanUpInternal(UnityAction onEnd)
    {
        Destroy(FindObjectOfType<AstarPath>());
        
        // remove any queued updates
        AstarPath.active.FlushGraphUpdates();
        AstarPath.active.FlushWorkItems();
        
        yield return new WaitForEndOfFrame(); // wait for any remaining work to complete
        
        onEnd.Invoke();
    }
}
