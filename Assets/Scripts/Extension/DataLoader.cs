using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader
{
    public static T LoadJson<T>(string path)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(path);
        T temp = JsonUtility.FromJson<T>(jsonText.text);
        return temp;
    }
}