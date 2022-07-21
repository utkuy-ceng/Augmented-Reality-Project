using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 20f;
    bool isRotate = false;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
