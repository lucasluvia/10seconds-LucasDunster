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
    public GameObject followTarget;
    [SerializeField] float aimSensitivity = 1.0f;

    GameController gameController;
    CameraBehaviour camera;

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
        camera = GameObject.Find("Virtual Camera").GetComponent<CameraBehaviour>();
        rigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        GoToCurrentSpawnpoint();
    }

    void Update()
    {
        switch (playerState)
        {
            case PlayerState.PLAY:
                PlayUpdate();
                break;
            case PlayerState.DEATH:
                DeathUpdate();
                break;
            case PlayerState.FALL:
                FallUpdate();
                break;
            case PlayerState.START:
                StartUpdate();
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

    void StartUpdate()
    {
        // player not culled
        // set animator state
        // camera starts behind character
        // camera zoom in to proper position then cull player
        // enable UI + timer and enter Play state
    }

    void PlayUpdate()
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
            // enter death state
            // leave death state
            GoToCurrentSpawnpoint();
            // enter start state
        }

        if (isGrounded && isJumping)
        {
            isJumping = false;
        }

    }

    void DeathUpdate()
    {
        // disable UI + timer
        // stop culling character
        // play death animation
        // zoom out
        // OR stop following but keep tracking player as they fall forwards
        // go to Start state
    }

    void FallUpdate()
    {
        // keep UI?
        // stop culling character
        // play falling animation
        // stop following character and freeze camera position and follow the character as they fall
        // go to Start state
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
