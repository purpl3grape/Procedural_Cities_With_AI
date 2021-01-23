using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveState
{
    MovingToTarget,
    ArrivedAtTarget
}

public class PlayerMove : MonoBehaviour
{
    Grid GridReference;
    PathFinding pathFinding;
    Transform tr;
    Rigidbody rb;

    public int currentFinalPathNumber = 0;
    public Vector3 directionToNode;
    public float MaxSpeed = 10;
    public float speed = 10;
    public LayerMask playerLayerMask;

    public PlayerMoveState playerMoveState = PlayerMoveState.ArrivedAtTarget;
    public List<Node> FinalPath;

    Node lastNodeVisited;

    private void Awake()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
        GridReference = FindObjectOfType<Grid>();
        pathFinding = GridReference.GetComponent<PathFinding>();
    }

    // Update is called once per frame
    private void Update()
    {
        
        rb.angularDrag = 0;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.drag = 0;
        
        if (playerMoveState == PlayerMoveState.ArrivedAtTarget)
        {
            if (Vector3.Distance(tr.position, pathFinding.TargetPosition.position) > 2)
            {
                FinalPath = pathFinding.FindPath(tr.position, pathFinding.TargetPosition.position);//Find a path to the goal
                currentFinalPathNumber = 0;
                playerMoveState = PlayerMoveState.MovingToTarget;
            }
        }

        if (playerMoveState == PlayerMoveState.MovingToTarget)
        {
            MovePlayer();

            if (FinalPath.Count == 0)
            {
                playerMoveState = PlayerMoveState.ArrivedAtTarget;
            }

            if (refreshTargetPath == null)
            {
                refreshTargetPath = RefreshTargetPath();
                StartCoroutine(refreshTargetPath);
            }
        }
    }

    private IEnumerator refreshTargetPath;
    private IEnumerator RefreshTargetPath()
    {
        yield return new WaitForSeconds(4);
        lastNodeVisited.bIsPlayer = false;

        playerMoveState = PlayerMoveState.ArrivedAtTarget;
        refreshTargetPath = null;
    }

    float distanceToNode = 0.1f;
    RaycastHit hit;
    float accelRate;
    float decelRate;
    float targetAccelRate = 45;
    float targetDecelRate = 45;
    private void MovePlayer()
    {
        if (FinalPath.Count > 0)
        {
            //yield return new WaitForSeconds(3);
            Node n;
            if (currentFinalPathNumber >= FinalPath.Count)
            {
                playerMoveState = PlayerMoveState.ArrivedAtTarget;
                return;
            }

            n = FinalPath[currentFinalPathNumber];
            directionToNode = n.vPosition - tr.position;

            lastNodeVisited = FinalPath[currentFinalPathNumber];
            lastNodeVisited.bIsPlayer = true;

            Ray ray = new Ray(tr.position, directionToNode.normalized);
            if (Physics.Raycast(ray, out hit, 2 + MaxSpeed * Time.deltaTime, playerLayerMask))
            {
                accelRate = 0f;

                Debug.Log("Hit Something: " + hit.transform.name);
                speed = Mathf.Lerp(MaxSpeed, 0f, decelRate / targetDecelRate);
                decelRate += Time.fixedDeltaTime * 100;
                //speed = 0;

                FinalPath = pathFinding.FindPath(tr.position, pathFinding.TargetPosition.position);//Find a path to the goal
                lastNodeVisited.bIsPlayer = false;
                currentFinalPathNumber = 0;

            }
            else
            {
                decelRate = 0f;

                speed = Mathf.Lerp(1f,MaxSpeed, accelRate / targetAccelRate);
                accelRate += Time.fixedDeltaTime * 100;
            }

            if (directionToNode.magnitude > distanceToNode)
            {
                if (directionToNode.normalized.magnitude * speed * Time.deltaTime < directionToNode.magnitude)
                {
                    //Debug.Log("Moving from: " + tr.position + ", to: " + FinalPath[currentFinalPathNumber].vPosition);
                    tr.position += directionToNode.normalized * speed * Time.deltaTime;
                }
                else
                {
                    //may get rid off
                    tr.position = n.vPosition;
                }
            }
            else if (directionToNode.magnitude < .1f)
            {
                tr.position = n.vPosition;


                lastNodeVisited = FinalPath[currentFinalPathNumber];
                lastNodeVisited.bIsPlayer = false;
                if (currentFinalPathNumber < FinalPath.Count - 1)
                {
                    //Get Last Node Visited and set that to Free up, so no player in it.

                    currentFinalPathNumber++;

                    //Set the current to bIsPlayuer to true for the Node after updating to the next the node within the final path.
                
                //FinalPath[currentFinalPathNumber].bIsPlayer = true;
                    //lastNodeVisited.bIsPlayer = false;
                    
                //if (currentFinalPathNumber == 0)
                    //    FinalPath[currentFinalPathNumber - 1].bIsPlayer = false;
                    //else
                    //    FinalPath[currentFinalPathNumber - 1].bIsPlayer = false;
                    //if (currentFinalPathNumber == FinalPath.Count - 1)
                    //{
                    //    distanceToNode = .1f;
                    //}
                    //else
                    //{
                    //    distanceToNode = 0.1f;
                    //}
                }
                else
                {
                    //Debug.Log("Reached Here");
                    currentFinalPathNumber = 0;
                    //lastNodeVisited.bIsPlayer = false;
                    playerMoveState = PlayerMoveState.ArrivedAtTarget;
                }
            }




        }
    }

    //public LayerMask PlayerAndWallLayerMask;
    //private void CheckPlayerObstacles()
    //{
    //    //NodeArray = new Node[iGridSizeX, iGridSizeY];//Declare the array of nodes.
    //    Vector3 bottomLeft = GridReference.transform.position - Vector3.right * GridReference.vGridWorldSize.x / 2 - Vector3.forward * GridReference.vGridWorldSize.y / 2;//Get the real world position of the bottom left of the grid.
    //    for (int x = 0; x < GridReference.iGridSizeX; x++)//Loop through the array of nodes.
    //    {
    //        for (int y = 0; y < GridReference.iGridSizeY; y++)//Loop through the array of nodes
    //        {
    //            Vector3 worldPoint = bottomLeft + Vector3.right * (x * GridReference.fNodeDiameter + GridReference.fNodeRadius) + Vector3.forward * (y * GridReference.fNodeDiameter + GridReference.fNodeRadius);//Get the world co ordinates of the bottom left of the graph
    //            bool Wall = true;//Make the node a wall
    //
    //            //If the node is not being obstructed
    //            //Quick collision check against the current node and anything in the world at its position. If it is colliding with an object with a WallMask,
    //            //The if statement will return false.
    //            if (Physics.CheckSphere(worldPoint, GridReference.fNodeRadius, PlayerAndWallLayerMask))
    //            {
    //                Wall = false;//Object is not a wall
    //            }
    //
    //            GridReference.NodeArray[x, y] = new Node(Wall, worldPoint, x, y);//Create a new node in the array.
    //        }
    //    }
    //}
}
