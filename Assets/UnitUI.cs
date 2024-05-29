using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    [ReadOnly]
    public bool isSelected = false;

    [ReadOnly]
    public UnitController BindedObject;



    private Outline outline;
    private Button button;

    [ReadOnly]
    public int WaterAmount;
    public int MAXWaterAmount;

    public Image WaterFill;

    [ReadOnly]
    public bool EnableAuto = true;


    void Awake()
    {
        this.outline = GetComponent<Outline>();
        this.button = GetComponentInChildren<Button>();
        this.button.onClick.AddListener(() =>
        {
            if (!isSelected)
            {
                UnitManager.instance.UnSelectAll();
                Select();
                UnitManager.instance.OnSelectUI(this);
            }
        });


        // init 
        Unselect();
    }

    void Start()
    {
        WaterAmount = MAXWaterAmount;
    }

    void FixedUpdate()
    {
        if (MAXWaterAmount != 0)
        {
            WaterFill.fillAmount = (float)WaterAmount / MAXWaterAmount;
        }
    }


    public void Unselect()
    {
        this.isSelected = false;
        this.outline.effectColor = Color.black;

        if (BindedObject != null)
        {
            BindedObject.UnSelectThis();
        }
    }

    public void Select()
    {
        this.isSelected = true;
        this.outline.effectColor = Color.white;

        if (BindedObject != null)
        {
            BindedObject.SelectThis();
            Debug.Log("Show");
        }
    }



    public void BindObject(UnitController unitController)
    {
        this.BindedObject = unitController;
        this.BindedObject.BindedUI = this;
    }

    public bool UseWater(int count = 1)
    {
        if (WaterAmount > 0)
        {
            WaterAmount -= count;
            WaterAmount = WaterAmount < 0 ? 0 : WaterAmount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FillWater(int count = 1)
    {
        if (WaterAmount < MAXWaterAmount)
        {
            WaterAmount += count;
            WaterAmount = WaterAmount > MAXWaterAmount ? MAXWaterAmount : WaterAmount;
        }
    }
}
