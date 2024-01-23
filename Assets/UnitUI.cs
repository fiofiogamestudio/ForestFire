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
            }
        });


        // init 
        Unselect();
    }

    void Start()
    {

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
    }
}
