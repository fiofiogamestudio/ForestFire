using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TreeController : MonoBehaviour
{
    // public Texture2D FuelsMap;

    public Material Black;

    public bool Burn = false;
    public int pi;
    public int pj;

    public void SetPos(int i, int j)
    {
        pi = i;
        pj = j;
    }

    public float fuel = 1.0f;
    public float fire = 0.0f;

    public GameObject FireObject;

    public void FixedUpdate()
    {
        // float fuel = GetPixelFromRT(FuelsMap, new Vector2Int(px, py)).r;
        // float fire = GetPixelFromRT(FiresMap, new Vector2Int(px, py)).r;
        float fire = FireSimulator.instance.GetFire(pi, pj);
        float fuel = FireSimulator.instance.GetFuel(pi, pj);

        // if (!Burn && Input.GetKeyDown(KeyCode.Space))
        if (!Burn)
        {
            if (fire > 0.1f)
            {
                FireObject.gameObject.SetActive(true);
            }
            else if (fuel < 0.1f && !Burn)
            {
                FireObject.gameObject.SetActive(false);
                Burn = true;
                BecomeBlack();
            }
        }
    }

    [ContextMenu("test")]
    public void BecomeBlack()
    {
        // Debug.Log("Burn!");
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = Black;
            }
            renderer.materials = materials;
        }
    }




    void Update()
    {
        // 检查鼠标左键是否被按下
        if (Input.GetMouseButtonDown(1))
        {
            // 从摄像机到鼠标位置发出射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;



            // 进行射线投射
            if (Physics.Raycast(ray, out hit))
            {
                // 检查射线是否与Tree物体的Collider相交
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    // 调用处理Tree交互的函数
                    HandleTreeInteraction(hit.collider.gameObject);
                }
            }
        }
    }

    void HandleTreeInteraction(GameObject tree)
    {
        // 在这里编写处理Tree交互的逻辑
        // 例如：tree.GetComponent<TreeController>().SomeFunction();
        // Debug.Log("Tree was clicked!");
        BecomeBlack();

        FireSimulator.instance.KanTree(pi, pj);
    }
}
