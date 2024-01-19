using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    public Texture2D HeightmapTex;

    // public Texture2D RivermapTex;

    // public string RiveredSavePath = "RiverHeightmap";

    [ContextMenu("Generate River")]
    public void GenerateRiver()
    {
        string SavePath = GameConfig.RIVERMAP_PATH;
        int width = HeightmapTex.width;
        float[] heightmap = PCGNode.PackNode.Unpack(HeightmapTex);
        float[] rivermap = PCGNode.RiverNode.GenerateRiver(heightmap, width, PCGNode.MaskNode.EmptyMask(width), PCGNode.MaskNode.EmptyMask(width));
        Texture2D RivermapTex = PCGNode.PackNode.Pack(rivermap, width);
        PCGNode.PackNode.SaveTexture2D(RivermapTex, SavePath);
    }

    // [ContextMenu("Generate River Height")]
    // public void GenerateRiverHeight()
    // {
    //     int width = HeightmapTex.width;

    //     float[] heightmap = PCGNode.PackNode.Unpack(HeightmapTex);
    //     float[] rivermap = PCGNode.PackNode.Unpack(RivermapTex);

    //     heightmap = PCGNode.MathNode.Influence(heightmap, rivermap, -100.0f);

    //     PCGNode.PackNode.PackAndSave(heightmap, width, RiveredSavePath);

    // }

    // [ContextMenu("Debug Output")]
    // public void DebugOutput()
    // {
    //     int width = HeightmapTex.width;

    //     float[] heightmap = PCGNode.PackNode.Unpack(HeightmapTex);

    //     PCGNode.PackNode.PackAndSave(heightmap, width, "Test");
    // }
}
