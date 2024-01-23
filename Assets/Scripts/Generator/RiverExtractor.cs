using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverExtractor : MonoBehaviour
{
    public Texture2D RawRiverMap;
    [HideInInspector]
    public float[] riverStream;
    [Range(0, 1)]
    public float threshold = 0.5f;

    [ContextMenu("Extract River")]
    public float[] ExtractRiver()
    {
        string SavePath = GameConfig.EXTRACTED_RIVERMAP_PATH;
        int width = GameConfig.MAP_WIDTH;
        int count = width * width;
        float[] rivermap;
        if (GameConfig.USE_STREAM)
        {
            rivermap = riverStream;
        }
        else
        {
            rivermap = PCGNode.PackNode.Unpack(RawRiverMap);
        }

        // Extracted

        for (int i = 0; i < count; i++)
        {
            if (rivermap[i] < threshold) rivermap[i] = 0;
            else rivermap[i] = 1;
        }

        // Simplify
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (checkToSimplify(ref rivermap, i, j))
                {
                    rivermap[i + j * width] = 0;
                }
            }
        }

        if (!GameConfig.USE_STREAM)
        {
            PCGNode.PackNode.PackAndSave(rivermap, width, SavePath);
        }

        return rivermap;
    }

    bool checkToSimplify(ref float[] map, int i, int j)
    {



        return false;
    }
}
