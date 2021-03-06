﻿using UnityEngine;
using static MathFunctions;
using XInputDotNetPure;
public class CarController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rBody;
    public Transform[] wheels;

    [Header("Physics")]

    public Vector3 velocity;
    public float drag;

    public float brakingForce;
    public float turnSpeed; //How fast the steering angle can be changed
    public float rotationForce; //How fast to rotate the car (based on steering angle)
    public float rotationDrag; //How much drag to put on the steering wheel (returns wheel to center as well)

    [Header("Engine")]
    public Vector2 RPMRange; //X = minumum RPM, Y = maximum RPM
    public float currentRPM;
    public float engineSlowdown;

    public float horsepower; //How much RPM gets added every frame while at full throttle
    public AnimationCurve engineLoad; //How much load gets put on the engine by speed (Top speed = max load = no acceleration)

    [Header("Transmission")]
    public int[] topSpeedByGear; //Top speed at max RPM for each gear
    public int currentGear;

    [Header("Sound Effects")]
    public AudioSource mainEngineSound;
    public Vector2 pitchRange; //X = minumum RPM sound, Y = maximum RPM sound

    private bool[] buttonFramePresses;

    void Start()
    {
        if (!rBody) rBody = GetComponent<Rigidbody>();
        buttonFramePresses = new bool[] { false, false };
        //0 = B
        //1 = X
    }

    void Update()
    {
        #region Input

        GamePadState controllerState = GamePad.GetState(PlayerIndex.One);

        //if (Input.GetKey(KeyCode.W))
        //    currentRPM += horsepower;
        //else if (Input.GetKey(KeyCode.S))
        //    currentRPM -= Mathf.Min(brakingForce, currentRPM); //TODO this will break if minimum RPM is not 0!
        //else if (currentRPM > RPMRange.x) currentRPM -= Mathf.Min(engineSlowdown, currentRPM);

        //if (Input.GetKey(KeyCode.A))
        //    rotationForce -= turnSpeed;
        //if (Input.GetKey(KeyCode.D))
        //    rotationForce += turnSpeed;

        //if (Input.GetKeyDown(KeyCode.E) && currentGear < topSpeedByGear.Length)
        //{
        //    currentGear++;
        //    currentRPM = GetRPMFromTransmissionSpeed(topSpeedByGear[currentGear - 1], RPMRange.y, velocity.z);
        //}
        //if (Input.GetKeyDown(KeyCode.Q) && currentGear > 1)
        //{
        //    currentGear--;
        //    currentRPM = GetRPMFromTransmissionSpeed(topSpeedByGear[currentGear - 1], RPMRange.y, velocity.z);
        //}
        if (controllerState.Buttons.B == ButtonState.Released) buttonFramePresses[0] = false;
        if (controllerState.Buttons.X == ButtonState.Released) buttonFramePresses[1] = false;

        if (controllerState.Triggers.Right > 0)
            currentRPM += horsepower * controllerState.Triggers.Right * engineLoad.Evaluate(velocity.z);
        else if (controllerState.Triggers.Left > 0)
            currentRPM -= Mathf.Min(brakingForce * controllerState.Triggers.Left, currentRPM); //TODO this will break if minimum RPM is not 0!
        else if (currentRPM > RPMRange.x) currentRPM -= Mathf.Min(engineSlowdown, currentRPM);

        rotationForce += controllerState.ThumbSticks.Left.X * turnSpeed;

        if (!buttonFramePresses[0] && controllerState.Buttons.B == ButtonState.Pressed && currentGear < topSpeedByGear.Length)
        {
            buttonFramePresses[0] = true;
            currentGear++;
            currentRPM = GetRPMFromTransmissionSpeed(topSpeedByGear[currentGear - 1], RPMRange.y, velocity.z);
        }
        if (!buttonFramePresses[1] && controllerState.Buttons.X == ButtonState.Pressed && currentGear > 1)
        {
            buttonFramePresses[1] = true;
            currentGear--;
            currentRPM = GetRPMFromTransmissionSpeed(topSpeedByGear[currentGear - 1], RPMRange.y, velocity.z);
        }

        #endregion

        #region Physics

        if (currentRPM > RPMRange.y) currentRPM = RPMRange.y;

        velocity *= drag;
        rotationForce *= rotationDrag;

        velocity.z = GetTransmissionSpeedFromRPM(topSpeedByGear[currentGear - 1], RPMRange.y, currentRPM);

        transform.Translate(velocity * Time.deltaTime);
        transform.Rotate(transform.up * rotationForce);

        //Rotates wheels based on speed
        for (int i = 0; i < wheels.Length; i++) wheels[i].Rotate(Vector3.forward * velocity.z);

        #endregion

        #region End-Of-Frame Updates

        mainEngineSound.pitch = GetProportionalLerp(RPMRange, pitchRange, currentRPM);


        #endregion
    }

    float GetTransmissionSpeedFromRPM(float topSpeedForGear, float maxRPM, float currentRPM)
    {
        return (topSpeedForGear / maxRPM) * currentRPM;
    }
    float GetRPMFromTransmissionSpeed(float topSpeedForGear, float maxRPM, float currentSpeed)
    {
        return (maxRPM / topSpeedForGear) * currentSpeed;
    }

}

//Math Sector™

//To create a linear graph from a max X and max Y:
//y = (maxY / maxX) * x;
//Alternatively:
//x = (maxX / maxY) * y;