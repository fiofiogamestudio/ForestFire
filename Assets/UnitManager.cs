using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public Transform UnitGroup;
    public Transform UnitRoot;

    public GameObject InitPrefab;

    public GameObject FirePrefab;

    [ReadOnly]
    public List<UnitUI> UnitUIList = new List<UnitUI>();
    [ReadOnly]
    public List<FireObject> FireObjectList = new List<FireObject>();


    public static UnitManager instance;
    public void Awake()
    {
        if (instance == null) instance = this;
    }

    public void Start()
    {
        LoadLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartFire();
        }
    }

    public void StartFire()
    {
        for (int i = 0; i < FireObjectList.Count; i++)
        {
            FireObjectList[i].Fire();
        }
    }

    private void LoadLevel()
    {
        LevelInfo info = getInfo(GameConfig.MAP_TO_LOAD);
        int initX = info.initX;
        int initY = info.initY;

        GameObject initObject = GameObject.Instantiate(InitPrefab);
        initObject.transform.position = new Vector3(initX, WorldGenerator.instance.GetPosHeight(initX, initY), initY);

        foreach (var fire in info.fires)
        {
            GameObject fireObject = GameObject.Instantiate(FirePrefab);
            Debug.Log(fire.fireX + " " + fire.fireY);
            fireObject.transform.position = new Vector3(fire.fireX, WorldGenerator.instance.GetPosHeight(initX, initY) + 80, fire.fireY);

            FireObjectList.Add(fireObject.GetComponent<FireObject>());
        }

        // generate unit UI
        foreach (var unitIndex in info.units)
        {
            GameObject unitUI = GameObject.Instantiate(UnitHolder.instance.GetUI(unitIndex));
            unitUI.transform.SetParent(UnitGroup);

            GameObject unitObject = GameObject.Instantiate(UnitHolder.instance.GetUnit(unitIndex));
            unitObject.transform.SetParent(UnitRoot);
            unitObject.transform.position = new Vector3(initX + Random.Range(-10, 10), WorldGenerator.instance.GetPosHeight(initX, initY), initY + Random.Range(-10, 10));

            UnitUI unitUI1Script = unitUI.GetComponent<UnitUI>();
            UnitUIList.Add(unitUI1Script);

            UnitController unitScript = unitObject.GetComponent<UnitController>();
            unitScript.InitData(unitIndex);

            unitUI1Script.BindObject(unitScript);
        }
    }

    private LevelInfo getInfo(string mapName)
    {
        LevelJson jsonObject = DataLoader.LoadJson<LevelJson>("Map/Level");
        foreach (var info in jsonObject.levels)
        {
            if (info.mapName == mapName) return info;
        }
        return null;
    }

    public void UnSelectAll()
    {
        foreach (var unitUI in UnitUIList)
        {
            unitUI.Unselect();
        }
    }



}

[System.Serializable]
public class FirePos
{
    public int fireX;
    public int fireY;
}

[System.Serializable]
public class LevelInfo
{
    public string mapName;
    public int[] units;
    public int initX;
    public int initY;
    public FirePos[] fires;
}

[System.Serializable]
public class LevelJson
{
    public LevelInfo[] levels;
}



