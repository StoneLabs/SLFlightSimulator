using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class RedOnCollider : MonoBehaviour
{
    Renderer renderer;
    public Color OnCollision;
    public Color OnFree;

    // Start is called before the first frame update
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
