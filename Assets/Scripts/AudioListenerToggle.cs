using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioListenerToggle : MonoBehaviour
{
    public KeyCode key = KeyCode.M;

    private void Update()
    {
        if (Input.GetKeyDown(key))
            AudioListener.pause = !AudioListener.pause;


    }
}
