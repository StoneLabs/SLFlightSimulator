using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes attached material on trigger enter/leave
/// </summary>
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class RedOnCollider : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    Renderer renderer;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public Color OnCollision;
    public Color OnFree;

    void Start()
    {
        this.renderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        this.renderer.material.color = OnCollision;
    }
    private void OnTriggerExit(Collider other)
    {
        this.renderer.material.color = OnFree;
    }
}
