using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    public Texture2D HeightmapTex;
    [HideInInspector]
    public float[] heightStream;

    // public Texture2D RivermapTex;

    // public string RiveredSavePath = "RiverHeightmap";

    [ContextMenu("Generate River")]
    public float[] GenerateRiver()
    {
        string SavePath = GameConfig.RIVERMAP_PATH;
        int width = GameConfig.MAP_WIDTH;
        float[] heightmap;
        if (GameConfig.USE_STREAM)
        {
            heightmap = heightStream;
        }
        else
        {
            heightmap = PCGNode.PackNode.Unpack(HeightmapTex);
        }
        float[] rivermap = PCGNode.RiverNode.GenerateRiver(heightmap, width, PCGNode.MaskNode.EmptyMask(width), PCGNode.MaskNode.EmptyMask(width));
        if (!GameConfig.USE_STREAM)
        {
            PCGNode.PackNode.PackAndSave(rivermap, width, SavePath);
        }
        return rivermap;
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
