using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    public class Node
    {
        public int gridX;
        public int gridY;

        public bool IsWall;
        public Vector3 Position;

        public Node Parent;

        public int gCost;   //Distance From Starting Node, How far we travelled
        public int hCost;   //Distance From End Node, How far left

        public int FCost { get { return gCost + hCost; } }

        //Constructor
        public Node(bool a_IsWall, Vector3 a_Pos, int a_gridX, int a_gridY)
        {
            IsWall = a_IsWall; //Is the node is obstructed
            Position = a_Pos;
            gridX = a_gridX;
            gridY = a_gridY;
        }
    }
}