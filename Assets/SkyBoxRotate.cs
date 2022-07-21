using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRotate : MonoBehaviour
{
    public float speed = 1f;
    bool isRotate = false;
    private void FixedUpdate()
    {
        if (isRotate)
            RenderSettings.skybox.SetFloat("_Rotation", Time.deltaTime * speed);
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
