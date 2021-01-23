using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetBlock : MonoBehaviour
{
    public LayerMask hitLayers;

    Vector3 mouse;
    private Ray castPoint;
    private RaycastHit hit;
    private Transform tr;

    private void Start()
    {
        tr = transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouse = Input.mousePosition;
            castPoint = Camera.main.ScreenPointToRay(mouse);    //Case ray to get wherer the mouse is pointing at

            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers))
            {
                tr.position = new Vector3(hit.point.x, 0, hit.point.z);    //Move the target to the mouse position;
            }
        }
    }
}
