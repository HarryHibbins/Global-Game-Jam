using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killfeed : MonoBehaviour
{
    public List<GameObject> list;

    private void Update()
    {
        if (transform.childCount > 3)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
}
