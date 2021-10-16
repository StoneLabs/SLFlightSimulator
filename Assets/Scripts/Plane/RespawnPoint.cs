using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Respawn point object. Needs to be added to planeManager to be used
/// </summary>
public class RespawnPoint : MonoBehaviour
{
    public bool spawnInMotion = false;
    public string spawnName = "Respawn point";
    public KeyCode spawnKey = KeyCode.F1;

    private void OnDrawGizmos()
    {
        // Visualize respawn point using sphere gizmos
        GizmosUtils.SetT(transform);
        Gizmos.DrawSphere(Vector3.zero, 2.0f);
        GizmosUtils.DrawArrow(Vector3.zero, transform.forward, 5.0f);
    }
}
