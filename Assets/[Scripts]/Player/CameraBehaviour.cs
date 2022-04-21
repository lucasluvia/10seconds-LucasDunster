using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBehaviour : MonoBehaviour
{
    PlayerController player;


    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        SprintControls();

    }

    private void SprintControls()
    {
        //sprint changes
        if (player.isSprinting)
        {
            //change fov
            gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 80;
            //lower camera
            player.followTarget.transform.position = new Vector3(player.followTarget.transform.position.x, player.transform.position.y + 1.3f, player.followTarget.transform.position.z);
        }
        else
        {
            //change back
            gameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;

            player.followTarget.transform.position = new Vector3(player.followTarget.transform.position.x, player.transform.position.y + 1.1f, player.followTarget.transform.position.z);
        }
    }

}
