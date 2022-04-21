using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    public Transform spawnPoint;
    public int progressionNumber;
    //public bool isTriggered; // For potential progress bar

    GameController gameController;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        spawnPoint = GetComponentsInChildren<Transform>()[1];
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;

        gameController.SetCurrentSpawn(spawnPoint, progressionNumber);

    }


}
