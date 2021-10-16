using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Toggles Master audio (pauses it) on pressing specified key
/// </summary>
public class AudioListenerToggle : MonoBehaviour
{
    public KeyCode key = KeyCode.M;

    private void Update()
    {
        if (Input.GetKeyDown(key))
            AudioListener.pause = !AudioListener.pause;
    }
}
