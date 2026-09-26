using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCamera playerCamera;

    private void Awake()
    {
        playerMovement.enabled = false;
        playerCamera.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            playerMovement.enabled = true;
            playerCamera.enabled = true;
        }
    }
}
