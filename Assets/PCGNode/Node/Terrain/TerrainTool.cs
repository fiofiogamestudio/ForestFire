using UnityEngine;

namespace PCGNode
{
    public static class TerrainTool
    {
        public static int[] GetRandomIndices(int mapSize, int num)
        {
            int[] randomIndices = new int[num];
            for (int i = 0; i < num; i++)
            {
                int randomX = Random.Range(0, mapSize);
                int randomY = Random.Range(0, mapSize);
                randomIndices[i] = randomY * mapSize + randomX;
            }
            return randomIndices;
        }

        public static float[] Normalize(float[] heightmap)
        {
            float min_height = 1.0f;
            float max_height = 0.0f;
            int length = heightmap.Length;
            for (int i = 0; i < length; i++)
            {
                min_height = heightmap[i] < min_height ? heightmap[i] : min_height;
                max_height = heightmap[i] > max_height ? heightmap[i] : max_height;
            }
            float step_height = max_height - min_height;
            for (int i = 0; i < length; i++)
            {
                heightmap[i] = (heightmap[i] - min_height) / step_height;
            }
            return heightmap;
        }

        public static float[] Flip(float[] heightmap, int width)
        {
            float[] flipmap = new float[width * width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    flipmap[i + j * width] = heightmap[j + i * width];
                }
            }
            return flipmap;
        }
    }
}