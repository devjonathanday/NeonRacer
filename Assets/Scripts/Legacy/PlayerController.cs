using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    GamePadState controllerState;

    [Header("Input")]
    public float throttle; //0 to 1 based on right trigger
    public float brakes; //0 to 1 based on left trigger
    public float steering; //-1 to 1 based on left stick

    [Header("Attributes")]
    public float horsepower; //Acceleration
    public float brakingForce; //Deceleration
    public float maxSteeringTorque; //Amount of rotational torque applied when turning
    public float reverseVelocity; //Amount of force added when reversing
    public float groundCheckDistance;
    public LayerMask groundLayers;

    public AnimationCurve steeringAmount;

    public float tractionLimit; //The speed at which the car cannot hold full traction
    public float topSpeed;
    public float speed; //Car speed, taken from rBody velocity

    [Header("Visuals")]
    private Vector3 wheelRotation;
    public float maxWheelAngle;
    private float currentWheelAngle;
    public float steeringLerp;

    [Header("References - Self")]
    public Rigidbody rBody;
    public Transform LFWheel, RFWheel, LRWheel, RRWheel;
    public AudioCrossfader engineSFX;

    [Header("References - Prefabs")]
    public GameObject wallDragEffect;

    private void Start()
    {
        wheelRotation = new Vector3(0, 90, 0);
    }

    void Update()
    {
        #region Input

        controllerState = GamePad.GetState(PlayerIndex.One);
        throttle = controllerState.Triggers.Right;
        brakes = controllerState.Triggers.Left;
        steering = controllerState.ThumbSticks.Left.X;

        #endregion

        #region Audio

        engineSFX.pitch = Mathf.Lerp(0, 1, speed / topSpeed);

        #endregion
    }

    private void FixedUpdate()
    {
        #region Physics

        speed = rBody.velocity.magnitude;

        //Re-orient the player to move in the current direction
        rBody.velocity = new Vector3(transform.forward.x * speed, rBody.velocity.y, transform.forward.z * speed);

        if (Physics.Raycast(transform.position, -transform.up, groundCheckDistance, groundLayers))
        {
            //Acceleration
            if (speed < topSpeed)
                rBody.velocity += transform.forward * throttle * horsepower;

            //Braking
            rBody.velocity -= transform.forward * brakes * brakingForce;

            //Steering
            //transform.Rotate(Vector3.up, steering * maxSteeringTorque);
            rBody.AddTorque(transform.up * steering * steeringAmount.Evaluate(speed) * maxSteeringTorque);
        }
        Debug.DrawRay(transform.position, -transform.up * groundCheckDistance, Color.red, 0.02f);

        #endregion

        #region Visuals

        wheelRotation.x = 0;
        wheelRotation.z += speed;

        LRWheel.localEulerAngles = wheelRotation;
        RRWheel.localEulerAngles = wheelRotation;

        currentWheelAngle = Mathf.Lerp(currentWheelAngle, (-steering * maxWheelAngle), steeringLerp);
        wheelRotation.x = currentWheelAngle;

        LFWheel.localEulerAngles = wheelRotation;
        RFWheel.localEulerAngles = wheelRotation;

        #endregion
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Instantiate(wallDragEffect, collision.contacts[0].point, Quaternion.identity);
            speed = rBody.velocity.magnitude;
        }
    }
}

//Old physics update
/*
#region Physics

speed = rBody.velocity.magnitude;

        //Re-orient the player to move in the current direction
        rBody.velocity = transform.forward* speed;

        //Acceleration
        if (speed<topSpeed)
            rBody.velocity += transform.forward* throttle* horsepower;

        //Braking/Reversing
        if (rBody.velocity.z > 0)
            rBody.velocity -= transform.forward* brakes * brakingForce;
        else rBody.velocity -= transform.forward* brakes * reverseVelocity;

//Steering
rBody.AddTorque(transform.up* steering * steeringAmount.Evaluate(speed) * maxSteeringTorque);

#endregion
*/

//Update with traction
/*
 void Update()
    {
        #region Input

        controllerState = GamePad.GetState(PlayerIndex.One);
        throttle = controllerState.Triggers.Right;
        brakes = controllerState.Triggers.Left;
        steering = controllerState.ThumbSticks.Left.X;

        #endregion

        #region Physics

        rBody.velocity += transform.forward * throttle * horsepower;
        speed = rBody.velocity.magnitude;
            rBody.velocity = Vector3.Lerp(transform.forward * rBody.velocity.magnitude, rBody.velocity,
              GetProportionalLerp(tractionLimit, topSpeed, 0, 1, speed));
        rBody.AddTorque(transform.up * steering * steeringTorque);

        if (brakes > 0)
        {
            if (speed > 0) rBody.velocity /= 1 + (brakes * brakingForce);
            else rBody.velocity += -transform.forward * horsepower;
        }
        
        


        #endregion
    }
*/
