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
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float groundDrag = 2f;
    [SerializeField] private float airDrag = 1f;

    private Transform cameraTransform;

    [Header("Jumping")]
    [SerializeField] private float fallMult = 2f;
    [SerializeField] private float baseGravity = -9.81f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] Transform groundCheck;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpCoolDown = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float jumpCoolDownCounter;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
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
        MovementDrag();
        Jump();
        VariableGravity();
        rb.MoveRotation(Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f));
    }

    void Update()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (playerInput.Player.Jump.triggered && jumpCoolDownCounter <= 0f)
        {
            jumpBufferCounter = jumpBufferTime;
            jumpCoolDownCounter = jumpCoolDown;
        }

        if(jumpCoolDown > 0f)
            jumpCoolDownCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

    }

    void Jump()
    {
        if(jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    void VariableGravity()
    {
        if(rb.linearVelocity.y < 0f)
            rb.AddForce(Vector3.up * baseGravity * fallMult, ForceMode.Acceleration);
        else
            rb.AddForce(Vector3.up * baseGravity, ForceMode.Acceleration);
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
        rb.linearDamping = IsGrounded()? groundDrag : airDrag;
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
        return Physics.Raycast(groundCheck.position, Vector3.down, 0.1f, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheck.position, Vector3.down * 0.1f);
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
