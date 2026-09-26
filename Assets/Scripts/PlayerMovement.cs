using System.IO;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions playerInput;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 1f;
    [SerializeField] private float acceleration = 10f;

    private Transform cameraTransform;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] Transform groundCheck;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new InputSystem_Actions();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        cameraTransform = camObject.transform;
    }

    private void FixedUpdate()
    {
        Move();
        SpeedControl();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f);
        MovementDrag();
    }

    void Move()
    {
        Vector2 moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        Vector3 dirn = (cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x).normalized;
        dirn.y = 0f;
        rb.AddForce(dirn * moveSpeed * acceleration, ForceMode.Force);
    }

    void MovementDrag()
    {
        rb.linearDamping = IsGrounded() ? groundDrag : airDrag;
    }

    void SpeedControl()
    {
        Vector3 flatVal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if(flatVal.magnitude > moveSpeed)
        {
            Vector3 limitedVal = flatVal.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVal.x, rb.linearVelocity.y, limitedVal.z);
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.1f, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void OnDestroy()
    {
        playerInput.Dispose();
    }
}
