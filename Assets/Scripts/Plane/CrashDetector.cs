using UnityEngine;

class CrashDetector : MonoBehaviour
{
    public PlaneManager manager;
    public string LandableTag = "";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != LandableTag)
        {
            Debug.Log("Crash: " + other.tag + " != " + LandableTag);
            manager.Crash();
        }
    }
}
