using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverMeshController : MonoBehaviour
{
    public Vector3[] RawPoints;
    public Vector3[] LerpedPointList;

    void OnDrawGizmos()
    {
        // PathTool.DrawPath(PathTool.GeneratePath(RawPoints));
    }

    public void SetRawPoints(Vector3[] rawPoints)
    {
        int n = rawPoints.Length;
        RawPoints = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            Vector3 p = rawPoints[i];
            // RawPoints[i] = p;
            RawPoints[i] = new Vector3(p.x, 0, p.y);
        }
    }
}
