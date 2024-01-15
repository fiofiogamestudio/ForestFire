using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCGNode;

public class TestErosion : MonoBehaviour
{
    public Texture2D HeightmapTex;
    public string SavePath = "erodedHeightmap";

    [ContextMenu("Erode")]
    public void Erode()
    {
        float[] heightmap = PackNode.Unpack(HeightmapTex);

        // ErosionNode.ErosionConfig config = ErosionNode.ErosionConfigBuilder.Default;

        // heightmap = ErosionNode.Erode(heightmap, HeightmapTex.width, config);
        ErosionNode.Erode(heightmap, HeightmapTex.width, 100000);

        PackNode.SaveTexture2D(PackNode.Pack(heightmap, HeightmapTex.width), SavePath);
    }
}
