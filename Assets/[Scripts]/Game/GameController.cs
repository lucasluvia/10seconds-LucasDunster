using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Checkpoints")]
    public Transform currentCheckpoint;
    public List<CheckpointBehaviour> checkpoints = new List<CheckpointBehaviour>();
    public List<GameObject> platforms = new List<GameObject>();
    public int CurrentProgression = 0;
    public Transform StartSpawn;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource CheckPoint;

    Timer timer;

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
        foreach (GameObject platform in GameObject.FindGameObjectsWithTag("Platform"))
        {
            platforms.Add(platform);
        }

        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
        StartSpawn = GameObject.FindWithTag("StartPoint").GetComponent<Transform>();
        SetCurrentSpawn(StartSpawn, -1);
    }

    // Pass in -1 to override the checking process and always set spawn
    public void SetCurrentSpawn(Transform newSpawn, int progressionNumber)
    {
        //Debug.Log(progressionNumber + ">" + CurrentProgression);

        if (progressionNumber == -1 || progressionNumber > CurrentProgression)
        {
            if (progressionNumber > 1)
                CheckPoint.Play();
            CurrentProgression = progressionNumber;
            currentCheckpoint = newSpawn;
        }

    }

    public void SetPlatformColour(Color colour)
    {
        foreach (GameObject platform in platforms)
        {
            Mesh mesh = platform.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;

            // create new colors array where the colors will be created.
            Color[] colors = new Color[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
                colors[i] = colour;

            // assign the array of colors to the Mesh.
            mesh.colors = colors;
        }

    }

    public bool IsInCountdown()
    {
        switch (timer.timerState)
        {
            case TimerState.COUNTDOWN:
                return true;
                break;
            case TimerState.COOLDOWN:
                return false;
                break;
        }
        Debug.Log("BAD SIGN!!!!!!!!!");
        return false;
    }


}
