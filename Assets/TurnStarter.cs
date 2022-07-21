using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStarter : MonoBehaviour
{
    public GameObject StartToTurn;

    public void StartTurn()
    {
        if (StartToTurn != null)
        {
            StartToTurn.SetActive(true);

        }
    }
}
