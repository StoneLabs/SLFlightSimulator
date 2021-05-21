using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public bool OverrideCOM = false;
    public Vector3 NewCenterOfMass;
    [Range(0.0f, 1.0f)]
    public float VisualizationRadius = 0.2f;

    private bool overridenCOM;

    // Update is called once per frame
    void Update()
    {
        if (overridenCOM != OverrideCOM)
        {
            Debug.Log("Changing COM");

            if (OverrideCOM)
                GetComponent<Rigidbody>().centerOfMass = NewCenterOfMass;
            else
                GetComponent<Rigidbody>().ResetCenterOfMass();

            overridenCOM = OverrideCOM;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, VisualizationRadius);
    }

}
