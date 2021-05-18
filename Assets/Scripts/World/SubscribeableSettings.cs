using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeableSettings : ScriptableObject
{
    private System.Action listener = null;

    public bool HasSubscriber
    {
        get { return listener != null; }
    }

    public void Subscribe(System.Action action)
    {
        //if (action != null)
        //    Debug.LogWarning("Warning SubscriptableSettings listener has been overwritten.");

        listener = action;
    }

    private void OnValidate()
    {
        if (listener != null)
            listener.Invoke();
    }
}
