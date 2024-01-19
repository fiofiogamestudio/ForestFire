using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapModifier : MonoBehaviour
{
    [Header("HeightMap and RiverMap")]
    public Texture2D HeightMap;
    public Texture2D RiverMap;
    [Header("River Influence")]
    [Range(0.0f, 1.0f)]
    public float baseHeight = 0.1f;








    // private AnimationCurve CurveMappingX2 = AnimationCurve.




    [ContextMenu("Modify HeightMap")]
    public void ModifyHeightMap()
    {
        int width = HeightMap.width;
        // load height and river
        float[] heightmap = PCGNode.PackNode.Unpack(HeightMap);
        float[] rivermap = PCGNode.PackNode.Unpack(RiverMap);

        // River Influence
        for (int i = 0; i < width * width; i++)
        {
            // heightmap[i] = heightmap[i] * (1 - baseHeight) + baseHeight;
            heightmap[i] -= rivermap[i];
            heightmap[i] = heightmap[i] < 0 ? 0 : heightmap[i];
        }

        // Save
        PCGNode.PackNode.SaveTexture2D(PCGNode.PackNode.Pack(heightmap, width), GameConfig.MODIFIED_HEIGHTMAP_PATH);
    }
}
