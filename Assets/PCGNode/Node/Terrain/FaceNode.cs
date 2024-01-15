using UnityEngine;

namespace PCGNode
{
    public static class FaceNode
    {
        // public static void CalcNormal(float[] heightmap, int width, out float[] xs, out float[] ys, out float[] zs, float scale = 1.0f)
        // {
        //     xs = new float[width * width];
        //     ys = new float[width * width];
        //     zs = new float[width * width];

        //     for (int i = 0; i < width; i++)
        //     {
        //         for (int j = 0; j < width; j++)
        //         {
        //             // Compute gradient in x direction
        //             float gx = (i < width - 1 ? heightmap[j * width + i + 1] : heightmap[j * width + i]) - heightmap[j * width + i];

        //             // Compute gradient in y direction
        //             float gy = (j < width - 1 ? heightmap[(j + 1) * width + i] : heightmap[j * width + i]) - heightmap[j * width + i];

        //             // Set z component to scale
        //             float gz = scale;

        //             // Calculate normal vector (nx, ny, nz)
        //             float nx = -gx;
        //             float ny = -gy;
        //             float nz = gz;

        //             // Normalize the normal vector
        //             float length = (float)Mathf.Sqrt(nx * nx + ny * ny + nz * nz);
        //             nx /= length;
        //             ny /= length;
        //             nz /= length;

        //             // Store the normalized normal in the arrays
        //             xs[j * width + i] = nx;
        //             ys[j * width + i] = ny;
        //             zs[j * width + i] = nz;
        //         }
        //     }
        // }

        public static void CalcNormal(float[] heightmap, int width, out float[] xs, out float[] ys, out float[] zs, float scale = 1.0f)
        {
            xs = new float[width * width];
            ys = new float[width * width];
            zs = new float[width * width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Calculate heights for neighboring points, handling boundaries
                    float hL = (i > 0) ? heightmap[j * width + (i - 1)] : heightmap[j * width + i];
                    float hR = (i < width - 1) ? heightmap[j * width + (i + 1)] : heightmap[j * width + i];
                    float hD = (j > 0) ? heightmap[(j - 1) * width + i] : heightmap[j * width + i];
                    float hU = (j < width - 1) ? heightmap[(j + 1) * width + i] : heightmap[j * width + i];

                    // Tangent vectors
                    Vector3 tanX = new Vector3(1, 0, (hR - hL) * scale);
                    Vector3 tanY = new Vector3(0, 1, (hU - hD) * scale);

                    // Cross product of tangent vectors gives the normal
                    Vector3 normal = Vector3.Cross(tanX, tanY);
                    normal.Normalize();

                    // Store the normalized normal components
                    xs[j * width + i] = normal.x;
                    ys[j * width + i] = normal.y;
                    zs[j * width + i] = normal.z;
                }
            }
        }





    }
}