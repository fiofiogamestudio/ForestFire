using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRebuilder : MonoBehaviour
{
    [Header("Auto Refresh (Terrain)")]
    public Texture2D ModifiedHeightMap;
    [HideInInspector]
    public float[] heightStream;
    public TerrainControlller TerrainObject;





    [Header("Edge")]
    public bool EnableEdge = true;


    [ContextMenu("Refresh Terrain")]
    public void RefreshTerrain()
    {
        int width = GameConfig.MAP_WIDTH;
        float[] heightmap;
        if (GameConfig.USE_STREAM)
        {
            heightmap = heightStream;
        }
        else
        {
            heightmap = PCGNode.PackNode.Unpack(ModifiedHeightMap);
        }



        // scale
        if (GameConfig.TERRAIN_HEIGHT_SCALE != 1.0f)
        {
            for (int i = 0; i < heightmap.Length; i++)
            {
                heightmap[i] = heightmap[i] * GameConfig.TERRAIN_HEIGHT_SCALE;
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
