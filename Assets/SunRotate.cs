using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotate : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 20f;
    bool isRotate = false;

    void Update()
    {
        if (isRotate)
        {
            transform.RotateAround(target.position, -Vector3.up, rotationSpeed * Time.deltaTime);

        }
    }

    public void StartButtonClick()
    {
        isRotate = true;
    }
    public void StopButtonClick()
    {
        isRotate = false;
    }
}
