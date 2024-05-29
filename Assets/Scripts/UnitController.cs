using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
// using UnityEditor.Rendering;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [ReadOnly]
    public int UnitIndex = 0;
    [ReadOnly]
    public bool isSelected = false;
    [ReadOnly]
    public UnitUI BindedUI;
    [ReadOnly]
    public Vector3 TargetPos;

    public GameObject TipIcon;

    private float speed = 1;

    private float pudaPower = 1;

    private int pudaRange = 0;

    private const float SPEED_MULTIPLIER = 10;

    public UnitAnimType animType = UnitAnimType.Fireman;


    private Animator animator;

    private LineRenderer lineRenderer;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = new Color(1, 1, 1, 0.5f);  // 半透明白色
        lineRenderer.endColor = new Color(1, 1, 1, 0.5f);
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
    }

    void Start()
    {
        TargetPos = transform.position;
    }

    void Update()
    {
        if (!GameManager.instance.bStartGame || GameManager.instance.bEndGame) return;
        if (isSelected)
        {
            Vector2 pos = WorldGenerator.instance.CheckClick();
            if (pos != Vector2.zero)
            {
                // Check River
                bool checkRiverResult = WorldGenerator.instance.CheckRiver(new Vector2Int(
                    Mathf.FloorToInt(transform.position.x),
                    Mathf.FloorToInt(transform.position.z)
                ), new Vector2Int(
                    Mathf.FloorToInt(pos.x),
                    Mathf.FloorToInt(pos.y)
                ));

                // Debug.Log(checkRiverResult);
                if (checkRiverResult && this.UnitIndex != 4)
                {

                }
                else
                {
                    this.TargetPos = new Vector3(pos.x, WorldGenerator.instance.GetPosHeight((int)pos.x, (int)pos.y), pos.y);
                }


            }
        }


        UpdateEffect();


        UpdateMove();
    }

    void UpdateMove()
    {
        // Debug.Log(TargetPos);
        bool enoughClose = Vector3.Distance(transform.position, TargetPos) < speed * SPEED_MULTIPLIER * 2;
        if (enoughClose)
        {
            transform.position = TargetPos;
            showPath(false);
        }
        else
        {
            Vector3 dir = (TargetPos - transform.position).normalized * speed * SPEED_MULTIPLIER * Time.deltaTime * 10;
            Vector3 pretend = transform.position + dir;
            pretend.y = WorldGenerator.instance.GetPosHeight((int)pretend.x, (int)pretend.z);
            transform.position = pretend;
            showPath(true);
        }


        // update anim
        if (animator != null)
        {
            switch (animType)
            {
                case UnitAnimType.Fireman:
                    {
                        if (transform.position != TargetPos)
                        {

                            animator.SetBool("moving", true);
                            transform.LookAt(
                                new Vector3(TargetPos.x, transform.position.y, TargetPos.z)
                            );
                        }
                        else
                        {
                            animator.SetBool("moving", false);
                        }

                    }
                    break;
            }
        }
        else
        {
            transform.LookAt(
                new Vector3(TargetPos.x, transform.position.y, TargetPos.z)
            );
        }

    }

    void showPath(bool show)
    {
        if (show)
        {
            // 设置LineRenderer的点数
            lineRenderer.positionCount = 2;

            // 设置起点和终点
            lineRenderer.SetPosition(0, transform.position + Vector3.up * 20);
            lineRenderer.SetPosition(1, TargetPos + Vector3.up * 20);
            float length = Vector3.Distance(transform.position, TargetPos);
            lineRenderer.material.mainTextureScale = new Vector2(length * 0.04f, 1);
            // lineRenderer.material.
        }
        else
        {
            // 隐藏线，可以通过设置点数为0来实现
            lineRenderer.positionCount = 0;
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
                pudaPower = 0.2f;
                pudaRange = 50;
                break;
            case 1: // 风力灭火机
                speed = 0.1f;
                pudaPower = 0.4f;
                pudaRange = 100;
                break;
            case 2: // 灭火水枪
                speed = 0.2f;
                pudaPower = 0.3f;
                pudaRange = 100;
                break;
            case 3: // 隔离阻火
                speed = 0.3f;
                pudaPower = 0.1f;
                pudaRange = 50;
                break;
            case 4: // 飞机
                speed = 0.5f;
                pudaPower = 0.3f;
                pudaRange = 50;
                break;
            case 5: // 点火
                break;
        }
    }


    private float effectTimer = 0.0f;
    private float effectTime = 1.0f;
    public void UpdateEffect()
    {
        if (BindedUI.EnableAuto)
        {
            effectTimer += Time.deltaTime;
            if (effectTimer > effectTime)
            {
                effectTimer = 0;

                switch (UnitIndex)
                {
                    case 0: // 扑打队员
                        PudaEffect();
                        break;
                    case 1: // 风力灭火机
                        FengliEffect();
                        break;
                    case 2: //水
                        {
                            if (WorldGenerator.instance.CheckWater(
                                new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)), 100))
                            {
                                BindedUI.FillWater();
                            }
                            Vector2 checkFireDir = FireSimulator.instance.ChecKFire(getMapPos(), pudaRange);
                            if (checkFireDir != Vector2.zero)
                            {
                                if (BindedUI.UseWater())
                                {
                                    ShuiEffect(checkFireDir);
                                }
                            }

                        }
                        break;
                    case 3: // 隔离
                        KanfaEffect();
                        break;
                    case 4: // 飞机
                        {
                            if (WorldGenerator.instance.CheckWater(
                                new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)), 100))
                            {
                                BindedUI.FillWater();
                            }
                            Vector2 checkFireDir = FireSimulator.instance.ChecKFire(getMapPos(), pudaRange);
                            Debug.Log("feiji" + checkFireDir);
                            if (checkFireDir != Vector2.zero)
                            {
                                if (BindedUI.UseWater(2))
                                {
                                    FeijiEffect(checkFireDir);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }


    void PudaEffect()
    {
        Vector2 checkFireDir = FireSimulator.instance.ChecKFire(getMapPos(), pudaRange);
        if (checkFireDir != Vector2.zero)
        {
            FireSimulator.instance.PudaAtDir(
                getMapPos(), checkFireDir, 45.0f, pudaPower, pudaRange);
            VFXManager.instance.ShowVFX(VFXType.Puda, transform.position + Vector3.up * 20, checkFireDir);
            transform.LookAt(transform.position + new Vector3(checkFireDir.x, 0, checkFireDir.y) * 10000);
        }
    }

    void FengliEffect()
    {
        Vector2 checkFireDir = FireSimulator.instance.ChecKFire(getMapPos(), pudaRange);
        if (checkFireDir != Vector2.zero)
        {
            FireSimulator.instance.PudaAtDir(
                getMapPos(), checkFireDir, 45.0f, pudaPower, pudaRange);
            VFXManager.instance.ShowVFX(VFXType.Fengli, transform.position + Vector3.up * 20, checkFireDir);
            transform.LookAt(transform.position + new Vector3(checkFireDir.x, 0, checkFireDir.y) * 10000);
        }
    }

    void ShuiEffect(Vector2 checkFireDir)
    {
        {
            FireSimulator.instance.PudaAtDir(
                getMapPos(), checkFireDir, 30.0f, pudaPower, pudaRange);
            VFXManager.instance.ShowVFX(VFXType.Shui, transform.position + Vector3.up * 20, checkFireDir);
            transform.LookAt(transform.position + new Vector3(checkFireDir.x, 0, checkFireDir.y) * 10000);
        }
    }

    void KanfaEffect()
    {
        // Vector2 checkFireDir = FireSimulator.instance.ChecKFire(getMapPos(), pudaRange);
        // if (checkFireDir != Vector2.zero)
        {
            FireSimulator.instance.KanTree(
                Mathf.FloorToInt(getMapPos().x), Mathf.FloorToInt(getMapPos().y));
            // VFXManager.instance.ShowVFX(VFXType.Fengli, transform.position + Vector3.up * 20, checkFireDir);
            // transform.LookAt(transform.position + new Vector3(checkFireDir.x, 0, checkFireDir.y) * 10000);
        }
    }

    void FeijiEffect(Vector2 checkFireDir)
    {
        {
            FireSimulator.instance.PudaAtDir(
                getMapPos(), checkFireDir, 360.0f, pudaPower, pudaRange);
            VFXManager.instance.ShowVFX(VFXType.Shui, transform.position + Vector3.up * 20, checkFireDir);
            transform.LookAt(transform.position + new Vector3(checkFireDir.x, 0, checkFireDir.y) * 10000);
        }
    }

    public void FireEffect()
    {
        FireSimulator.instance.FireAt(getMapPos());
        Debug.Log("点火");
    }




    Vector2 getMapPos()
    {
        return new Vector2(transform.position.x, transform.position.z);
    }
}

public enum UnitAnimType
{
    Fireman
};
