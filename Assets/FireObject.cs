using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObject : MonoBehaviour
{
    // Start is called before the first frame update
    public void Fire()
    {
        FireSimulator.instance.FireAt(new Vector2(transform.position.x, transform.position.z));

        GameObject.Destroy(this.gameObject);
    }
}
