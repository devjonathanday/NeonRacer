using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCrossfader : MonoBehaviour
{
    public AudioSource[] audioTones;
    public AnimationCurve[] audioBlendCurves;
    public Vector2[] pitchRanges;

    float pitchVar;
    public float pitch
    {
        get { return pitchVar; }
        set
        {
            pitchVar = value;

            for(int i = 0; i < audioTones.Length; i++)
            {
                audioTones[i].volume = audioBlendCurves[i].Evaluate(pitch);
                audioTones[i].pitch = MathFunctions.GetProportionalLerp(Vector2.up, pitchRanges[i], pitch);
            }
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.M))
        {
            pitch += 0.01f;
            if (pitch > 1) pitch = 1;
        }
        if (Input.GetKey(KeyCode.N))
        {
            pitch -= 0.01f;
            if (pitch < 0) pitch = 0;
        }
    }
}