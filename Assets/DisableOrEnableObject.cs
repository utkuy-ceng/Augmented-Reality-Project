using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableOrEnableObject : MonoBehaviour
{
    public GameObject turn;
    private bool satelliteEnabled;
    public Transform target;
    public float rotationSpeed = 20;
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(StartAndFinish);
        satelliteEnabled = true;
        turn.SetActive(satelliteEnabled);
    }

    public void StartAndFinish()
    {
        satelliteEnabled ^= true;
        turn.SetActive(satelliteEnabled);
        transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
