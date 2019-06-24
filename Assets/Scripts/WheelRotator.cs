using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    public CarController car;
    public float rotationBounds;

    Vector3 desiredRotation;

    void Start()
    {
        desiredRotation = Vector3.zero;
    }

    void Update()
    {
        desiredRotation.x = -rotationBounds * car.rotationForce;
        desiredRotation.y = transform.localEulerAngles.y;
        desiredRotation.z = transform.localEulerAngles.z;
        transform.localEulerAngles = desiredRotation;
    }
}
