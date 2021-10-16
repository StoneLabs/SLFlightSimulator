using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes center of mass of attached rigidbody
/// </summary>
public class CenterOfMass : MonoBehaviour
{
    public bool OverrideCOM = false;
    public Vector3 NewCenterOfMass;
    [Range(0.0f, 1.0f)]
    public float VisualizationRadius = 0.2f;

    // Wether COM has been overwritten
    private bool overridenCOM; 

    void Update()
    {
        // Apply/reset COM to set state as needed
        if (overridenCOM != OverrideCOM)
        {
            Debug.Log("Changing COM");

            if (OverrideCOM)
                GetComponent<Rigidbody>().centerOfMass = NewCenterOfMass;
            else
                GetComponent<Rigidbody>().ResetCenterOfMass(); // original value

            overridenCOM = OverrideCOM;
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize COM
        Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, VisualizationRadius);
    }

}
