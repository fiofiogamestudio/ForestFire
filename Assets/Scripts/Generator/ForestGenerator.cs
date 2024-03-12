using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    private float TerrainHeight = 200;
    public Texture2D HeightMap;

    public Texture2D NormalMap;

    public Texture2D SlopeMap;

    public GameObject TreePrefab;
    public GameObject BushPrefab;

    public Transform ForestRoot;


    // public

    [ContextMenu("Generate Forest")]
    public void GenerateForest()
    {
        generateForestByParam(NormalMap, SlopeMap);
        // for (int i = 0; i < 1000; i++)
        // {
        //     for (int j = 0; j < 1000; j++)
        //     {
        //         if (i % 10 != 0 || j % 10 != 0) continue;
        //         float height = heightmap[i + j * width];
        //         GameObject tree = GameObject.Instantiate(TreePrefab, new Vector3(j, height * TerrainHeight, i), Quaternion.identity);
        //         tree.transform.SetParent(transform);
        //         // tree.transform.localScale = Vector3.one * 500;
        //         tree.transform.localRotation = Quaternion.identity;
        //     }
        // }
    }

    public void generateForestByParam(Texture2D normalMap, Texture2D slopeMap)
    {
        ClearForest();
        int width = HeightMap.width;
        float[] heightmap = PCGNode.PackNode.Unpack(HeightMap);
        float[] xs, ys, zs;
        PCGNode.PackNode.UnpackNormal(normalMap, out xs, out ys, out zs);

        float[] slopemap = PCGNode.PackNode.Unpack(slopeMap);
        for (int k = 0; k < 1000000; k++)
        {
            int i = Random.Range(0, width);
            int j = Random.Range(0, width);
            float height = heightmap[i + j * width];
            float slope = slopemap[j + i * width];

            // Debug.Log(xs[i + j * width] + " " + ys[i + j * width] + " " + zs[i + j * width]);
            if (height > 0.8) continue;
            if (slope < 0.2f) continue;
            // Debug.Log(slope);
            GameObject treePrefab = SamplePrefab(heightmap, zs[i + j * width], i, j, 0.1f, 0.3f);
            if (treePrefab != null)
            {
                Vector3 pos = new Vector3(i, height * TerrainHeight, j);
                Vector3 topPoint = pos + Vector3.up * 10;
                Vector3 bottomPoint = pos - Vector3.up * 10;
                // bool isOverlapping = Physics.CheckCapsule(topPoint, bottomPoint, 8);
                int layerMask = ~(1 << LayerMask.NameToLayer("Terrain"));

                bool isOverlapping = Physics.CheckCapsule(topPoint, bottomPoint, 5, layerMask);

                if (!isOverlapping)
                {
                    GameObject tree = GameObject.Instantiate(treePrefab, pos, Quaternion.identity);
                    tree.GetComponent<TreeController>().SetPos(i, j);
                    tree.transform.SetParent(ForestRoot);
                }

            }
        }

    }

    [ContextMenu("Clear Forest")]
    public void ClearForest()
    {
        var tempArray = new GameObject[ForestRoot.childCount];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = ForestRoot.GetChild(i).gameObject;
        }

        foreach (var child in tempArray)
        {
            DestroyImmediate(child);
        }
    }

    // float calc_slope(float x, float y, float z)
    // {

    // }


    public GameObject SamplePrefab(float[] heightmap, float slope, int i, int j, float slopeThreshold, float maxSlope)
    {
        // Assume your heightmap is a 2D grid stored in a 1D array.
        int width = (int)Mathf.Sqrt(heightmap.Length);
        if (i < 0 || i >= width || j < 0 || j >= width)
        {
            Debug.LogError("Index out of bounds.");
            return null;
        }

        // Debug.Log(slope);

        if (slope > maxSlope)
        {
            // Too steep for any vegetation.
            return null;
        }
        else if (slope < slopeThreshold)
        {
            // Gentle slope, suitable for trees.
            return TreePrefab;
        }
        else
        {
            // Steeper slope, suitable for bushes.
            return BushPrefab;
        }
    }



}
