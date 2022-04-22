using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    PLAY,
    DEATH,
    FALL,
    START,
    WIN
}

public class PlayerController : MonoBehaviour
{
    [Header("Player State Variables")]
    public bool isGrounded;
    public bool isJumping;
    public bool isSprinting;
    public bool canControl;
    [SerializeField] PlayerState playerState;

    [Header("Movement Variables")]
    [SerializeField] float walkSpeed = 5.0f;
    [SerializeField] float runSpeed = 10.0f;
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float airVelocityMax = 5.0f;

    [Header("Camera Variables")]
    public Transform followTarget;
    [SerializeField] float aimSensitivity = 1.0f;

    GameController gameController;
    CameraController cameraController;

    float waitTimer;

    //Player Components
    Rigidbody rigidbody;
    Animator playerAnimator;

    // References
    Vector2 inputVector = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector2 lookInput = Vector3.zero;

    // Animator Hashes
    public readonly int deathHash = Animator.StringToHash("IsDeath");
    public readonly int startHash = Animator.StringToHash("IsStart");
    public readonly int winHash = Animator.StringToHash("IsWin");
    public readonly int fallHash = Animator.StringToHash("IsFall");

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        cameraController = GameObject.FindWithTag("CameraController").GetComponent<CameraController>();
        rigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        SetPlayerState(PlayerState.START);
    }

    void Update()
    {
        switch (playerState)
        {
            case PlayerState.PLAY:
                PlayUpdate();
                break;
            case PlayerState.DEATH:
                NonPlayUpdate(2f, PlayerState.START);
                break;
            case PlayerState.FALL:
                NonPlayUpdate(1.5f, PlayerState.START);
                break;
            case PlayerState.START:
                NonPlayUpdate(0.5f, PlayerState.PLAY);
                break;
            case PlayerState.WIN:
                WinUpdate();
                break;
        }
    }

    void FixedUpdate()
    {
        if (playerState != PlayerState.PLAY) return;

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

    void PlayUpdate()
    {
        //aimsing,looking
        followTarget.rotation *= Quaternion.AngleAxis(lookInput.x * aimSensitivity, Vector3.up);
        followTarget.rotation *= Quaternion.AngleAxis(lookInput.y * aimSensitivity, Vector3.left);

        var angles = followTarget.localEulerAngles;
        angles.z = 0;

        var angle = followTarget.localEulerAngles.x;

        if (angle > 180 && angle < 300)
        {
            angles.x = 300;
        }
        else if (angle < 180 && angle > 60)
        {
            angles.x = 60;
        }

        followTarget.localEulerAngles = angles;

        //rotate player based on look transform
        transform.rotation = Quaternion.Euler(0, followTarget.rotation.eulerAngles.y, 0);
        followTarget.localEulerAngles = new Vector3(angles.x, 0, 0);

        //movement
        if (!(inputVector.magnitude > 0)) moveDirection = Vector3.zero;

        moveDirection = transform.forward * inputVector.y + transform.right * inputVector.x;
        float currentSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 movementDirection = moveDirection * (currentSpeed * Time.deltaTime);

        transform.position += movementDirection;

        if (!gameController.IsInCountdown() && isGrounded)
        {
            SetPlayerState(PlayerState.DEATH);
        }

        if (isGrounded && isJumping)
        {
            isJumping = false;
        }

    }

    void NonPlayUpdate(float timeInUpdate, PlayerState nextState)
    {
        waitTimer += Time.deltaTime;


        if (waitTimer > timeInUpdate)
        {
            waitTimer = 0f;
            SetPlayerState(nextState);
        }
    }

    void WinUpdate()
    {
        // disable UI + timer
        // stop culling character
        // play win animation
        // turn camera and zoom out
        // show end level UI
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

        if(other.gameObject.CompareTag("FallPlane"))
        {
            SetPlayerState(PlayerState.FALL);
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

    public void SetPlayerState(PlayerState newState)
    {
        playerState = newState;

        playerAnimator.SetBool(deathHash, false);
        playerAnimator.SetBool(winHash, false);
        playerAnimator.SetBool(fallHash, false);
        playerAnimator.SetBool(startHash, false);

        CameraType newCamType = CameraType.FP_CAM;
        switch (newState)
        {
            case PlayerState.PLAY:
                newCamType = CameraType.FP_CAM;
                break;
            case PlayerState.DEATH:
                newCamType = CameraType.BACK_CAM;
                playerAnimator.SetBool(deathHash, true);
                break;
            case PlayerState.FALL:
                newCamType = CameraType.FALL_CAM;
                playerAnimator.SetBool(fallHash, true);
                break;
            case PlayerState.START:
                GoToCurrentSpawnpoint();
                playerAnimator.SetBool(startHash, true);
                isGrounded = false;
                newCamType = CameraType.BACK_CAM;
                break;
            case PlayerState.WIN:
                playerAnimator.SetBool(winHash, true);
                newCamType = CameraType.FRONT_CAM;
                break;
        }
        cameraController.SetActiveCamera(newCamType);

        // Set Animation State

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
