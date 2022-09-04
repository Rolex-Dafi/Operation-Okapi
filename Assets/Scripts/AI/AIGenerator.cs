using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class AIGenerator : MonoBehaviour
{
    [SerializeField] private MapGenerator roomGenerator;

    private PointGraph currentPointGraph;

    private MapGenerator.GridTile[,] gridTiles;

    public void ClearNavMesh()
    {
        // This holds all graph data
        AstarData data = AstarPath.active.data;
        
        // This creates a Point Graph
        currentPointGraph ??= data.AddGraph(typeof(PointGraph)) as PointGraph;

        currentPointGraph.nodes = null;
    }
    
    public void Init(MapGenerator roomGenerator)
    {
        this.roomGenerator = roomGenerator;
    }
    
    public void GenerateNavMesh()
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
            
            // TODO add diagonal connections in both directions for smoother AI pathing
            
            // Add 2 nodes and connect them
            /*var node1 = graph.AddNode((Int3) new Vector3(1, 2, 3));
            var node2 = graph.AddNode((Int3) new Vector3(4, 5, 6));
            var cost1 = (uint)(node2.position - node1.position).costMagnitude;
            node1.AddConnection(node2, cost1);
            node2.AddConnection(node1, cost1);*/
        }));

        // Run the above work item immediately
        AstarPath.active.FlushWorkItems();
    }

    public Vector3[] GenerateEnemySpawnPoints(int numEnemies = 5) // TODO set this from level data
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
}
