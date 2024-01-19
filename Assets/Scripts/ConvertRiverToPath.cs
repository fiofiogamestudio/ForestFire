using System.Collections.Generic;
using UnityEngine;

public class ConvertRiverToPath : MonoBehaviour
{
    private const int SMOOTH = 50;
    public Texture2D RiverMap;

    public List<List<Vector3>> pathSegList = new List<List<Vector3>>();

    public Texture2D HeightMap;

    public GameObject riverPrefab;

    [ContextMenu("Convert")]
    public void Convert()
    {

        // Clear
        while (transform.childCount != 0) GameObject.DestroyImmediate(transform.GetChild(0).gameObject);

        float[] result = new float[GameConfig.MAP_WIDTH * GameConfig.MAP_WIDTH];
        foreach (var seg in pathSegList)
        {
            List<Vector3> path = PathTool.GeneratePath(seg.ToArray());
            foreach (var point in path) Debug.Log(point);
            PathTool.DrawToArray(ref result, path, GameConfig.MAP_WIDTH);
            // PathTool.DrawToArray(ref result, seg);
            GameObject riverObject = GameObject.Instantiate(riverPrefab);
            riverObject.transform.parent = this.transform;
            // CalcRiverMesh(riverObject, PathTool.GeneratePath(seg.ToArray()), 20);
        }


        PCGNode.PackNode.SaveTexture2D(PCGNode.PackNode.Pack(result, GameConfig.MAP_WIDTH), "test curve new");

    }





    private List<Vector3> findEntry(float[] map, int width, float threshold)
    {
        List<Vector3> result = new List<Vector3>();
        if (map[0] > threshold) result.Add(new Vector3 { x = 0, y = 0, z = 0 });
        if (map[width - 1] > threshold) result.Add(new Vector3 { x = width - 1, y = 0, z = 0 });
        if (map[width * (width - 1)] > threshold) result.Add(new Vector3 { x = 0, y = width - 1, z = 0 });
        if (map[width * width - 1] > threshold) result.Add(new Vector3 { x = width - 1, y = width - 1, z = 0 });
        for (int i = 1; i < width - 1; i++)
        {
            if (map[i] > threshold) result.Add(new Vector3 { x = i, y = 0, z = 0 });
            if (map[width * (width - 1) + i] > threshold) result.Add(new Vector3 { x = i, y = width - 1, z = 0 });
            if (map[width * i] > threshold) result.Add(new Vector3 { x = 0, y = i, z = 0 });
            if (map[width * i + (width - 1)] > threshold) result.Add(new Vector3 { x = width - 1, y = i, z = 0 });
        }
        return result;
    }

    private void findPath(float[] map, Vector3 source, float threshold, ref List<List<Vector3>> pathSegList, ref bool[] visited)
    {
        List<Vector3> nodes = new List<Vector3>();
        nodes.Add(source);
        visited[(int)source.x + (int)source.y * GameConfig.MAP_WIDTH] = true;
        Vector3 current = source;
        int step = 0;
        Debug.Log("start find path from " + current);
        List<Vector3> adjs = findAdj(current, ref map, threshold, ref visited, GameConfig.MAP_WIDTH);
        // Debug.Log("adjs count " + adjs.Count);
        while (adjs.Count != 0)
        {
            step++;
            Vector3 next = adjs[0];
            visited[(int)next.x + (int)next.y * GameConfig.MAP_WIDTH] = true;
            if (adjs.Count == 1 && step > SMOOTH)
            {
                step = 0;
                nodes.Add(next);
                // Debug.Log("add node " + next);
            }
            else if (adjs.Count != 1)
            {
                // step = 0;
                // nodes.Add(next);
                // for (int i = 1; i < adjs.Count; i++)
                {
                    // findPath(map, next, threshold, ref pathSegList, ref visited);
                }
            }

            adjs = findAdj(next, ref map, threshold, ref visited, GameConfig.MAP_WIDTH);
        }

        if (nodes.Count > 1)
        {
            pathSegList.Add(nodes);
        }
    }

    private List<Vector3> findAdj(Vector3 center, ref float[] map, float threshold, ref bool[] visited, int width)
    {
        List<Vector3> nodes = new List<Vector3>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int x = (int)center.x + i;
                int y = (int)center.y + j;
                if (x >= 0 && x < width && y >= 0 && y < width)
                {
                    float z = map[x + y * width];
                    if (!visited[x + y * width] && z > threshold)
                    {
                        nodes.Add(new Vector3 { x = x, y = y, z = z });
                    }
                }
            }
        }


        return nodes;
    }
}
