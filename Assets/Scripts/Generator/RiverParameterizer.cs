using System.Collections;
using System.Collections.Generic;
using PCGNode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class RiverParameterizer : MonoBehaviour
{
    public Texture2D ExtractedRiverMap;

    public bool VisualizeRiver = true;
    List<List<PathTool.PathPoint>> ParameterizedRiverList = new List<List<PathTool.PathPoint>>();
    List<List<PathTool.PathNode>> RiverGroupList = new List<List<PathTool.PathNode>>();

    public List<List<PathTool.PathPoint>> EndRiverPathList = new List<List<PathTool.PathPoint>>();
    public List<List<PathTool.PathPoint>> NotEndRiverPathList = new List<List<PathTool.PathPoint>>();

    [ContextMenu("Parameterize River")]
    public void ParameterizeRiver()
    {
        string VisualizedRiverPath = GameConfig.VISUALIZED_RIVERMAP_PATH;

        ParameterizedRiverList.Clear();
        RiverGroupList.Clear();

        // load rivermap
        int width = ExtractedRiverMap.width;
        float[] rivermap = PCGNode.PackNode.Unpack(ExtractedRiverMap);

        // find start point list
        List<PathTool.PathPoint> startPointList = findStartPointList(ref rivermap, width);

        // find path group
        bool[] visited = new bool[width * width];
        foreach (var startPoint in startPointList)
        {
            findRiverGroup(ref rivermap, ref visited, startPoint, width);
        }


        // find path list
        // int index = 0;
        foreach (var riverGroup in RiverGroupList)
        {
            // index++;
            // if (index != 2) continue;
            traceRiverPath(riverGroup[0]);
        }

        // visualize river path
        if (VisualizeRiver)
        {
            float[] map = new float[width * width];
            foreach (var path in EndRiverPathList)
            {
                foreach (var point in path)
                {
                    map[point.x + point.y * width] += 0.5f;
                }
            }
            foreach (var path in NotEndRiverPathList)
            {
                foreach (var point in path)
                {
                    map[point.x + point.y * width] += 0.5f;
                }
            }
            PCGNode.PackNode.PackAndSave(map, width, VisualizedRiverPath);
        }
    }

    List<PathTool.PathPoint> findStartPointList(ref float[] rivermap, int width)
    {
        List<PathTool.PathPoint> startPointList = new List<PathTool.PathPoint>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i == 0 || j == 0 || i == width - 1 || j == width - 1)
                {
                    if (rivermap[i + j * width] > 0) startPointList.Add(new PathTool.PathPoint(i, j, 0));
                }
            }
        }
        return startPointList;
    }

    void findRiverGroup(ref float[] map, ref bool[] visited, PathTool.PathPoint startPoint, int width)
    {
        int i = startPoint.x;
        int j = startPoint.y;
        if (visited[i + j * width]) return;
        PathTool.PathNode currentNode = new PathTool.PathNode(startPoint);
        Queue<PathTool.PathNode> nodeQueue = new Queue<PathTool.PathNode>();
        nodeQueue.Enqueue(currentNode);
        List<PathTool.PathNode> currentGroup = new List<PathTool.PathNode>();
        currentGroup.Add(currentNode);
        visited[i + j * width] = true;
        // Debug.Log("startPoint " + startPoint.x + " " + startPoint.y);
        int check = 0;
        while (nodeQueue.Count != 0)
        {
            currentNode = nodeQueue.Dequeue();
            List<PathTool.PathPoint> adjacentPointList = getAdjacentPointList(ref map, ref visited, currentNode.point, width);
            foreach (var point in adjacentPointList)
            {
                // Debug.Log(point.x + " " + point.y + " -> " + currentNode.point.x + " " + currentNode.point.y);
                PathTool.PathNode adjacentNode = new PathTool.PathNode(point);
                adjacentNode.parent = currentNode;
                currentNode.children.Add(adjacentNode);
                nodeQueue.Enqueue(adjacentNode);
                currentGroup.Add(adjacentNode);
                visited[adjacentNode.point.x + adjacentNode.point.y * width] = true;
            }
            check++;
            if (check > 100000) break;
        }
        RiverGroupList.Add(currentGroup);
        // Debug.Log(check);
    }

    List<PathTool.PathPoint> getAdjacentPointList(ref float[] map, ref bool[] visited, PathTool.PathPoint currentPoint, int width)
    {
        int i = currentPoint.x;
        int j = currentPoint.y;
        List<PathTool.PathPoint> adjacentPointList = new List<PathTool.PathPoint>();
        for (int m = -1; m <= 1; m++)
        {
            for (int n = -1; n <= 1; n++)
            {
                if ((m != 0 || n != 0) &&
                (i + m >= 0 && i + m <= width - 1) &&
                (j + n >= 0 && j + n <= width - 1))
                {
                    int index = (i + m) + (j + n) * width;
                    if (!visited[index] && map[index] > 0)
                    {
                        // Debug.Log(m + " " + n + " " + (i + m) + " " + (j + n));
                        adjacentPointList.Add(new PathTool.PathPoint(i + m, j + n, 0));
                    }
                }
            }
        }
        return adjacentPointList;
    }

    void traceRiverPath(PathTool.PathNode startNode, PathTool.PathNode prevNode = null, int prevCount = 10)
    {
        List<PathTool.PathPoint> pathPointList = new List<PathTool.PathPoint>();
        while (prevCount > 0 && prevNode != null)
        {
            pathPointList.Insert(0, prevNode.point);
            prevNode = prevNode.parent;
            prevCount--;
        }

        PathTool.PathNode currentNode = startNode;
        while (currentNode.children.Count == 1)
        {
            // Debug.Log(currentNode.point.x + " " + currentNode.point.y);
            pathPointList.Add(currentNode.point);
            currentNode = currentNode.children[0];
        }

        // Debug.Log("Path Length " + pathPointList.Count);
        if (currentNode.children.Count < 1)
        {
            pathPointList.Add(currentNode.point);
            EndRiverPathList.Add(pathPointList);
        }
        else if (currentNode.children.Count > 1)
        {
            pathPointList.Add(currentNode.point);
            NotEndRiverPathList.Add(pathPointList);
            foreach (var child in currentNode.children)
            {
                traceRiverPath(child, currentNode);
            }
        }
    }

    public void GetPathList(out List<List<PathTool.PathPoint>> endPathList, out List<List<PathTool.PathPoint>> notEndPathList, int minLength = 2)
    {
        ParameterizeRiver();
        endPathList = new List<List<PathTool.PathPoint>>();
        notEndPathList = new List<List<PathTool.PathPoint>>();
        foreach (var path in EndRiverPathList)
        {
            if (path.Count >= minLength) endPathList.Add(path);
        }
        foreach (var path in NotEndRiverPathList)
        {
            if (path.Count >= minLength) notEndPathList.Add(path);
        }
    }


}
