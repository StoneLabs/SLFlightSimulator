using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
    public bool disableOnStart = true;

    void Start()
    {
        if (disableOnStart)
            this.gameObject.SetActive(false);
    }
}
