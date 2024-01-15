using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateTerrain : MonoBehaviour
{
    public Texture2D HeightMap;
    public float HScale = 1.0f;
    [Range(1, 1024)]
    public int SegX = 64;
    [Range(1, 1024)]
    public int SegY = 64;
    public float MeshWidth = 20.0f;

    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        // Calculate the number of vertices, triangles and uv
        mesh.Clear();
        Vector3[] vertices = new Vector3[(SegX + 1) * (SegY + 1)];
        int[] triangles = new int[SegX * SegY * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        float size = MeshWidth;
        float sizeReciprocal = 1f / size;

        // Generate vertices and uv
        int i = 0;
        for (int y = 0; y <= SegY; y++)
        {
            for (int x = 0; x <= SegX; x++)
            {
                float height = HeightMap.GetPixel((int)((float)x / SegX * HeightMap.width), (int)((float)y / SegY * HeightMap.height)).grayscale;
                // float height = 0;
                vertices[i] = new Vector3(x * size, height * HScale, y * size);
                uv[i] = new Vector2(x * sizeReciprocal, y * sizeReciprocal);
                i++;
            }
        }

        // Generate triangles
        for (int ti = 0, vi = 0, y = 0; y < SegY; y++, vi++)
        {
            for (int x = 0; x < SegX; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + SegX + 1;
                triangles[ti + 5] = vi + SegX + 2;
            }
        }

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
