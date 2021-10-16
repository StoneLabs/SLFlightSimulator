using UnityEngine;

/// <summary>
/// Performs crash detection using triggers. Ignored LandableTag
/// </summary>
class CrashDetector : MonoBehaviour
{
    public PlaneManager manager;
    public string LandableTag = "";

    private void OnTriggerEnter(Collider other)
    {
        // If collided object is not landable
        if (other.tag != LandableTag)
        {
            Debug.Log("Crash: " + other.tag + " != " + LandableTag);
            manager.Crash(); // Crash plane
        }
    }
}
