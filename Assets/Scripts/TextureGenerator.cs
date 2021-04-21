using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromGrayscaleMap(float[,] map)
    {
        return TextureFromGrayscaleMap(map, (value) => Color.Lerp(Color.black, Color.white, value));
    }

    public static Texture2D TextureFromGrayscaleMap(float[,] map, Gradient gradient)
    {
        return TextureFromGrayscaleMap(map, (value) => gradient.Evaluate(value));
    }

    public static Texture2D TextureFromGrayscaleMap(float[,] map,  System.Func<float, Color> mapper)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = mapper.Invoke(map[x, y]);

        return TextureFromColorMap(colorMap, width, height);
    }

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
