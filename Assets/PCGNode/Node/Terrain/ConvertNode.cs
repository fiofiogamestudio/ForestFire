using UnityEngine;

namespace PCGNode
{
    public static partial class Terrain
    {
        public static Texture2D ConvertHeightmapToNormalMap(Texture2D heightMap, float strength = 1.0f)
        {
            int width = heightMap.width;
            int height = heightMap.height;
            Texture2D normalMap = new Texture2D(width, height, TextureFormat.ARGB32, true);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float xLeft = heightMap.GetPixel(x - 1 < 0 ? 0 : x - 1, y).grayscale;
                    float xRight = heightMap.GetPixel(x + 1 >= width ? width - 1 : x + 1, y).grayscale;
                    float yUp = heightMap.GetPixel(x, y - 1 < 0 ? 0 : y - 1).grayscale;
                    float yDown = heightMap.GetPixel(x, y + 1 >= height ? height - 1 : y + 1).grayscale;
                    float xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    float yDelta = ((yUp - yDown) + 1) * 0.5f;
                    normalMap.SetPixel(x, y, new Color(xDelta, yDelta, strength, 1.0f));
                }
            }
            normalMap.Apply();
            return normalMap;
        }
    }

}