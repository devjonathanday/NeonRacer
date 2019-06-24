using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctions
{ 
    //Returns value within B proportional to known currentValue within A
    public static float GetProportionalLerp(Vector2 a, Vector2 b, float currentValue)
    {
        return ((b.y - b.x) * ((currentValue - a.x) / (a.y - a.x))) + b.x;
    }
}