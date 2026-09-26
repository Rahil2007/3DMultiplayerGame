using System.Globalization;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform playerHead;
    private void Start()
    {
        GameObject vCamObject = GameObject.FindGameObjectWithTag("VirtualCamera");
        if (vCamObject != null)
        {
            var vCam = vCamObject.GetComponent<CinemachineCamera>();
            if (vCam != null)
            {
                vCam.Follow = playerHead;
                vCam.LookAt = playerHead;
            }   
        }
    }
}
