using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [ReadOnly]
    public int UnitIndex = 0;
    [ReadOnly]
    public bool isSelected = false;
    [ReadOnly]
    public Vector3 TargetPos;

    public GameObject TipIcon;

    private float speed = 1;

    private float pudaPower = 1;

    private const float SPEED_MULTIPLIER = 10;

    void Start()
    {
        TargetPos = transform.position;
    }

    void Update()
    {
        if (isSelected)
        {
            Vector2 pos = WorldGenerator.instance.CheckClick();
            if (pos != Vector2.zero)
            {
                this.TargetPos = new Vector3(pos.x, WorldGenerator.instance.GetPosHeight((int)pos.x, (int)pos.y), pos.y);
            }
        }


        UpdateEffect();
    }

    void FixedUpdate()
    {
        // Debug.Log(TargetPos);
        if (Vector3.Distance(transform.position, TargetPos) < speed * SPEED_MULTIPLIER * 2)
        {
            transform.position = TargetPos;
        }
        else
        {
            Vector3 dir = (TargetPos - transform.position).normalized * speed * SPEED_MULTIPLIER;
            Vector3 pretend = transform.position + dir;
            pretend.y = WorldGenerator.instance.GetPosHeight((int)pretend.x, (int)pretend.z);
            transform.position = pretend;
        }
    }


    public void SelectThis()
    {
        this.TipIcon.SetActive(true);
        isSelected = true;
    }

    public void UnSelectThis()
    {
        this.TipIcon.SetActive(false);
        isSelected = false;
    }





    public void InitData(int unitIndex)
    {
        this.UnitIndex = unitIndex;

        switch (unitIndex)
        {
            case 0: // 扑打队员
                speed = 0.3f;
                pudaPower = 0.1f;
                break;
            case 1: // 风力灭火机
                speed = 0.2f;
                pudaPower = 0.2f;
                break;
        }
    }


    private float effectTimer = 0.0f;
    private float effectTime = 1.0f;
    public void UpdateEffect()
    {
        effectTimer += Time.deltaTime;
        if (effectTimer > effectTime)
        {
            effectTimer = 0;

            // 扑打队员和风力灭火机
            if (UnitIndex == 0 || UnitIndex == 1)
            {
                Debug.Log("puda");
                FireSimulator.instance.PudaAt(
                    new Vector2(transform.position.x, transform.position.z));
            }
        }
    }
}
