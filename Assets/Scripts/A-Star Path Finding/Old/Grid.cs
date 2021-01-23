using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Old{
    public class Grid : MonoBehaviour
    {
        public Transform StartPosition;
        public LayerMask WallMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public float distance;

        Node[,] grid;
        public List<Node> FinalPath;

        float nodeDiameter;
        int gridSizeX, gridSizeY;



        private void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            CreateGrid();
        }

        public List<Node> GetNeighboringNodes(Node currentNode)
        {
            List<Node> NeighboringNodes = new List<Node>();
            int xCheck;
            int yCheck;

            //for (int x = -1; x <= 1; x++)
            //{
            //    for (int y = -1; y <= 1; y++)
            //    {
            //        //if we are on the node tha was passed in, skip this iteration.
            //        if (x == 0 && y == 0)
            //        {
            //            continue;
            //        }
            //
            //        xCheck = currentNode.gridX + x;
            //        yCheck = currentNode.gridY + y;
            //
            //        //Make sure the node is within the grid.
            //        if (xCheck >= 0 && xCheck < gridSizeX && yCheck >= 0 && yCheck < gridSizeY)
            //        {
            //            NeighboringNodes.Add(grid[xCheck, yCheck]); //Adds to the neighbours list.
            //        }
            //
            //    }
            //}

            //Right Side Neighbor
            xCheck = currentNode.gridX + 1;
            yCheck = currentNode.gridY;
            if (xCheck >= 0 && xCheck < gridSizeX)
            {
                if (yCheck >= 0 && yCheck < gridSizeY)
                {
                    NeighboringNodes.Add(grid[xCheck, yCheck]);
                }
            }

            //Left Side Neighbor
            xCheck = currentNode.gridX - 1;
            yCheck = currentNode.gridY;
            if (xCheck >= 0 && xCheck < gridSizeX)
            {
                if (yCheck >= 0 && yCheck < gridSizeY)
                {
                    NeighboringNodes.Add(grid[xCheck, yCheck]);
                }
            }

            //Front Side Neighbor
            xCheck = currentNode.gridX;
            yCheck = currentNode.gridY + 1;
            if (xCheck >= 0 && xCheck < gridSizeX)
            {
                if (yCheck >= 0 && yCheck < gridSizeY)
                {
                    NeighboringNodes.Add(grid[xCheck, yCheck]);
                }
            }

            //Back Side Neighbor
            xCheck = currentNode.gridX;
            yCheck = currentNode.gridY - 1;
            if (xCheck >= 0 && xCheck < gridSizeX)
            {
                if (yCheck >= 0 && yCheck < gridSizeY)
                {
                    NeighboringNodes.Add(grid[xCheck, yCheck]);
                }
            }

            return NeighboringNodes;
        }
        public Node GetClosestNodeFromWorldPosition(Vector3 _worldPosition)
        {
            //Get Position in Node Array
            float xPoint = ((_worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
            float yPoint = ((_worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y);

            xPoint = Mathf.Clamp01(xPoint);
            yPoint = Mathf.Clamp01(yPoint);

            int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
            int y = Mathf.RoundToInt((gridSizeY - 1) * yPoint);

            return grid[x, y];
        }

        private void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            for(int y = 0; y < gridSizeX; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool Wall = true;

                    if(Physics.CheckSphere(worldPoint, nodeRadius, WallMask))
                    {
                        Wall = false;
                        Debug.Log("Node  Wall");
                    }

                    Debug.Log("Node Not Wall");
                    //grid[x, y] = new Node(Wall, worldPoint, x, y);
                    grid[y, x] = new Node(Wall, worldPoint, x, y);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, .5f, gridWorldSize.y));

            if(grid != null)
            {
                foreach(Node node in grid)
                {
                    if (node.IsWall)
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                    }

                    if(FinalPath != null)
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawCube(node.Position, new Vector3((nodeDiameter - distance), .5f, (nodeDiameter - distance)));//  Vector3.one * (nodeDiameter - distance));
                }
            }
        }
    }
}