using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Purplegrapestudios
{
    public class StructureHelper : MonoBehaviour
    {
        public BuildingTypes[] buildingTypes;
        public GameObject[] naturePrefabs;
        public bool randomNaturePlacement = false;
        [Range(0, 1)]
        public float randomNaturePlacementThreshold = 0.3f;
        public Dictionary<Vector3Int, GameObject> structureDictionary = new Dictionary<Vector3Int, GameObject>();
        public Dictionary<Vector3Int, GameObject> natureDictionary = new Dictionary<Vector3Int, GameObject>();

        public void PlaceStructuresAroundRoad(List<Vector3Int> roadPositions)
        {
            //Direction in freeEstateSpots (Dictionary) points toward the road tile.
            Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpacesAroundRoad(roadPositions);
            List<Vector3Int> blockedPositions = new List<Vector3Int>();
            foreach(var freeSpot in freeEstateSpots)
            {
                if (blockedPositions.Contains(freeSpot.Key))
                {
                    continue;
                }
                var rotation = Quaternion.identity;

                switch (freeSpot.Value)
                {
                    case Direction.Up:
                        rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case Direction.Down:
                        rotation = Quaternion.Euler(0, -90, 0);
                        break;
                    case Direction.Left:
                        //Do Nothing
                        break;
                    case Direction.Right:
                        rotation = Quaternion.Euler(0, 180, 0);
                        break;
                    default:
                        break;
                }

                for(int i =0; i < buildingTypes.Length; i++)
                {
                    if (buildingTypes[i].quantity == -1)
                    {
                        if (randomNaturePlacement)
                        {
                            var random = UnityEngine.Random.value;
                            if(random < randomNaturePlacementThreshold)
                            {
                                var nature = SpawnPrefab(naturePrefabs[UnityEngine.Random.Range(0,naturePrefabs.Length)], freeSpot.Key, rotation);
                                natureDictionary.Add(freeSpot.Key, nature);
                                break;
                            }
                        }
                        var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
                        structureDictionary.Add(freeSpot.Key, building);
                        break;
                    }
                    if (buildingTypes[i].IsBuildingAvailable())
                    {
                        if (buildingTypes[i].sizeRequired > 1)
                        {
                            var halfSize = Mathf.FloorToInt(buildingTypes[i].sizeRequired / 2.0f);
                            List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
                            if (VerifyIfBuildingFits(halfSize, freeEstateSpots, freeSpot, blockedPositions, ref tempPositionsBlocked))
                            {
                                blockedPositions.AddRange(tempPositionsBlocked);
                                var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
                                structureDictionary.Add(freeSpot.Key, building);
                                foreach(var pos in tempPositionsBlocked)
                                {
                                    //Each of the blocked positions point to the structure
                                    if (!structureDictionary.ContainsKey(pos))
                                        structureDictionary.Add(pos, building);
                                }
                            }
                        }
                        else
                        {
                            var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
                            structureDictionary.Add(freeSpot.Key, building);
                        }
                        break;
                    }
                }
                //Instantiate(prefab, freeSpot.Key, rotation, transform);
            }
        }

        private bool VerifyIfBuildingFits(int halfSize, Dictionary<Vector3Int, Direction> freeEstateSpots, KeyValuePair<Vector3Int, Direction> freeSpot, List<Vector3Int> blockedPositions, ref List<Vector3Int> tempPositionsBlocked)
        {
            Vector3Int direction = Vector3Int.zero;
            if (freeSpot.Value == Direction.Down || freeSpot.Value == Direction.Up)
            {
                //Road above or below, building will be placed (left/right)
                direction = Vector3Int.right;
            }
            else
            {
                //Road left or right, building will be placed (up/down)
                direction = new Vector3Int(0, 0, 1);
            }
            for (int i = 1; i <= halfSize; i++)
            {
                //Check at 1, 0 is current position

                //Check next position to the left and next position to the right
                var pos1 = freeSpot.Key + direction * i;
                var pos2 = freeSpot.Key - direction * i;
                if(!freeEstateSpots.ContainsKey(pos1) || !freeEstateSpots.ContainsKey(pos2) || blockedPositions.Contains(pos1) || blockedPositions.Contains(pos2))
                {
                    //Not Free positions, so building doesn't fit
                    return false;
                }
                tempPositionsBlocked.Add(pos1);
                tempPositionsBlocked.Add(pos2);
            }
            return true;
        }

        private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation)
        {
            var newStructure = Instantiate(prefab, position, rotation, transform);
            return newStructure;
        }

        private Dictionary<Vector3Int, Direction> FindFreeSpacesAroundRoad(List<Vector3Int> roadPositions)
        {
            Dictionary<Vector3Int, Direction> freeSpaces = new Dictionary<Vector3Int, Direction>();
            foreach(var position in roadPositions)
            {
                var neighborDirections = PlacementHelper.FindNeighbor(position, roadPositions);

                foreach(Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    if(neighborDirections.Contains(direction) == false)
                    {
                        Vector3Int newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
                        if (freeSpaces.ContainsKey(newPosition))
                        {
                            continue;
                        }
                        freeSpaces.Add(newPosition, PlacementHelper.GetReverseDirection(direction));
                    }
                }
            }
            return freeSpaces;
        }
    }
}