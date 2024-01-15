using UnityEngine;

namespace PCGNode
{
    public class TerrainNode
    {

        public struct FracMapConfig
        {
            public float persistence;
            public float lacunarity;
            public float initialScale;
            public int seed;

            public FracMapConfig(float persistence, float lacunarity, float initialScale, int seed)
            {
                this.persistence = persistence;
                this.lacunarity = lacunarity;
                this.initialScale = initialScale;
                this.seed = seed;
            }

            public static FracMapConfig Default
            {
                get
                {
                    return new FracMapConfig(
                        0.5f, 2.0f, 2.0f, 0
                    );
                }
            }
        }

        // public static FracMapConfig 

        public static float[] FracMap(
            int mapSize,
            FracMapConfig config)
        {
            float[] map = new float[mapSize * mapSize];
            var prng = new System.Random(config.seed);
            float persistence = config.persistence;
            float lacunarity = config.lacunarity;
            float initialScale = config.initialScale;


            int numOctaves = Mathf.CeilToInt(Mathf.Log(mapSize, 2));
            // Debug.Log(mapSize + " " + numOctaves);
            Vector2[] offsets = new Vector2[numOctaves];
            for (int i = 0; i < numOctaves; i++)
            {
                offsets[i] = new Vector2(prng.Next(-1000, 1000), prng.Next(-1000, 1000));
            }
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    float noiseValue = 0;
                    float scale = initialScale;
                    float weight = 1;
                    for (int i = 2; i < numOctaves; i++)
                    {
                        Vector2 p =
                        offsets[i] +
                        new Vector2(x / (float)mapSize, y / (float)mapSize) * scale;
                        // noiseValue += Mathf.PerlinNoise (p.x, p.y) * weight;
                        noiseValue += (PCGNode.NoiseNode.noise_3d0(p.x, p.y) * 0.5f + 0.5f) * weight;
                        weight *= persistence;
                        scale *= lacunarity;
                    }
                    map[y * mapSize + x] = noiseValue;
                    minValue = Mathf.Min(noiseValue, minValue);
                    maxValue = Mathf.Max(noiseValue, maxValue);
                }
            }

            // Normalize
            if (maxValue != minValue)
            {
                for (int i = 0; i < map.Length; i++)
                {
                    map[i] = (map[i] - minValue) / (maxValue - minValue);
                }
            }
            return map;
        }

        public delegate float MapGenerator(int i, int j);

        public static float[] CustomMap(int mapSize, MapGenerator generator)
        {
            float[] heightmap = new float[mapSize * mapSize];
            for (int j = 0; j < mapSize; j++)
            {
                for (int i = 0; i < mapSize; i++)
                {
                    heightmap[j * mapSize + i] = generator(i, j);
                }
            }
            return heightmap;
        }
    }
}