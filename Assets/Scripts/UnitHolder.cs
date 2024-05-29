using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHolder : MonoBehaviour
{
    [System.Serializable]
    public class UnitMapping
    {
        public GameObject unitUI;
        public GameObject unitObject;
    }

    public List<UnitMapping> UnitMappingList = new List<UnitMapping>();

    public static UnitHolder instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public GameObject GetUI(int index)
    {
        return UnitMappingList[index].unitUI;
    }

    public GameObject GetUnit(int index)
    {
        return UnitMappingList[index].unitObject;
    }
}
