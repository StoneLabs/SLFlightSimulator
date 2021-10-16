using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves a oject constantly by translation (not physics based)
/// </summary>
public class ConstantMovement : MonoBehaviour
{
    public Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += movement * Time.deltaTime;
    }
}
