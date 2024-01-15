using UnityEngine;

namespace PCGNode
{
    public static class FireNode
    {
        public static string FireComputePath = "Fire";

        public static void SimulateFire(float[] heights, int width, ref float[] fuels, ref float[] fires, int step = 1)
        {
            ComputeShader fireCompute = Resources.Load(FireComputePath) as ComputeShader;

            RenderTexture TempResult = new RenderTexture(width, width, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            TempResult.enableRandomWrite = true;
            TempResult.Create();


        }

    }
}