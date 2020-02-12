using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnginePreview : MonoBehaviour
{
    [System.Serializable]
    public class PistonAttributes
    {
        public float timeOffset;
        [HideInInspector] public Vector3 direction;
        [HideInInspector] public Vector3 startPosition;
    }

    float time;
    [SerializeField] float simulationSpeed;
    [SerializeField] float strokeDistance;
    [SerializeField] float strokeOffset;
    [SerializeField] PistonAttributes[] pistons;
    [SerializeField] Transform[] pistonTransforms;
    [SerializeField] Transform[] rods;
    [SerializeField] Transform crankshaft;
    [SerializeField] TextMeshProUGUI tachometer;
    public AudioSource fire;

    void Start()
    {
        for (int i = 0; i < pistons.Length; i++)
        {
            pistons[i].startPosition = pistonTransforms[i].position;
            pistons[i].direction = pistonTransforms[i].forward;
        }
    }

    void Update()
    {
        time += Time.deltaTime * simulationSpeed;
        crankshaft.localEulerAngles = new Vector3(time * 360, 90, 0);
    }

    void LateUpdate()
    {
        for (int i = 0; i < pistons.Length; i++)
        {
            pistonTransforms[i].position =
                pistons[i].startPosition +
                (pistons[i].direction * Mathf.Rad2Deg * (Mathf.Sin((time + pistons[i].timeOffset) * Mathf.PI * 2) + 1) * strokeDistance) * Mathf.Deg2Rad +
                (pistons[i].direction * strokeOffset);
        }

        for (int i = 0; i < rods.Length; i++)
        {
            rods[i].rotation =
                Quaternion.Euler(rods[i].eulerAngles.x,
                rods[i].eulerAngles.y,
                Mathf.Rad2Deg * Mathf.Atan2(pistonTransforms[i].position.y - rods[i].position.y, pistonTransforms[i].position.x - rods[i].position.x) + 180);
        }
        tachometer.text = "RPM: " + simulationSpeed * 60;
    }
}