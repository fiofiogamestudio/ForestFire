// using UnityEngine;
// using System.Collections.Generic;

// namespace PCGNode
// {
//     public class ErosionNode
//     {
//         public struct ErosionConfig
//         {
//             public int iterations;
//             public int brushRadius;
//             public int maxStep;
//             public float sedimentCapacityFactor;
//             public float minSedimentCapacity;
//             public float depositSpeed;
//             public float erodeSpeed;
//             public float evaporateSpeed;
//             public float gravity;
//             public float inertia;

//             public float startSpeed;
//             public float startWater;
//         };

//         public static class ErosionConfigBuilder
//         {
//             public static ErosionConfig Default
//             {
//                 get
//                 {
//                     return new ErosionConfig
//                     {
//                         iterations = 50000,
//                         brushRadius = 3,
//                         maxStep = 30,
//                         sedimentCapacityFactor = 3,
//                         minSedimentCapacity = 0.01f,
//                         depositSpeed = 0.3f,
//                         erodeSpeed = 0.3f,
//                         evaporateSpeed = 0.01f,
//                         gravity = 4,
//                         inertia = 0.3f,
//                         startSpeed = 1,
//                         startWater = 1
//                     };
//                 }
//             }
//         }

//         private struct GradientAndHeight
//         {
//             public float gradX;
//             public float gradY;
//             public float height;
//         }

//         struct ErosionBrush
//         {
//             public int mapSize;
//             public int radius;
//             public List<int> offsets;
//             public List<float> weights;
//         }

//         public static float[] Erode(float[] heightmap, int mapSize, ErosionConfig config)
//         {
//             // Prepare Brush
//             ErosionBrush brush = genBrush(mapSize, config.brushRadius);
//             // Prepare Indices (for droplets)
//             int[] randomIndices = TerrainTool.GetRandomIndices(mapSize - 1, config.iterations);

//             int brushLength = brush.offsets.Count;
//             List<int> brushOffsets = brush.offsets;
//             List<float> brushWeights = brush.weights;

//             float depositSpeed = config.depositSpeed;
//             float erodeSpeed = config.erodeSpeed;
//             float evaporateSpeed = config.evaporateSpeed;
//             float sedimentCapacityFactor = config.sedimentCapacityFactor;
//             float minSedimentCapacity = config.minSedimentCapacity;
//             float gravity = config.gravity;
//             float inertia = config.inertia;
//             // simulate
//             for (int it = 0; it < config.iterations; it++)
//             {
//                 int startIndex = randomIndices[it];
//                 float posX = (float)startIndex % mapSize;
//                 float posY = (float)startIndex / mapSize;
//                 float dirX = 0;
//                 float dirY = 0;
//                 float speed = config.startSpeed;
//                 float water = config.startWater;
//                 float sediment = 0;

//                 // step
//                 for (int step = 0; step < config.maxStep; step++)
//                 {
//                     // movement
//                     int pointX = (int)posX;
//                     int pointY = (int)posY;
//                     int currentIndex = pointY * mapSize + pointX;
//                     float offsetX = posX - pointX;
//                     float offsetY = posY - pointY;
//                     GradientAndHeight gradientAndHeight = computeGradientAndHeight(heightmap, mapSize, posX, posY, "old" + step.ToString());
//                     dirX = (dirX * inertia - gradientAndHeight.gradX * (1 - inertia));
//                     dirY = (dirY * inertia - gradientAndHeight.gradY * (1 - inertia));
//                     float len = Mathf.Max(0.01f, Mathf.Sqrt(dirX * dirX + dirY * dirY));
//                     dirX = dirX / len;
//                     dirY = dirY / len;
//                     posX += dirX;
//                     posY += dirY;

//                     // edge conditoin
//                     if (dirX == 0 || dirY == 0) break;
//                     if (posX < brush.radius || posX >= mapSize - brush.radius || posY < brush.radius || posY >= mapSize - brush.radius) break;

//                     // delta height
//                     float newHeight = computeGradientAndHeight(heightmap, mapSize, posX, posY, "new").height;
//                     float deltaHeight = newHeight - gradientAndHeight.height;

//                     // Debug.Log(deltaHeight);

//                     // deposit or erode
//                     float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);
//                     if (sediment > sedimentCapacity || deltaHeight > 0)
//                     {
//                         float amountToDeposit = deltaHeight > 0 ?
//                             Mathf.Min(deltaHeight, sediment) :
//                             (sediment - sedimentCapacity) * depositSpeed;
//                         sediment -= amountToDeposit;
//                         // apply
//                         heightmap[currentIndex] += amountToDeposit * (1 - offsetX) * (1 - offsetY);
//                         heightmap[currentIndex + 1] += amountToDeposit * offsetX * (1 - offsetY);
//                         heightmap[currentIndex + mapSize] += amountToDeposit * (1 - offsetX) * offsetY;
//                         heightmap[currentIndex + mapSize + 1] += amountToDeposit * offsetX * offsetY;
//                     }
//                     else
//                     {
//                         float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);
//                         for (int i = 0; i < brushLength; i++)
//                         {
//                             int erodeIndex = currentIndex + brushOffsets[i];
//                             float weightedAmountToErode = amountToErode * brushWeights[i];
//                             weightedAmountToErode = Mathf.Min(weightedAmountToErode, heightmap[erodeIndex]);
//                             heightmap[erodeIndex] -= weightedAmountToErode;
//                             sediment += weightedAmountToErode;
//                         }
//                     }

//                     // flow and evaporate
//                     speed = Mathf.Sqrt(Mathf.Max(0, speed * speed + deltaHeight * gravity));
//                     water *= (1 - evaporateSpeed);
//                 }

//             }

//             return heightmap;
//         }

//         private static GradientAndHeight computeGradientAndHeight(float[] heightmap, int mapSize, float posX, float posY, string a)
//         {
//             try
//             {

//                 int pointX = (int)posX;
//                 int pointY = (int)posY;
//                 int pointIndex = pointY * mapSize + pointX;
//                 float offsetX = posX - pointX;
//                 float offsetY = posY - pointY;
//                 // compute height
//                 float heightTopLeft = heightmap[pointIndex];
//                 float heightTopRight = heightmap[pointIndex + 1];
//                 float heightBottomLeft = heightmap[pointIndex + mapSize];
//                 float heightBottomRight = heightmap[pointIndex + mapSize + 1];

//                 // compute gradient
//                 float gradX = (heightTopRight - heightTopLeft) * (1 - offsetY) +
//                               (heightBottomRight - heightBottomLeft) * offsetY;
//                 float gradY = (heightTopLeft - heightBottomLeft) * (1 - offsetX) +
//                               (heightTopRight - heightBottomRight) * offsetX;
//                 float height = heightTopLeft * (1 - offsetX) * (1 - offsetY) +
//                               heightTopRight * offsetX * (1 - offsetY) +
//                               heightBottomLeft * (1 - offsetX) * offsetY +
//                               heightBottomRight * offsetX * offsetY;
//                 return new GradientAndHeight()
//                 {
//                     gradX = gradX,
//                     gradY = gradY,
//                     height = height
//                 };
//             }
//             catch (System.Exception e)
//             {

//                 Debug.Log(posX + " " + posY);
//                 return new GradientAndHeight();
//             }
//         }


//         private static ErosionBrush genBrush(int mapSize, int radius)
//         {
//             List<int> brushIndexOffsets = new List<int>();
//             List<float> brushWeights = new List<float>();

//             float weightSum = 0;
//             for (int y = -radius; y <= radius; y++)
//             {
//                 for (int x = -radius; x <= radius; x++)
//                 {
//                     float sqrtDist = x * x + y * y;
//                     if (sqrtDist < radius * radius)
//                     {
//                         brushIndexOffsets.Add(y * mapSize + x);
//                         float weight = 1 - Mathf.Sqrt(sqrtDist) / radius;
//                         weightSum += weight;
//                         brushWeights.Add(weight);
//                     }
//                 }
//             }

//             // normalize weight

//             for (int i = 0; i < brushWeights.Count; i++)
//             {
//                 brushWeights[i] /= weightSum;
//             }

//             return new ErosionBrush()
//             {
//                 mapSize = mapSize,
//                 radius = radius,
//                 offsets = brushIndexOffsets,
//                 weights = brushWeights
//             };
//         }
//         public static float[] ErodeGPU(float[] heightmap, int mapSize, ErosionConfig config)
//         {

//             // Prepare Brush
//             ErosionBrush brush = genBrush(mapSize, config.brushRadius);
//             // Prepare Indices (for droplets)
//             int[] randomIndices = TerrainTool.GetRandomIndices(mapSize, config.iterations);

//             // Create Resources
//             ComputeShader erosion = Resources.Load("Erosion") as ComputeShader;
//             ComputeBuffer mapBuffer = ComputeTool.GetBuffer(heightmap);
//             ComputeBuffer brushWeightsBuffer = ComputeTool.GetBuffer(brush.weights);
//             ComputeBuffer brushOffsetsBuffer = ComputeTool.GetBuffer(brush.offsets);
//             ComputeBuffer randomIndicesBuffer = ComputeTool.GetBuffer(randomIndices);

//             // Set Buffers
//             erosion.SetBuffer(0, "heightmap", mapBuffer);
//             erosion.SetBuffer(0, "brushOffsets", brushOffsetsBuffer);
//             erosion.SetBuffer(0, "brushWeights", brushWeightsBuffer);
//             erosion.SetBuffer(0, "randomIndices", randomIndicesBuffer);

//             // Set Variables
//             erosion.SetInt("mapSize", mapSize);
//             erosion.SetInt("borderSize", config.brushRadius);
//             erosion.SetInt("brushLength", brush.offsets.Count);
//             erosion.SetInt("maxStep", config.maxStep);
//             erosion.SetFloat("sedimentCapacityFactor", config.sedimentCapacityFactor);
//             erosion.SetFloat("minSedimentCapacity", config.minSedimentCapacity);
//             erosion.SetFloat("depositSpeed", config.depositSpeed);
//             erosion.SetFloat("erodeSpeed", config.erodeSpeed);
//             erosion.SetFloat("evaporateSpeed", config.evaporateSpeed);
//             erosion.SetFloat("gravity", config.gravity);
//             erosion.SetFloat("inertia", config.inertia);
//             erosion.SetFloat("startSpeed", config.startSpeed);
//             erosion.SetFloat("startWater", config.startWater);

//             // Run
//             erosion.Dispatch(0, config.iterations / 1024, 1, 1);
//             mapBuffer.GetData(heightmap);

//             // Release
//             mapBuffer.Release();
//             brushOffsetsBuffer.Release();
//             brushWeightsBuffer.Release();
//             randomIndicesBuffer.Release();

//             return heightmap;
//         }
//     }
// }

using UnityEngine;

namespace PCGNode
{
    public class ErosionNode
    {
        private static float init_speed = 1;
        private static float init_water = 1;
        private static int max_life = 30;
        private static float inertia = 0.05f;
        private static float sediment_capacity_factor = 4;
        private static float min_sediment_capacity = 0.01f;
        private static float deposit_speed = 0.3f, erode_speed = 0.4f, evaporate_speed = 0.01f;
        private static float gravity = 4;
        private static void calculate_height(
            float[] map,
            int width,
            float x,
            float y,
            out float height)
        {
            int i = (int)x,
                j = (int)y;
            int index = i + j * width;
            float u = x - i;
            float v = y - j;
            float h1 = map[index];
            float h2 = map[index + 1];
            float h3 = map[index + width];
            float h4 = map[index + width + 1];
            height = h1 * (1 - u) * (1 - v) + h2 * u * (1 - v) + h3 * (1 - u) * v + h4 * u * v;
        }
        private static void calculate_height_and_gradient(
            float[] map,
            int width,
            int index,
            float u,
            float v,
            out float height,
            out float grad_x,
            out float grad_y)
        {
            float h1 = map[index];
            float h2 = map[index + 1];
            float h3 = map[index + width];
            float h4 = map[index + width + 1];
            // gradient
            grad_x = (h2 - h1) * (1 - v) + (h4 - h3) * v;
            grad_y = (h3 - h1) * (1 - u) + (h4 - h2) * u;
            // height
            height = h1 * (1 - u) * (1 - v) + h2 * u * (1 - v) + h3 * (1 - u) * v + h4 * u * v;
        }
        public static float[] Erode(float[] heightmap, int width, int iterations)
        {
            while (iterations-- > 0)
            {
                float pos_x = Random.Range(0, width - 1);
                float pos_y = Random.Range(0, width - 1);
                float dir_x = 0, dir_y = 0;
                float speed = init_speed;
                float water = init_water;
                float sediment = 0;
                for (int life = 0; life < max_life; life++)
                {
                    int grid_x = (int)pos_x;
                    int grid_y = (int)pos_y;
                    int grid_index = grid_x + grid_y * width;
                    float offset_x = pos_x - grid_x;
                    float offset_y = pos_y - grid_y;
                    // Calculate Height and Gradient
                    float height, grad_x, grad_y;
                    calculate_height_and_gradient(heightmap, width, grid_index, offset_x, offset_y, out height, out grad_x, out grad_y);
                    // Update direction and position
                    dir_x = dir_x * inertia - grad_x * (1 - inertia);
                    dir_y = dir_y * inertia - grad_y * (1 - inertia);
                    float len = Mathf.Sqrt(dir_x * dir_x + dir_y * dir_y);
                    if (len != 0) { dir_x /= len; dir_y /= len; }
                    pos_x += dir_x;
                    pos_y += dir_y;
                    // Stop if not moving, or moving out of map
                    if (dir_x == 0 && dir_y == 0) break;
                    if (pos_x < 0 || pos_x >= width - 1 || pos_y < 0 || pos_y >= width - 1) break;
                    // Calculate Change
                    float new_height, delta_height;
                    calculate_height(heightmap, width, pos_x, pos_y, out new_height);
                    delta_height = new_height - height;
                    // Calculate Sediment : if enough -> deposit, if lack -> erode
                    float sediment_capacity = Mathf.Max(
                        -delta_height * speed * water * sediment_capacity_factor,
                        min_sediment_capacity
                    );
                    if (sediment > sediment_capacity || delta_height > 0)
                    {
                        float deposit = (delta_height > 0) ? Mathf.Min(delta_height, sediment) : (sediment - sediment_capacity) * deposit_speed;
                        heightmap[grid_index] += deposit * (1 - offset_x) * (1 - offset_y);
                        heightmap[grid_index + 1] += deposit * offset_x * (1 - offset_y);
                        heightmap[grid_index + width] += deposit * (1 - offset_x) * offset_y;
                        heightmap[grid_index + width + 1] += deposit * offset_x * offset_y;
                        sediment -= deposit;
                    }
                    else
                    {
                        float erode = Mathf.Min((sediment_capacity - sediment) * erode_speed, -delta_height);
                        heightmap[grid_index] -= erode * (1 - offset_x) * (1 - offset_y);
                        heightmap[grid_index + 1] -= erode * offset_x * (1 - offset_y);
                        heightmap[grid_index + width] -= erode * (1 - offset_x) * offset_y;
                        heightmap[grid_index + width + 1] -= erode * offset_x * offset_y;
                        sediment += erode;
                    }
                    // Gravity and Evaporation
                    speed = Mathf.Sqrt(speed * speed + delta_height * gravity);
                    water -= water * evaporate_speed;
                }
            }
            return heightmap;
        }
    }
}

