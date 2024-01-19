using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRebuilder : MonoBehaviour
{
    [Header("Auto Refresh (Terrain)")]
    public Texture2D ModifiedHeightMap;
    public TerrainControlller TerrainObject;

    [Header("Smooth")]
    public bool EnableSmooth = true;

    public int BlurTimes = 3;

    [Header("Edge")]
    public bool EnableEdge = true;


    [ContextMenu("Refresh Terrain")]
    public void RefreshTerrain()
    {
        int width = ModifiedHeightMap.width;
        float[] heightmap = PCGNode.PackNode.Unpack(ModifiedHeightMap);

        // Smooth
        if (EnableSmooth)
        {
            for (int i = 0; i < BlurTimes; i++)
            {
                heightmap = PCGNode.BlurNode.BlurSimple(heightmap, width);
            }
        }

        // Edge
        if (EnableEdge)
        {
            int edged_width = width + 1;
            float[] edgedmap = new float[edged_width * edged_width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i != width - 1 && j != width - 1)
                    {
                        edgedmap[(i + 1) + (j + 1) * edged_width] = heightmap[i + j * width];
                    }
                }
            }

            edgedmap = PCGNode.TerrainTool.Flip(edgedmap, edged_width);
            TerrainObject.RefreshHeightmapToTerrain(edgedmap, edged_width, TerrainObject.GetComponent<Terrain>());
        }
        else
        {
            heightmap = PCGNode.TerrainTool.Flip(heightmap, width);
            TerrainObject.RefreshHeightmapToTerrain(heightmap, width, TerrainObject.GetComponent<Terrain>());
        }
    }
}
