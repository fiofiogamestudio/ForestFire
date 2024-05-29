using System.Collections;
using System.Collections.Generic;
// using JetBrains.Annotations;
// using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

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

        AutoButton.onClick.AddListener(() =>
        {
            if (SelectedUI != null)
            {
                if (AutoButton.GetComponentInChildren<Text>().text == "关闭自动")
                {
                    SelectedUI.EnableAuto = false;
                    AutoButton.GetComponentInChildren<Text>().text = "开启自动";

                }
                else if (AutoButton.GetComponentInChildren<Text>().text == "开启自动")
                {
                    SelectedUI.EnableAuto = true;
                    AutoButton.GetComponentInChildren<Text>().text = "关闭自动";
                }
            }
        });

        FireButton.onClick.AddListener(() =>
        {
            if (SelectedUI != null && SelectedUI.BindedObject && SelectedUI.BindedObject.UnitIndex == 5)
            {
                SelectedUI.BindedObject.FireEffect();
            }
        });
    }

    public void Start()
    {
        LoadLevel();
    }

    public Button AutoButton;

    public Button FireButton;

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartFire();
        // }
    }

    public void StartFire()
    {
        for (int i = 0; i < FireObjectList.Count; i++)
        {
            if (FireObjectList[i] != null)
                FireObjectList[i].Fire();
        }
    }

    private void LoadLevel()
    {
        LevelInfo info = getInfo(GameConfig.MAP_TO_LOAD);
        if (info == null)
        {
            Debug.LogWarning("No level info");
            return;
        }
        int initX = info.initX;
        int initY = info.initY;

        GameObject initObject = GameObject.Instantiate(InitPrefab);
        initObject.transform.position = new Vector3(initX, WorldGenerator.instance.GetPosHeight(initX, initY), initY);

        foreach (var fire in info.fires)
        {
            GameObject fireObject = GameObject.Instantiate(FirePrefab);
            // Debug.Log(fire.fireX + " " + fire.fireY);
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

    [ReadOnly]
    public UnitUI SelectedUI;
    public void OnSelectUI(UnitUI ui)
    {
        this.SelectedUI = ui;

        if (SelectedUI.BindedObject.UnitIndex != 5)
        {
            this.AutoButton.gameObject.SetActive(true);
            this.FireButton.gameObject.SetActive(false);
            if (ui.EnableAuto)
            {
                this.AutoButton.GetComponentInChildren<Text>().text = "关闭自动";
            }
            else
            {
                this.AutoButton.GetComponentInChildren<Text>().text = "开启自动";
            }
        }
        else // 点火工具
        {
            this.FireButton.gameObject.SetActive(true);

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



