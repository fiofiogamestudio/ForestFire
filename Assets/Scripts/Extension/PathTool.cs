using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class PathTool
{
    [System.Serializable]
    public struct PathPoint
    {
        public int x;
        public int y;
        public float t;

        public PathPoint(int x, int y, float t)
        {
            this.x = x;
            this.y = y;
            this.t = t;
        }
    }

    [System.Serializable]
    public class PathNode
    {
        public PathPoint point;
        public PathNode parent = null;
        public List<PathNode> children = new List<PathNode>();

        public PathNode(PathPoint point)
        {
            this.point = point;
        }
    }

    public static List<Vector3> GeneratePath(Vector3[] pivots, int density = 10)
    {
        if (pivots == null || pivots.Length <= 1)
        {
            Debug.LogError("No Pivots Input");
            return null;
        }

        List<Vector3> pointList = new List<Vector3>();

        Vector3[] points = cutmull_rom(pivots);
        Vector3 prevPoint = interp(points, 0);
        int amount = (pivots.Length - 1) * density;
        for (int i = 0; i <= amount; i++)
        {
            float t = (float)i / amount;
            Vector3 point = interp(points, t);
            pointList.Add(point);
            prevPoint = point;
        }

        return pointList;
    }

    public static float GetPathLength(List<Vector3> points)
    {
        float len = 0;
        for (int i = 1; i < points.Count; i++)
        {
            len += Vector3.Distance(points[i], points[i - 1]);
        }
        return len;
    }

    public static void DrawPath(List<Vector3> points)
    {
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i], points[i - 1]);
        }
    }

    public static void DrawToArray(ref float[] map, List<Vector3> pointList, int width)
    {
        for (int i = 1; i < pointList.Count; i++)
        {
            // bresenham
            Vector2 p1 = pointList[i];
            Vector2 p2 = pointList[i - 1];
            drawline(ref map, width, p1, p2, 2);
        }
    }

    private static void drawline(ref float[] canvas, int width, Vector2 p1, Vector2 p2, int lineWidth)
    {
        Vector2 u = p2 - p1;
        float len = u.magnitude;
        Vector2 step = u / len;
        Vector2 perp = new Vector2(-step.y, step.y) * lineWidth / 2;
        for (float i = 0; i < len; i += 1)
        {
            Vector2 pos = p1 + step * i;
            for (float j = -lineWidth / 2; j < lineWidth / 2; j++)
            {
                pos = pos + perp * j;
                int x = (int)pos.x;
                int y = (int)pos.y;
                x = x < 0 ? 0 : (x >= width ? width - 1 : x);
                y = y < 0 ? 0 : (y >= width ? width - 1 : y);
                canvas[y * width + x] = 1;
            }
        }

    }

    private static Vector3[] cutmull_rom(Vector3[] pivots)
    {
        Vector3[] old_pivots = pivots;
        int old_len = old_pivots.Length;
        int len = old_len + 2;
        pivots = new Vector3[len];
        Array.Copy(old_pivots, 0, pivots, 1, old_len);

        // first and last control point
        pivots[0] = pivots[1] + (pivots[1] - pivots[2]);
        pivots[len - 1] = pivots[len - 2] + (pivots[len - 2] - pivots[len - 3]);

        // 
        if (pivots[1] == pivots[len - 2])
        {
            Vector3[] loopSpline = new Vector3[len];
            Array.Copy(pivots, loopSpline, len);
            loopSpline[0] = loopSpline[len - 3];
            loopSpline[len - 1] = loopSpline[2];
            pivots = new Vector3[len];
            Array.Copy(loopSpline, pivots, len);
        }

        return pivots;
    }

    private static Vector3 interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u) +
            (2f * a - 5f * b + 4f * c - d) * (u * u) +
            (-a + c) * u +
            2f * b
        );
    }

}
