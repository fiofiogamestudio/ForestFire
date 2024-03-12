using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    private float rotationSpeed = 450.0f; // 每秒旋转角度

    // Update is called once per frame
    void Update()
    {
        // 绕 y 轴旋转物体
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
