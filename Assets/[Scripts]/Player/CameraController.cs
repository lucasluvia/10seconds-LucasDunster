using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public CameraType activeType;
    List<CameraBehaviour> cameras = new List<CameraBehaviour>();

    void Awake()
    {
        foreach (GameObject camera in GameObject.FindGameObjectsWithTag("VCam"))
        {
            if (camera.GetComponent<CameraBehaviour>())
            {
                cameras.Add(camera.GetComponent<CameraBehaviour>());
            }
        }
    }

    public void SetActiveCamera(CameraType newCam)
    {
        foreach (CameraBehaviour camera in cameras)
        {
            if (camera.type == newCam)
            {
                activeType = newCam;
                camera.transform.gameObject.SetActive(true);
                camera.SetCullingMask();
            }
            else
            {
                camera.transform.gameObject.SetActive(false);
            }

        }
    }

}
