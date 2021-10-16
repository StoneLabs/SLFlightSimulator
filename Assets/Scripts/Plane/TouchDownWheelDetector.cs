using UnityEngine;

/// <summary>
/// Wheel touchdown detection
/// </summary>
public class TouchDownWheelDetector : MonoBehaviour
{
    // TouchDown count. Increses when trigger is entered, decreases when trigger is exited
    private uint touchDownCount = 0;
    public bool IsTouchDown
    {
        get
        {
            return touchDownCount > 0;
        }
    }

    // Setting
    [Header("Debug Visualization")]
    public bool visualize;
    public string wheelName;
    public float x, y;

    private void OnTriggerEnter(Collider other)
    {
        touchDownCount++;
    }
    private void OnTriggerExit(Collider other)
    {
        touchDownCount--;
    }

    public void OnGUI()
    {
        if (visualize)
            GUI.Label(new Rect(x, y, 200, 20), IsTouchDown ? $"{wheelName} - TOUCH DOWN" : $"{wheelName} - IN AIR");
    }
}
