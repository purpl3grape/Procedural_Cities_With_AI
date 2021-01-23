using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Purplegrapestudios
{
    public static class PlacementHelper
    {
        public static List<Direction> FindNeighbor(Vector3Int position, ICollection<Vector3Int> collection)
        {
            List<Direction> neighBorDirections = new List<Direction>();
            if(collection.Contains(position + Vector3Int.right))
            {
                neighBorDirections.Add(Direction.Right);
            }
            if (collection.Contains(position + Vector3Int.left))
            {
                neighBorDirections.Add(Direction.Left);
            } 
            if (collection.Contains(position + new Vector3Int(0, 0, 1)))
            {
                neighBorDirections.Add(Direction.Up);
            }
            if (collection.Contains(position - new Vector3Int(0, 0, 1)))
            {
                neighBorDirections.Add(Direction.Down);
            }

            return neighBorDirections;
        }

        internal static Vector3Int GetOffsetFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Vector3Int(0, 0, 1);
                case Direction.Down:
                    return new Vector3Int(0, 0, -1);
                case Direction.Left:
                    return Vector3Int.left;
                case Direction.Right:
                    return Vector3Int.right;
                default:
                    break;
            }
            throw new System.Exception("No direction such as " + direction);
        }

        public static Direction GetReverseDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    break;
            }
            throw new System.Exception("No direction such as " + direction);
        }
    }
}