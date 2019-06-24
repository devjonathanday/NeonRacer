using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform anchor; //Child of the player object, used for positioning
    public Transform focus; //Actual player object, used for rotation

    public float positionLerp;
    public float rotationLerp;

    RaycastHit intersection;
    Vector3 desiredPosition;
    public float offset;

    void Update()
    {
        //Checks for a line from the focus to where the camera *should* be
        //If that line is intersected by an object, place the camera there
        //Otherwise, place the camera on the anchor
        if (Physics.Raycast(focus.position, (anchor.position - focus.position), out intersection, (anchor.position - focus.position).magnitude + offset))
            desiredPosition = intersection.point + ((anchor.position - focus.position).normalized * -offset);
        else desiredPosition = anchor.position;
        //Camera does not change height
        desiredPosition.y = anchor.position.y;
        //~Lerp for smoothness
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionLerp);
        transform.LookAt(focus, Vector3.up);
    }
}