using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    private float rotationSpeed = 2;

    void Update()
    {
        transform.Rotate(new Vector3(0,45,0) * rotationSpeed * Time.deltaTime);

    }
}
