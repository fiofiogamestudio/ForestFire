using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public void DrawLine(Vector3 start, Vector3 end)
    {
        // Set the number of points to 2
        lineRenderer.positionCount = 2;

        // Set the positions
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Set it as a dashed line
        MakeDashed();
    }

    private void MakeDashed()
    {
        // Assuming you have a material and texture for the LineRenderer
        // The exact value for the tiling will depend on your texture and the desired look for the dashed line
        float tiling = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)) * 2; // This might need adjustment
        lineRenderer.material.mainTextureScale = new Vector2(tiling, 1);
    }

    [ContextMenu("TestDraw")]
    public void TestDraw()
    {
        DrawLine(new Vector3(200, 200, 300), new Vector3(512, 200, 512));
    }
}