using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    public class PathFinding : MonoBehaviour
    {
        Grid grid;
        public Transform StartPosition;
        public Transform TargetPosition;

        private void Awake()
        {
            grid = GetComponent<Grid>();
        }

        private void Update()
        {
            FindPath(StartPosition.position, TargetPosition.position);
        }

        private void FindPath(Vector3 _startPosition, Vector3 _targetPosition)
        {
            Node StartNode = grid.GetClosestNodeFromWorldPosition(_startPosition);
            Node TargetNode = grid.GetClosestNodeFromWorldPosition(_targetPosition);

            List<Node> OpenList = new List<Node>();
            HashSet<Node> ClosedList = new HashSet<Node>();

            //Initialized the Starting Node by adding it as the first entry in the open list
            //Starting with that 1 node, each of its neighboring Nodes will be subsequently added in later in the function
            OpenList.Add(StartNode);

            //Recursive Part
            while (OpenList.Count > 0)
            {
                Node CurrentNode = OpenList[0];
                //Debug.Log("Assessing Start");

                for (int i = 1; i < OpenList.Count; i++)
                {
                    Debug.Log("Assessing OpenList");

                    if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].hCost < CurrentNode.hCost)
                    {
                        CurrentNode = OpenList[i];
                    }
                }
                OpenList.Remove(CurrentNode);
                ClosedList.Add(CurrentNode);

                if (CurrentNode == TargetNode)
                {
                    GetFinalPath(StartNode, TargetNode);
                }

                //Searching each Neighbor and assessing their G Costs (Movement so far Costs), and their H Costs (How far to go Cost)
                foreach (Node NeighborNode in grid.GetNeighboringNodes(CurrentNode))
                {
                    //Debug.Log("Assessing Neighbors");
                    if (!NeighborNode.IsWall || ClosedList.Contains(NeighborNode))
                    {
                        //Debug.Log("Wall");
                        continue;
                    }
                    int MoveCost = CurrentNode.gCost + GetManhattenDistance(CurrentNode, NeighborNode);
                    Debug.Log("Move Cost: " + MoveCost);
                    //Assess Current Node's Neighbors whose moveCost is already lower than, or if it is not already in the open list.
                    if (MoveCost < NeighborNode.gCost || !OpenList.Contains(NeighborNode))
                    {
                        Debug.Log("Move Cost is less than Neighbor G cost");

                        NeighborNode.gCost = MoveCost;
                        NeighborNode.hCost = GetManhattenDistance(NeighborNode, TargetNode);
                        NeighborNode.Parent = CurrentNode;

                        if (!OpenList.Contains(NeighborNode))
                        {
                            //Current Node's Neighbors are now added to OpenList
                            OpenList.Add(NeighborNode);
                            Debug.Log("Adding Neighbor ");

                        }
                    }
                }
                //Debug.Log("OpenList Count: " + OpenList.Count);
            }
        }

        private int GetManhattenDistance(Node nodeA, Node nodeB)
        {
            int deltaX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int deltaY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            return deltaX + deltaY;
        }

        private void GetFinalPath(Node _startNode, Node _targetNode)
        {
            List<Node> FinalPath = new List<Node>();
            Node currNode = _targetNode;

            while (currNode != _startNode)
            {
                FinalPath.Add(currNode);
                currNode = currNode.Parent;
            }

            FinalPath.Reverse();

            grid.FinalPath = FinalPath;
            Debug.Log("Final Path Found: " + grid.FinalPath.Count);
        }
    }
}