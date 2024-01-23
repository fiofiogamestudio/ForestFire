using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FireSimulator : MonoBehaviour
{
    public static FireSimulator instance;
    public ComputeShader FireCompute;

    public RenderTexture FuelsRT;
    public RenderTexture FiresRT;

    public bool UseFuelMap = true;

    public int times = 10;

    public int width = 1024;

    [HideInInspector]
    public float[] fuels = new float[1024 * 1024];
    [HideInInspector]
    public float[] fires = new float[1024 * 1024];

    public Texture2D NormalMap;

    void Awake()
    {
        if (instance == null) instance = this;

    }
    void Start()
    {
        if (UseFuelMap)
        {
            // fuels = PCGNode.PackNode.Unpack(FuelMap);
        }
        else
        {
            for (int i = 0; i < 1024 * 1024; i++) fuels[i] = 1.0f;
        }

        SimulateFire();
    }

    public void FireAt(Vector2 pos)
    {
        if (pos.x > 0 && pos.x < 1024 && pos.y > 0 && pos.y < 1024)
        {
            // float px = pos.x / 1024;
            // float py = pos.y / 1024;
            // Debug.Log(px + " " + py);
            // int ix = width - 1 - (int)pos.x;
            // int iy = width - 1 - (int)pos.y;
            int ix = (int)pos.x;
            int iy = (int)pos.y;
            Debug.Log(ix + " " + iy);
            debugNormal(ix, iy);

            // fires[ix + iy * 1024] = 0.1f;
            firesBuffer.GetData(fires);

            // for (int i = 0; i < ix; i++)
            // {
            //     for (int j = 0; j < iy; j++)
            //     {
            //         fires[i + j * 1024] = 1.0f;
            //     }
            // }
            fires[ix + iy * width] = 1.0f;

            firesBuffer.SetData(fires);
        }

    }

    public void PudaAt(Vector2 pos1, float influence = 1.0f, int d = 50)
    {
        if (pos1.x > 0 && pos1.x < 1024 && pos1.y > 0 && pos1.y < 1024)
        {
            int ix = (int)pos1.x;
            int iy = (int)pos1.y;
            firesBuffer.GetData(fires);
            if (ix > d && ix < 1024 - d && iy > d && iy < 1024 - d)
            {
                for (int i = ix - d; i <= ix + d; i++)
                {
                    for (int j = iy - d; j <= iy + d; j++)
                    {
                        if (Vector2Int.Distance(new Vector2Int(ix, iy), new Vector2Int(i, j)) < d)
                        {
                            float value = fires[i + j * width];
                            value -= influence;
                            value = value < 0 ? 0 : value;
                            fires[i + j * width] = value;
                        }
                    }
                }
            }
            firesBuffer.SetData(fires);

        }
    }

    void Update()
    {

    }

    public void KanTree(int ix, int iy)
    {
        int d = 10;
        if (ix > d && ix < 1024 - d && iy > d && iy < 1024 - d)
        {
            for (int i = ix - d; i <= ix + d; i++)
            {
                for (int j = iy - d; j <= iy + d; j++)
                {
                    if (Vector2Int.Distance(new Vector2Int(ix, iy), new Vector2Int(i, j)) < d)
                    {
                        fuelsBuffer.GetData(fuels);
                        fuels[i + j * width] = 0.0f;
                        fuelsBuffer.SetData(fuels);
                    }
                }
            }
        }
    }

    int count = 0;
    void FixedUpdate()
    {
        count++;
        if (count > 100)
        {
            count = 0;
            RenderTexture myRenderTexture = FiresRT; // 获取或设置你的RenderTexture
            Texture2D myTexture2D = RenderTextureToTexture2D(myRenderTexture);
            // SaveTextureAsyc(myTexture2D, "FiresMap");
            // PCGNode.PackNode.SaveTexture2D(myTexture2D, "FiresMap");
        }
    }


    public Texture2D RenderTextureToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);

        // 将当前的RenderTexture设置为我们的rTex
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = rTex;

        // 从RenderTexture读取像素到Texture2D
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        // 恢复先前的活动RenderTexture
        RenderTexture.active = currentActiveRT;
        return tex;
    }


    public Color DebugNormalColor = Color.white;
    public Vector3 DebugNormalVector = Vector3.up;

    private void debugNormal(int ix, int iy)
    {
        // Color color = NormalMap.GetPixel(ix, iy);
        Color color = NormalMap.GetPixel(ix, iy);
        float x = color.r;
        float y = color.g;
        float z = color.b;
        x = x * 2 - 1;
        y = y * 2 - 1;
        z = z * 2 - 1;
        DebugNormalColor = color;
        DebugNormalVector = new Vector3(x, y, z);
        // Debug.Log(color);
    }



    public void SetWind(Vector2 wind)
    {
        shader.SetFloat("windX", wind.x);
        shader.SetFloat("windY", wind.y);


        Debug.Log("Set Wind " + wind);
    }

    public void SetSpeed(float speed)
    {
        shader.SetFloat("speed", speed);
    }


    [ContextMenu("Simulate Fire")]
    public void SimulateFire()
    {
        StartCoroutine(IESimulateFire(times));
    }

    private ComputeBuffer fuelsBuffer;
    private ComputeBuffer firesBuffer;

    public ComputeShader shader;

    public IEnumerator IESimulateFire(float times, float wait = 0.05f, int width = 1024)
    {
        shader = FireCompute;

        int kernelIndex = shader.FindKernel("SimulateFire");

        fuelsBuffer = new ComputeBuffer(fuels.Length, sizeof(float));
        firesBuffer = new ComputeBuffer(fires.Length, sizeof(float));

        fuelsBuffer.SetData(fuels);
        firesBuffer.SetData(fires);

        int[] dxs = { -1, 1, -1, 1, 0, 0, -1, 1 };
        int[] dys = { -1, -1, 1, 1, -1, 1, 0, 0 };

        ComputeBuffer dxsBuffer = new ComputeBuffer(dxs.Length, sizeof(int));
        ComputeBuffer dysBuffer = new ComputeBuffer(dys.Length, sizeof(int));

        dxsBuffer.SetData(dxs);
        dysBuffer.SetData(dys);

        shader.SetBuffer(kernelIndex, "fuelsBuffer", fuelsBuffer);
        shader.SetBuffer(kernelIndex, "firesBuffer", firesBuffer);
        shader.SetInt("width", width);
        shader.SetFloat("windX", 0);
        shader.SetFloat("windX", 0);
        shader.SetFloat("speed", 1);

        shader.SetBuffer(kernelIndex, "dxs", dxsBuffer);
        shader.SetBuffer(kernelIndex, "dys", dysBuffer);



        RenderTexture tempFuelsRT = new RenderTexture(width, width, 0, RenderTextureFormat.ARGBFloat);
        RenderTexture tempFiresRT = new RenderTexture(width, width, 0, RenderTextureFormat.ARGBFloat);
        tempFuelsRT.enableRandomWrite = true;
        tempFiresRT.enableRandomWrite = true;

        tempFuelsRT.Create();
        tempFiresRT.Create();

        shader.SetTexture(kernelIndex, "FuelsRT", tempFuelsRT);
        shader.SetTexture(kernelIndex, "FiresRT", tempFiresRT);

        RenderTexture tempNormalMap = new RenderTexture(width, width, 0, RenderTextureFormat.ARGBFloat);
        tempNormalMap.enableRandomWrite = true;
        tempNormalMap.Create();
        Graphics.Blit(NormalMap, tempNormalMap);
        shader.SetTexture(kernelIndex, "NormalMap", tempNormalMap);

        int threadGroupX = Mathf.CeilToInt(width / 8.0f);
        int threadGroupY = Mathf.CeilToInt(width / 8.0f);


        for (int k = 0; k < times; k++)
        {
            shader.Dispatch(kernelIndex, threadGroupX, threadGroupY, 1);

            // Debug.Log("iteration : " + k);

            // FireMaterial.SetTexture("_FuelsTex", FuelsRT);
            // FireMaterial.SetTexture("_FiresTex", FiresRT);
            Graphics.Blit(tempFuelsRT, FuelsRT);
            Graphics.Blit(tempFiresRT, FiresRT);

            yield return new WaitForSeconds(wait);


            // if (k % 10 == 0)
            // {
            //     Texture2D tempTex = new Texture2D(width, width, TextureFormat.RGB24, false);

            //     RenderTexture.active = FuelsRT;
            //     tempTex.ReadPixels(new Rect(0, 0, width, width), 0, 0);
            //     tempTex.Apply();

            //     PCGNode.PackNode.SaveTexture2D(tempTex, "FuelsMap");

            //     RenderTexture.active = FiresRT;
            //     tempTex.ReadPixels(new Rect(0, 0, width, width), 0, 0);
            //     tempTex.Apply();

            //     PCGNode.PackNode.SaveTexture2D(tempTex, "FiresMap");
            // }
        }


        // Don't forget to release the buffers when you're done with them!
        fuelsBuffer.Release();
        firesBuffer.Release();

        Debug.Log("Finish");
    }

    void OnDisable()
    {
        // Clear Fires Map
        int width = FiresRT.width;
        float[] reset = new float[width * width];
        PCGNode.PackNode.PackAndSave(reset, width, "FiresMap");
    }
}
