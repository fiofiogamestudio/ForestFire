using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// using UnityEngine.Rendering.HighDefinition;

public class VFXManager : MonoBehaviour
{
    public GameObject PudaVFX;
    public GameObject FengliVFX;

    public GameObject ShuiVFX;
    public static VFXManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }


    public void ShowVFX(VFXType vfxType, Vector3 pos, Vector2 dir)
    {
        Debug.Log(vfxType.ToString());
        GameObject vfxPrefab = null;
        switch (vfxType)
        {
            case VFXType.Puda:
                vfxPrefab = PudaVFX;
                break;
            case VFXType.Fengli:
                vfxPrefab = FengliVFX;
                break;
            case VFXType.Shui:
                // vfxPrefab = ShuiVFX;
                break;
        }

        if (vfxPrefab != null)
        {
            GameObject go = GameObject.Instantiate(vfxPrefab);
            go.transform.position = pos;
            go.transform.LookAt(transform.position + new Vector3(dir.x, 0, dir.y) * 10000);
        }
    }
}

public enum VFXType
{
    Puda,
    Fengli,
    Shui
};
