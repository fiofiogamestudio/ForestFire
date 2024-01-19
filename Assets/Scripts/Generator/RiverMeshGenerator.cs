using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class RiverMeshGenerator : MonoBehaviour
{
    public Texture2D RawHeightMap;
    public RiverParameterizer riverParameterizer;
    public GameObject EndRiverRoot;
    public GameObject NotEndRiverRoot;
    public GameObject RiverPrefab;

    public Vector3[] loadRiverPoints(List<PathTool.PathPoint> pointList)
    {
        return pointList.Select(p => new Vector3(p.x, p.y, 0)).ToArray();
    }

    public Vector3[] smoothPoints(Vector3[] points, int smoothCount)
    {
        if (points.Length <= 1) return null;
        int count = points.Length / smoothCount;
        if (count <= 0)
        {
            Vector3[] smoothedPoints = { points[0], points[points.Length - 1] };
            return smoothedPoints;
        }
        else
        {
            int other = points.Length % smoothCount;
            count = other > smoothCount / 2 ? count + 1 : count;
            // Debug.Log(points.Length + " " + count + " " + smoothCount);
            Vector3[] smoothedPoints = new Vector3[count + 1];
            for (int i = 0; i < count; i++)
            {
                smoothedPoints[i] = points[i * smoothCount];
            }
            smoothedPoints[count] = points[points.Length - 1];
            return smoothedPoints;
        }
    }

    void OnDrawGizmos()
    {
        // List<Vector3> pivots = new List<Vector3>();
        // pivots.Add(new Vector3(0, 0, 0));
        // pivots.Add(new Vector3(0, 1, 0));
        // pivots.Add(new Vector3(1, 1, 0));
        // pivots.Add(new Vector3(1, 0, 0));
        // PathTool.DrawPath(PathTool.GeneratePath(pivots.ToArray()));

        // Gizmos.DrawLine(Vector3.zero, Vector3.one);

    }

    [ContextMenu("Clear River Mesh")]
    void ClearRiverMesh()
    {
        while (EndRiverRoot.transform.childCount != 0) GameObject.DestroyImmediate(EndRiverRoot.transform.GetChild(0).gameObject);
        while (NotEndRiverRoot.transform.childCount != 0) GameObject.DestroyImmediate(NotEndRiverRoot.transform.GetChild(0).gameObject);
    }

    [ContextMenu("Generate River Mesh")]
    void GenerateRivermesh()
    {
        // List<Vector3> pivots = new List<Vector3>();
        // pivots.Add(new Vector3(0, 0, 0));
        // pivots.Add(new Vector3(0, 500, 0));
        // pivots.Add(new Vector3(500, 500, 0));
        // pivots.Add(new Vector3(500, 0, 0));

        // float[] map = new float[1024 * 1024];
        // List<Vector3> points = PathTool.GeneratePath(pivots.ToArray(), 20);
        // PathTool.DrawToArray(ref map, points);
        // PCGNode.PackNode.SaveTexture2D(PCGNode.PackNode.Pack(map, 1024), "test curve");

        // CalcRiverMesh(RiverObjcet, ref points, 10);

        // prepare map
        int width = GameConfig.MAP_WIDTH;
        float[] parameterizedMap = new float[width * width];

        // clear river mesh 
        ClearRiverMesh();

        // get river path list
        List<List<PathTool.PathPoint>> riverEndPathList;
        List<List<PathTool.PathPoint>> riverNotEndPathList;
        riverParameterizer.GetPathList(out riverEndPathList, out riverNotEndPathList);
        // Debug.Log(riverEndPathList.Count);
        // Debug.Log(riverNotEndPathList.Count);
        List<Vector3> TestList = new List<Vector3>();
        TestList.Add(new Vector3(100, 100, 0));
        TestList.Add(new Vector3(200, 200, 0));

        foreach (var riverPath in riverEndPathList)
        {
            Vector3[] points = loadRiverPoints(riverPath);
            if (points.Length <= 2) continue; // 去毛 （特别小的转角）
            GameObject riverObject = GameObject.Instantiate(RiverPrefab);
            riverObject.transform.parent = EndRiverRoot.transform;
            points = smoothPoints(points, 20);
            List<Vector3> pointList = PathTool.GeneratePath(points, 5);
            // PathTool.DrawToArray(ref parameterizedMap, points.ToList<Vector3>());
            PathTool.DrawToArray(ref parameterizedMap, pointList, width);
            riverObject.GetComponent<RiverMeshController>().SetRawPoints(points);
            riverObject.GetComponent<RiverMeshController>().LerpedPointList = pointList.ToArray();
            // riverObject.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
            CalcRiverMesh(riverObject, pointList, 10, true);
            // PathTool.DrawToArray(ref parameterizedMap, pointList, width);
        }

        foreach (var riverPath in riverNotEndPathList)
        {
            Vector3[] points = loadRiverPoints(riverPath);
            if (points.Length <= 2) continue;
            GameObject riverObject = GameObject.Instantiate(RiverPrefab);
            riverObject.transform.parent = NotEndRiverRoot.transform;
            points = smoothPoints(points, 20);
            List<Vector3> pointList = PathTool.GeneratePath(points, 5);
            PathTool.DrawToArray(ref parameterizedMap, pointList, width);
            riverObject.GetComponent<RiverMeshController>().SetRawPoints(points);
            riverObject.GetComponent<RiverMeshController>().LerpedPointList = pointList.ToArray();
            // riverObject.GetComponent<MeshRenderer>().material.color = Color.blue;
            CalcRiverMesh(riverObject, pointList, 10, false);
            // PathTool.DrawToArray(ref parameterizedMap, pointList, width);
        }

        PCGNode.PackNode.SaveTexture2D(PCGNode.PackNode.Pack(parameterizedMap, width), GameConfig.VISUALED_PATHMAP);

        // foreach (var riverPath in riverNotEndPathList)
        // {
        //     Vector3[] points = loadRiverPoints(riverPath);
        //     points = smoothPoints(points, 20);
        //     GameObject riverObject = GameObject.Instantiate(RiverPrefab);
        //     riverObject.transform.parent = NotEndRiverRoot.transform;
        //     CalcRiverMesh(riverObject, PathTool.GeneratePath(points, 5), 10, false);
        // }
    }


    public void CalcRiverMesh(GameObject RiverObject, List<Vector3> points, float width, bool useDynamicWidth = false)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 current = points[i];
            float height = RawHeightMap.GetPixel((int)current.x, (int)current.y).r * 210;
            points[i] = new Vector3(current.x, height, current.y);
        }


        RiverObject.transform.position = Vector3.zero;
        MeshRenderer riverRenderer = RiverObject.GetComponent<MeshRenderer>();
        MeshFilter riverFilter = RiverObject.GetComponent<MeshFilter>();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 forward = (points[i + 1] - points[i]).normalized;
            Vector3 right = new Vector3(-forward.z, 0, forward.x); // This assumes a flat terrain (y = 0)

            float t = (float)i / points.Count;
            float dynamicWidth = width;
            if (useDynamicWidth) dynamicWidth *= (1 - t * t * t);

            Vector3 leftVertex = points[i] - right * dynamicWidth / 2;
            Vector3 rightVertex = points[i] + right * dynamicWidth / 2;

            vertices.Add(leftVertex);
            vertices.Add(rightVertex);

            // Create two triangles for the quad
            if (i < points.Count - 2)
            {
                triangles.Add(i * 2);
                triangles.Add(i * 2 + 1);
                triangles.Add(i * 2 + 2);

                triangles.Add(i * 2 + 2);
                triangles.Add(i * 2 + 1);
                triangles.Add(i * 2 + 3);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        riverFilter.mesh = mesh;

        RiverObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
