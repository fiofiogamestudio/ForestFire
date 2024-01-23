using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapModifier : MonoBehaviour
{
    [Header("HeightMap and RiverMap")]
    public Texture2D HeightMap;
    public Texture2D RiverMap;
    [HideInInspector]
    public float[] heightStream;
    [HideInInspector]
    public float[] riverStream;

    [Header("Smooth")]
    public bool EnableSmooth = true;
    public int BlurTimes = 20;

    [Header("River Influence")]
    [Range(0.0f, 1.0f)]
    public float baseHeight = 0.1f;



    [ReadOnly]
    public bool EnableRiver = true;



    // private AnimationCurve CurveMappingX2 = AnimationCurve.




    [ContextMenu("Modify HeightMap")]
    public float[] ModifyHeightMap()
    {
        int width = GameConfig.MAP_WIDTH;
        // load height and river
        float[] heightmap;
        float[] rivermap;
        if (GameConfig.USE_STREAM)
        {
            heightmap = heightStream;
            rivermap = riverStream;
        }
        else
        {
            heightmap = PCGNode.PackNode.Unpack(HeightMap);
            rivermap = PCGNode.PackNode.Unpack(RiverMap);
        }

        // Smooth
        if (EnableSmooth)
        {
            for (int i = 0; i < BlurTimes; i++)
            {
                heightmap = PCGNode.BlurNode.BlurSimple(heightmap, width);
            }
        }

        // River Influence
        if (EnableRiver)
        {
            for (int i = 0; i < width * width; i++)
            {
                // heightmap[i] = heightmap[i] * (1 - baseHeight) + baseHeight;
                heightmap[i] -= rivermap[i];
                heightmap[i] = heightmap[i] < 0 ? 0 : heightmap[i];
            }
        }

        if (!GameConfig.USE_STREAM)
        {
            // Save
            PCGNode.PackNode.SaveTexture2D(PCGNode.PackNode.Pack(heightmap, width), GameConfig.MODIFIED_HEIGHTMAP_PATH);
        }
        return heightmap;
    }
}
