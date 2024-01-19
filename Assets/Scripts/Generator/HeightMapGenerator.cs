using System.ComponentModel;
using UnityEngine;

[ExecuteInEditMode]
public class HeightMapGenerator : MonoBehaviour
{
    public int seed = 0;

    [Header("Erosion")]
    public bool EnableErosion = true;

    public float ErosionMultiplier = 0.1f;

    [Header("Curve Mapping")]
    public bool EnableCurveMapping = true;
    private AnimationCurve MappingMountain
    {
        get
        {
            return AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }
    private AnimationCurve MappingLessMountain
    {
        get
        {
            AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            curve.keys[1].time = 0.5f;
            curve.keys[1].value = 1f;
            curve.keys[1].inTangent = 0f;
            return curve;
        }
    }

    private AnimationCurve MappingMoreMountain
    {
        get
        {
            AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            curve.keys[0].time = 0f;
            curve.keys[0].value = 0f;
            curve.keys[0].outTangent = 0f;
            return curve;
        }
    }

    [System.Serializable]
    public enum CurveMappingType
    {
        Mountain,
        LessMountain,
        MoreMountain
    }
    public CurveMappingType MappingType = CurveMappingType.Mountain;
    public AnimationCurve CurveMapping = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    [ContextMenu("Generate HeightMap")]
    public void GenerateHeightMap()
    {
        int width = GameConfig.MAP_WIDTH;
        string SavePath = GameConfig.HEIGHTMAP_PATH;

        PCGNode.TerrainNode.FracMapConfig config = PCGNode.TerrainNode.FracMapConfig.Default;
        config.seed = seed;

        float[] heightmap = PCGNode.TerrainNode.FracMap(width, config);


        // (1) Erosion
        if (EnableErosion)
        {
            heightmap = PCGNode.ErosionNode.Erode(heightmap, width, (int)(width * width * ErosionMultiplier));
        }

        // (2) Curve Mapping
        AnimationCurve curveMapping = null;
        switch (MappingType)
        {
            case CurveMappingType.Mountain:
                curveMapping = MappingMountain;
                break;
            case CurveMappingType.LessMountain:
                curveMapping = MappingLessMountain;
                break;
            case CurveMappingType.MoreMountain:
                curveMapping = MappingMoreMountain;
                break;
        }
        if (curveMapping != null)
        {
            heightmap = PCGNode.MappingNode.Mapping(heightmap, x => curveMapping.Evaluate(x));
        }

        PCGNode.PackNode.SaveTexture2D(PCGNode.PackNode.Pack(heightmap, width), SavePath);
    }
}