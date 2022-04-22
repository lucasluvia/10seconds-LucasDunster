using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBehaviour : MonoBehaviour
{
    public CameraType type;
    public LayerMask Mask;
    [SerializeField] bool isFPCam = false;

    PlayerController player;
    CinemachineVirtualCamera vCam;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        vCam = gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (isFPCam)
            SprintControls();
    }

    private void SprintControls()
    {
        if (player.isSprinting)
        {
            //change fov
            if (vCam.m_Lens.FieldOfView < 80)
                vCam.m_Lens.FieldOfView++;
            // adjust camera height to make it feel smoother
            //player.followTarget.position = new Vector3(player.followTarget.position.x, player.transform.position.y + 1.3f, player.followTarget.position.z);
        }
        else
        {
            //change back
            if (vCam.m_Lens.FieldOfView > 60)
                vCam.m_Lens.FieldOfView--;

            //player.followTarget.position = new Vector3(player.followTarget.position.x, player.transform.position.y + 1.1f, player.followTarget.position.z);
        }
    }

    public void SetCullingMask()
    {
        Camera.main.cullingMask = Mask;
    }

}
