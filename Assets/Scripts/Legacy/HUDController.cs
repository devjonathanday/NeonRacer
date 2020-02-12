using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static MathFunctions;

public class HUDController : MonoBehaviour
{
    [Header("Tachometer")]
    public Vector2 angleLimits;
    public RectTransform tachNeedle;
    public Image tachFill;
    public Vector2 fillRange;
    public float tachNeedleLerp;
    public TextMeshProUGUI currentGear;

    [Header("References")]
    public CarController car;

    void Update()
    {
        //Tachometer
        tachNeedle.rotation = Quaternion.Lerp(tachNeedle.rotation,
            Quaternion.AngleAxis(GetProportionalLerp(car.RPMRange, angleLimits, car.currentRPM), Vector3.forward), tachNeedleLerp);
        tachFill.fillAmount = Mathf.Lerp(tachFill.fillAmount, GetProportionalLerp(car.RPMRange, fillRange, car.currentRPM), tachNeedleLerp);
        currentGear.text = car.currentGear.ToString();
    }
}