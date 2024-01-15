using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Collections;

namespace PCGNode
{
    public class PackNode
    {
        public static float[] Unpack(Texture2D input)
        {
            int width = input.width;
            float[] result = new float[width * width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i + j * width] = input.GetPixel(i, j).r;
                }
            }
            return result;
        }

        public static void UnpackNormal(Texture2D input, out float[] xs, out float[] ys, out float[] zs)
        {
            int width = input.width;
            xs = new float[width * width];
            ys = new float[width * width];
            zs = new float[width * width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color color = input.GetPixel(i, j);
                    xs[i * width + j] = color.r * 2.0f - 1;
                    ys[i * width + j] = color.g * 2.0f - 1;
                    zs[i * width + j] = color.b * 2.0f - 1;
                }
            }
        }

        public static Texture2D Pack(float[] values, int width)
        {
            Texture2D output = new Texture2D(width, width, TextureFormat.RGBAFloat, false);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float value = values[i + j * width];
                    output.SetPixel(i, j, new Color(value, value, value));
                }
            }
            output.Apply();
            return output;
        }

        public static Texture2D PackRGB(float[] rvalues, float[] gvalues, float[] bvalues, int width)
        {
            Texture2D output = new Texture2D(width, width, TextureFormat.RGBAFloat, false);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float rvalue = rvalues[i + j * width];
                    float gvalue = gvalues[i + j * width];
                    float bvalue = bvalues[i + j * width];
                    output.SetPixel(i, j, new Color(rvalue, gvalue, bvalue));
                }
            }
            output.Apply();
            return output;
        }

        public static Texture2D PackNormal(float[] xvalues, float[] yvalues, float[] zvalues, int width)
        {
            Texture2D output = new Texture2D(width, width, TextureFormat.RGBAFloat, false);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float rvalue = (xvalues[i + j * width] + 1) / 2.0f;
                    float gvalue = (yvalues[i + j * width] + 1) / 2.0f;
                    float bvalue = (zvalues[i + j * width] + 1) / 2.0f;
                    output.SetPixel(i, j, new Color(rvalue, gvalue, bvalue));
                }
            }
            output.Apply();
            return output;
        }


        public static Texture2D PackMask(int[] values, int width)
        {
            Texture2D output = new Texture2D(width, width);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    float value = values[i + j * width];
                    output.SetPixel(i, j, new Color(value, value, value));
                }
            }
            output.Apply();
            return output;
        }

        public static void SaveTexture2D(Texture2D toSave, string path, bool silent = true)
        {
            byte[] data = toSave.EncodeToPNG();
            string savePath = Application.dataPath + "/" + path + ".png";
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
            if (!silent) Debug.Log("Save File : " + path + ".png");
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public static void SaveTexture2DAsync(Texture2D toSave, string path, bool silent = true)
        {
            CoroutineHelper.Start(EncodeAndSaveCoroutine(toSave, path, silent));
        }

        private static IEnumerator EncodeAndSaveCoroutine(Texture2D toSave, string path, bool silent)
        {
            byte[] data = null;

            // 在主线程上编码纹理为PNG
            data = toSave.EncodeToPNG();

            // 将文件保存操作移至后台线程
            System.Threading.Tasks.Task.Run(() => SaveToFile(data, path, silent));

#if UNITY_EDITOR
            // 必须在主线程上运行
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
            yield return null;
        }

        private static void SaveToFile(byte[] data, string path, bool silent)
        {
            FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
            if (!silent) Debug.Log("Save File : " + path + ".png");
        }

        public static void PackAndSave(float[] values, int width, string path)
        {
            Texture2D toSave = Pack(values, width);
            SaveTexture2D(toSave, path);
        }

        public static void PackAndSaveRGB(float[] rvalues, float[] gvalues, float[] bvalues, int width, string path)
        {
            Texture2D toSave = PackRGB(rvalues, gvalues, bvalues, width);
            SaveTexture2D(toSave, path);
        }

        public static void PackAndSaveNormal(float[] xvalues, float[] yvalues, float[] zvalues, int width, string path)
        {
            Texture2D toSave = PackNormal(xvalues, yvalues, zvalues, width);
            SaveTexture2D(toSave, path);
        }
    }
}
// CoroutineHelper类，允许从非MonoBehaviour类启动协程
public static class CoroutineHelper
{
    private static readonly EmptyMonoBehaviour helper;

    static CoroutineHelper()
    {
        helper = new GameObject("CoroutineHelper").AddComponent<EmptyMonoBehaviour>();
        GameObject.DontDestroyOnLoad(helper.gameObject);
    }

    public static Coroutine Start(IEnumerator coroutine)
    {
        return helper.StartCoroutine(coroutine);
    }

    private class EmptyMonoBehaviour : MonoBehaviour { }
}