using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Checkpoints")]
    public Transform currentCheckpoint;
    public List<CheckpointBehaviour> checkpoints = new List<CheckpointBehaviour>();
    public int CurrentProgression = 0;
    public Transform StartSpawn;

    void Awake()
    {
        foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            if (checkpoint.GetComponent<CheckpointBehaviour>())
            {
                checkpoints.Add(checkpoint.GetComponent<CheckpointBehaviour>());
                checkpoint.GetComponent<CheckpointBehaviour>().progressionNumber = checkpoints.Count;
            }
        }

        StartSpawn = GameObject.FindWithTag("StartPoint").GetComponent<Transform>();
        SetCurrentSpawn(StartSpawn, -1);
    }

    // Pass in -1 to override the checking process and always set spawn
    public void SetCurrentSpawn(Transform newSpawn, int progressionNumber)
    {
        if (progressionNumber == -1 || progressionNumber > CurrentProgression)
        {
            currentCheckpoint = newSpawn;
        }

    }

}
