using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureRenderer : MonoBehaviour
{
    public void DrawTexture(Texture2D texture)
    {
        // Use renderer or find one
        Renderer renderer = GetComponent<Renderer>();

        // Apply texture to material
        renderer.sharedMaterial.mainTexture = texture;
    }
}
