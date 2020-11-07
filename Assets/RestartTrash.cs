using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartTrash : MonoBehaviour
{
    // game object with game management scripts in order to restart the game
    [SerializeField] private GameObject manager;

    public void resetGame()
    {
        manager.GetComponent<SetUpTrash>().enabled = false;
        manager.GetComponent<ClickManager>().enabled = false;
        manager.GetComponent<SetUpTrash>().enabled = true;
        manager.GetComponent<ClickManager>().enabled = true;
    }
}
