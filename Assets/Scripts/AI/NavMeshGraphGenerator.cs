using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class NavMeshGraphGenerator : MonoBehaviour
{
    [SerializeField] private MapGenerator roomGenerator;

    private PointGraph currentPointGraph;

    public void ClearNavMesh()
    {
        // This holds all graph data
        AstarData data = AstarPath.active.data;
        
        // This creates a Point Graph
        currentPointGraph ??= data.AddGraph(typeof(PointGraph)) as PointGraph;

        currentPointGraph.nodes = null;
    }
    
    public void GenerateNavMesh(MapGenerator roomGenerator)
    {
        this.roomGenerator = roomGenerator;
        GenerateNavMesh();
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
            var map = roomGenerator.GetGrid();
            var nodes = new PointNode[map.GetLength(0)][];
            for (int index = 0; index < map.GetLength(0); index++)
            {
                nodes[index] = new PointNode[map.GetLength(1)];
            }

            // add the point nodes
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    var tile = map[i, j];
                    PointNode node = null;
                    if (tile.Empty)
                    {
                        node = currentPointGraph.AddNode(
                            (Int3)roomGenerator.GetSmallGridTileWorldCoordinates(map[i, j].XCoord, map[i, j].YCoord));
                    }
                    nodes[i][j] = node;
                }
            }
            
            // add connections
            // horizontal
            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 1; j < nodes[0].Length; j++)
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
            for (int i = 0; i < nodes[0].Length; i++)
            {
                for (int j = 1; j < nodes.Length; j++)
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
}
