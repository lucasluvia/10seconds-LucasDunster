using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Player State Variables")]
    public bool isGrounded;
    public bool isJumping;
    public bool isSprinting;

    [Header("Movement Variables")]
    [SerializeField] float walkSpeed = 5.0f;
    [SerializeField] float runSpeed = 10.0f;
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float airVelocityMax = 5.0f;

    [Header("Camera Variables")]
    public GameObject followTarget;
    [SerializeField] float aimSensitivity = 1.0f;

    GameController gameController;

    //Player Components
    Rigidbody rigidbody;
    Animator playerAnimator;

    // References
    Vector2 inputVector = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector2 lookInput = Vector3.zero;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        rigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        GoToCurrentSpawnpoint();
    }

    void Update()
    {
        //aimsing,looking
        followTarget.transform.rotation *= Quaternion.AngleAxis(lookInput.x * aimSensitivity, Vector3.up);
        followTarget.transform.rotation *= Quaternion.AngleAxis(lookInput.y * aimSensitivity, Vector3.left);

        var angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTarget.transform.localEulerAngles.x;

        if (angle > 180 && angle < 300)
        {
            angles.x = 300;
        }
        else if (angle < 180 && angle > 60)
        {
            angles.x = 60;
        }

        followTarget.transform.localEulerAngles = angles;

        //rotate player based on look transform
        transform.rotation = Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
        followTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        //movement
        if (!(inputVector.magnitude > 0)) moveDirection = Vector3.zero;

        moveDirection = transform.forward * inputVector.y + transform.right * inputVector.x;
        float currentSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 movementDirection = moveDirection * (currentSpeed * Time.deltaTime);

        transform.position += movementDirection;

        if (!gameController.IsInCountdown() && isGrounded)
        {
            GoToCurrentSpawnpoint();
        }

        if(isGrounded && isJumping)
        {
            isJumping = false;
        }

    }

    void FixedUpdate()
    {
        float currentSpeed;

        if (isSprinting) currentSpeed = runSpeed;
        else currentSpeed = walkSpeed;

        Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        if (localVelocity.x > currentSpeed)
            localVelocity.x = currentSpeed;
        if (localVelocity.z > currentSpeed)
            localVelocity.z = currentSpeed;

        rigidbody.velocity = transform.TransformDirection(localVelocity);

    }

    private void OnCollisionEnter(Collision other)
    {
        if ((!other.gameObject.CompareTag("Platform") || !other.gameObject.CompareTag("Checkpoint")) && !isJumping) return;

        isJumping = false;


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }

    }

    public void GoToCurrentSpawnpoint()
    {
        rigidbody.velocity = Vector3.zero;
        transform.position = gameController.currentCheckpoint.position;
    }



    public void OnMovement(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (isJumping)
            return;

        isJumping = value.isPressed;
        rigidbody.AddForce((transform.up + moveDirection) * jumpForce, ForceMode.Impulse);
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

}
