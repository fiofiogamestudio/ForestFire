using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PremapGenerator : MonoBehaviour
{
    public Texture2D HeightmapTex;

    public string NormapMapPath = "Temp/NormalMap";
    public string SlopeMapPath = "Temp/SlopeMap";
    public string FlowMapPath = "Temp/FlowMap";

    public Texture2D NormalMap;


    public Texture2D SlopeMap;
    public Texture2D RiverMap;

    [ContextMenu("Generate NormalMap")]
    public void GenerateNormalMap() // very slow
    {
        int width = HeightmapTex.width;
        float[] heightmap = PCGNode.PackNode.Unpack(HeightmapTex);
        float[] xs, ys, zs;
        PCGNode.FaceNode.CalcNormal(heightmap, HeightmapTex.width, out xs, out ys, out zs, width);
        for (int i = 0; i < 10; i++)
        {
            xs = PCGNode.BlurNode.BlurSimple(xs, width);
            ys = PCGNode.BlurNode.BlurSimple(ys, width);
            zs = PCGNode.BlurNode.BlurSimple(zs, width);

        }
        PCGNode.PackNode.PackAndSaveNormal(xs, ys, zs, HeightmapTex.width, NormapMapPath);
    }

    [ContextMenu("Generate SlopeMap")]
    public void GenerateSlopeMap()
    {
        int width = HeightmapTex.width;
        float[] xs, ys, zs;
        PCGNode.PackNode.UnpackNormal(NormalMap, out xs, out ys, out zs);
        float[] slopes = new float[width * width];

        float[] rivers = PCGNode.PackNode.Unpack(RiverMap);
        for (int i = 0; i < width * width; i++)
        {
            float slope = zs[i] / Mathf.Sqrt(xs[i] * xs[i] + ys[i] * ys[i]);
            slope = Mathf.Sqrt(slope);
            slopes[i] = slope;
        }

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                if (rivers[i + j * width] != 0 ||
               rivers[i + 1 + j * width] != 0 ||
               rivers[i - 1 + j * width] != 0 ||
               rivers[i + (j + 1) * width] != 0 ||
               rivers[i + (j - 1) * width] != 0 ||
               rivers[i - 1 + (j - 1) * width] != 0 ||
               rivers[i + 1 + (j - 1) * width] != 0 ||
               rivers[i - 1 + (j + 1) * width] != 0 ||
               rivers[i + 1 + (j + 1) * width] != 0)
                    slopes[i + j * width] = 0;
            }
        }

        // for (int i = 0; i < 3; i++)
        // {
        // slopes = PCGNode.BlurNode.BlurSimple(slopes, width);
        // }

        PCGNode.PackNode.PackAndSave(slopes, width, SlopeMapPath);
    }

    // [ContextMenu("GenerateFuelmap")]
    // public void GenerateFuelMap()
    // {
    //     int width = HeightmapTex.width;
    //     float[] heightmap = PCGNode.PackNode.Unpack(HeightmapTex);
    //     float[] slopes = PCGNode.PackNode.Unpack(SlopeMap);

    //     float[] fuels = new float[width * width];
    //     for (int i = 0; i < width * width; i++)
    //     {
    //         fuels[i] = slopes[i] * heightmap[i];
    //         // fuels[i] = fuels[i] * fuels[i] + 0.5f;
    //     }
    //     PCGNode.TerrainTool.Normalize(fuels);

    //     PCGNode.PackNode.PackAndSave(fuels, width, "FuelMap");
    // }


    [ContextMenu("Generate Flowmap")]
    public void GenerateFlowmap()
    {
        Texture2D flowmap = PCGNode.TextureAlgo.CalcFlowMapCPU_WaterShed(HeightmapTex);
        PCGNode.PackNode.SaveTexture2D(flowmap, FlowMapPath);
    }
}
