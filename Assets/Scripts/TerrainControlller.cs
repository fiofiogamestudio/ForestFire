using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainControlller : MonoBehaviour
{
    public Terrain MyTerrain;
    public Texture2D HeightmapTex;

    void Awake()
    {
        MyTerrain = GetComponent<Terrain>();
    }

    [ContextMenu("Update Terrain")]
    public void UpdateTerrain()
    {
        int width = HeightmapTex.width;
        float[] heightmap = PCGNode.PackNode.Unpack(HeightmapTex);
        RefreshHeightmapToTerrain(heightmap, width, MyTerrain);
    }

    public void RefreshHeightmapToTerrain(float[] heightmap, int width, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        if (terrainData.heightmapResolution != width)
        {
            terrainData.heightmapResolution = width;
        }

        float[,] heightmap2D = new float[width, width];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                heightmap2D[i, j] = heightmap[j * width + i];
            }
        }
        terrainData.SetHeights(0, 0, heightmap2D);
    }




    // [Header("Landscape")]
    // public Texture2D GrassTex;
    // public void GenerateLayer()
    // {

    // }
}
