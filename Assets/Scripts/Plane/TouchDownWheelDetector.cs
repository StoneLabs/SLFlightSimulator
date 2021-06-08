using UnityEngine;

public class TouchDownWheelDetector : MonoBehaviour
{
    private uint touchDownCount = 0;
    public bool IsTouchDown
    {
        get
        {
            return touchDownCount > 0;
        }
    }

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
