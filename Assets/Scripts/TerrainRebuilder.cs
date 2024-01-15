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

    [ContextMenu("Refresh Terrain")]
    public void RefreshTerrain()
    {
        int width = ModifiedHeightMap.width;
        float[] heightmap = PCGNode.PackNode.Unpack(ModifiedHeightMap);
        // Blur
        // Smooth
        if (EnableSmooth)
        {
            for (int i = 0; i < BlurTimes; i++)
            {
                heightmap = PCGNode.BlurNode.BlurSimple(heightmap, width);
            }
        }
        heightmap = PCGNode.TerrainTool.Flip(heightmap, width);
        TerrainObject.RefreshHeightmapToTerrain(heightmap, width, TerrainObject.GetComponent<Terrain>());
    }
}
