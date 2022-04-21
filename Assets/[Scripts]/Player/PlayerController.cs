using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


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

    [Header("Camera Variables")]
    [SerializeField] GameObject followTarget;
    [SerializeField] float aimSensitivity = 1.0f;

    //Player Components
    Rigidbody rigidbody;
    Animator playerAnimator;

    // References
    Vector2 inputVector = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector2 lookInput = Vector3.zero;

    GameObject vCam;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        vCam = GameObject.FindWithTag("VCam");
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

        //sprint changes
        if(isSprinting)
        {
            //change fov
            vCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 80;
            //lower camera
            followTarget.transform.position = new Vector3(followTarget.transform.position.x, transform.position.y + 1.3f, followTarget.transform.position.z);
        }
        else
        {
            //change back
            vCam.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;

            followTarget.transform.position = new Vector3(followTarget.transform.position.x, transform.position.y + 1.1f, followTarget.transform.position.z);
        }

        //rotate player based on look transform
        transform.rotation = Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
        followTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        //movement
        if (isJumping) return;
        if (!(inputVector.magnitude > 0)) moveDirection = Vector3.zero;

        moveDirection = transform.forward * inputVector.y + transform.right * inputVector.x;
        float currentSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 movementDirection = moveDirection * (currentSpeed * Time.deltaTime);

        transform.position += movementDirection;

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

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Ground") && !isJumping) return;

        isJumping = false;

    }
}
