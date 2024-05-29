using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXLife : MonoBehaviour
{
    public float lifeTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Destroy(this.gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
