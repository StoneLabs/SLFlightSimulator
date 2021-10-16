using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    /// <summary>
    /// Generates texture from Grayscale float map. Uses white, black gradient.
    /// </summary>
    /// <param name="map">input float map [0, 1]</param>
    /// <returns></returns>
    public static Texture2D TextureFromGrayscaleMap(float[,] map)
    {
        return TextureFromGrayscaleMap(map, (value) => Color.Lerp(Color.black, Color.white, value));
    }

    /// <summary>
    /// Generates Texture from grayscale using specified gradient
    /// </summary>
    /// <param name="map">input float map [0, 1]</param>
    /// <param name="gradient">color gradient for usage</param>
    /// <returns></returns>
    public static Texture2D TextureFromGrayscaleMap(float[,] map, Gradient gradient)
    {
        return TextureFromGrayscaleMap(map, (value) => gradient.Evaluate(value));
    }

    /// <summary>
    /// Generate Texture from grayscale using specified mapping function
    /// </summary>
    /// <param name="map">input float map</param>
    /// <param name="mapper">mapping function</param>
    /// <returns></returns>
    public static Texture2D TextureFromGrayscaleMap(float[,] map,  System.Func<float, Color> mapper)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        return TextureFromColorMap(ColorArrayFromGrayscaleMap(map, mapper), width, height);
    }


    /// <summary>
    /// Generate Color array from grayscale using black and white gradient
    /// </summary>
    /// <param name="map">input float map [0, 1]</param>
    /// <returns></returns>
    public static Color[] ColorArrayFromGrayscaleMap(float[,] map)
    {
        return ColorArrayFromGrayscaleMap(map, (value) => Color.Lerp(Color.black, Color.white, value));
    }

    /// <summary>
    /// Generate Color array from grayscale using specified gradient
    /// </summary>
    /// <param name="map">input float map [0, 1]</param>
    /// <param name="gradient">gradient for color mapping</param>
    /// <returns></returns>
    public static Color[] ColorArrayFromGrayscaleMap(float[,] map, Gradient gradient)
    {
        return ColorArrayFromGrayscaleMap(map, (value) => gradient.Evaluate(value));
    }

    /// <summary>
    /// Generate Color array from specified mapping
    /// </summary>
    /// <param name="map">input float map</param>
    /// <param name="mapper">gradient for color mapping</param>
    /// <returns></returns>
    public static Color[] ColorArrayFromGrayscaleMap(float[,] map, System.Func<float, Color> mapper)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = mapper.Invoke(map[x, y]);

        return colorMap;
    }

    /// <summary>
    /// Generates Texture from color array with specified width and height. Note height * width must equal map length.
    /// </summary>
    /// <param name="map">input float map</param>
    /// <param name="height">width</param>
    /// <param name="height">height</param>
    /// <returns></returns>
    public static Texture2D TextureFromColorMap(Color[] map, int width, int height)
    {
        if (width * height != map.Length)
            throw new System.ArgumentException("Width and height do not equal input length");

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(map);
        texture.Apply();

        return texture;
    }
}
