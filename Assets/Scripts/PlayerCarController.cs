using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerCarController : MonoBehaviour
{
    public float throttle; // Right trigger 0..1
    public float brakes;   // Left trigger  0..1
    public float steer;    // Left stick   -1..1

    public float steerAngle;
    public float maxSteerAngle;

    public float horsepower;

    public WheelCollider LFWheelCol, LRWheelCol, RFWheelCol, RRWheelCol;
    public Transform LFWheelT, LRWheelT, RFWheelT, RRWheelT;
    

    void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos = wheelTransform.position;
        Quaternion rot = wheelTransform.rotation;

        wheelCollider.GetWorldPose(out pos, out rot);

        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    void FixedUpdate()
    {
        #region Input

        GamePadState controllerState = GamePad.GetState(PlayerIndex.One);

        throttle = controllerState.Triggers.Right;
        brakes = controllerState.Triggers.Left;
        steer = controllerState.ThumbSticks.Left.X;

        #endregion

        #region Steering

        LFWheelCol.steerAngle = steer * maxSteerAngle;
        RFWheelCol.steerAngle = steer * maxSteerAngle;

        #endregion

        #region Acceleration

        LRWheelCol.motorTorque = throttle * horsepower;
        RRWheelCol.motorTorque = throttle * horsepower;

        #endregion

        #region Wheel Visuals

        UpdateWheelPose(LFWheelCol, LFWheelT);
        UpdateWheelPose(LRWheelCol, LRWheelT);
        UpdateWheelPose(RFWheelCol, RFWheelT);
        UpdateWheelPose(RRWheelCol, RRWheelT);

        #endregion
    }
}