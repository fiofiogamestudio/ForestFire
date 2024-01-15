using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateForest : MonoBehaviour
{
    public GameObject treePrefab;

    [ContextMenu("TestGenerate")]
    public void TestGenerate()
    {
        for (int i = 0; i < 300; i++)
        {
            for (int j = 0; j < 300; j++)
            {
                GameObject tree = GameObject.Instantiate(treePrefab, new Vector3(i, 0, j), Quaternion.identity);
                tree.transform.SetParent(this.transform);
                tree.transform.localScale = tree.transform.localScale * 0.1f;
            }
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        while (transform.childCount > 0)
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }




}
