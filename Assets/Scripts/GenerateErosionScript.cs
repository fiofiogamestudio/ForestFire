using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateErosionScript : MonoBehaviour
{
    public Texture2D HeightMap;
    public int ErosionTimes = 1; // per pixel
    public string SavePath;
    [ContextMenu("Erode")]
    void EditorErode()
    {
        System.DateTime beginTime = System.DateTime.Now;
        float[] map = PCGNode.PackNode.Unpack(HeightMap);
        PCGNode.ErosionNode.Erode(map, HeightMap.width, HeightMap.width * HeightMap.width * ErosionTimes);
        Debug.Log("Erode time : " + System.DateTime.Now.Subtract(beginTime).TotalMilliseconds);
        PCGNode.PackNode.SaveTexture2D(PCGNode.PackNode.Pack(map, HeightMap.width), SavePath);
    }
}
