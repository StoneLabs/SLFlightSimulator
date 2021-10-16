using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disables gameobject on start
/// </summary>
public class DisableOnStart : MonoBehaviour
{
    public bool disableOnStart = true;

    void Start()
    {
        if (disableOnStart)
            this.gameObject.SetActive(false);
    }
}
