using Unity.VisualScripting;
using UnityEngine;


[DefaultExecutionOrder(-1)]

public class PlayerController : MonoBehaviour
{

    #region Class Variables
    [Header("Components")]
    public CharacterController characterController;
    public Camera cam;

    [Header("Base Movement")]
    public float runAcceleration;
    public float runSpeed;

    public float sprintAcceleration;
    public float sprintSpeed;

    public float airAcceleration;

    public float drag;
    public float movingThreshold = 0.01f;

    public float gravity = 25f;
    public float jumpSpeed = 1f;

    public float crouchAcceleration;
    public float crouchSpeed;
    public float crouchYScale;
    public float startYScale;

    [Header("Camera Settings")]
    public float lookSenseH;
    public float lookSenseV;
    public float lookLimitV = 89f;

    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;
    private float verticalVelocity = 0f;

    private PlayerLocomotionInput locomotionInput;
    private PlayerState playersState;
    private EnemyController enemy;
    #endregion

    #region Start Up
    private void Awake()
    {
        locomotionInput = GetComponent<PlayerLocomotionInput>();
        playersState = GetComponent<PlayerState>();

        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();

        startYScale = transform.localScale.y;
    }
    #endregion

    #region Update Logic
    private void Update()
    {
        
        UpdateMovementState();
        HandleVerticalMovement();
        HandleLateralMovement();
        HandleCrouching();
      


    }


    void UpdateMovementState()
    {
        //bool canRun = CanRun();
        bool isMovementInput = locomotionInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = locomotionInput.sprintToggledOn && isMovingLaterally;
        bool isCrouching = /*(isMovingLaterally && !canRun)  || */ locomotionInput.crouchToggledOn;
        bool isGrounded = IsGrounded();
        

        PlayerMovementState lateralState = isCrouching ? PlayerMovementState.Crouching :
                                           isSprinting ? PlayerMovementState.Sprinting : isMovingLaterally ||
                                           isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idle;
        
        playersState.SetPlayerMovementState(lateralState);


        //Control Airborn State
        if (!isGrounded && characterController.velocity.y > 0f)
        {
            playersState.SetPlayerMovementState(PlayerMovementState.Jumping);
        }
        else if (!isGrounded && characterController.velocity.y <= 0f)
        {
            playersState.SetPlayerMovementState(PlayerMovementState.Falling);
        }

    }

    void HandleVerticalMovement()
    {
        bool isGrounded = playersState.InGroundedState();

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }

        verticalVelocity -= gravity * Time.deltaTime;

        if (locomotionInput.JumpPressed && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpSpeed * 3 * gravity);
        }
    }


    
    
    void HandleLateralMovement()
    {
        // Create quick references for current state
        bool isSprinting = playersState.currentPlayerMovementState == PlayerMovementState.Sprinting;
        bool isGrounded = playersState.InGroundedState();
        bool isCrouching = playersState.currentPlayerMovementState == PlayerMovementState.Crouching;
       

        // State dependant acceleration and speed
        float lateralAcceleration = isCrouching ? crouchAcceleration :
                                    !isGrounded ? airAcceleration :
                                    isSprinting ? sprintAcceleration : runAcceleration;

        float clampLateralMagnitude =  isCrouching ? crouchSpeed :
                                       !isGrounded ? sprintSpeed:
                                       isSprinting ? sprintSpeed : runSpeed;
        
        
        Vector3 camForwardXZ = new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z).normalized;
        Vector3 camRightXZ = new Vector3(cam.transform.right.x, 0f, cam.transform.right.z).normalized;
        Vector3 movementDirection = camRightXZ * locomotionInput.MovementInput.x + camForwardXZ * locomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);
        newVelocity.y += verticalVelocity;

        characterController.Move(newVelocity * Time.deltaTime);
    }

    void HandleCrouching()
    {
        bool isCrouching = locomotionInput.crouchToggledOn;

        if (isCrouching)
        {
            characterController.transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            enemy.detectionRange = 7f;
           
        }
        else if (!isCrouching)
        {
            characterController.transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            enemy.detectionRange = 10f;
            
        }
    }

    #endregion

    #region Late Update Logic
    private void LateUpdate()
    {
        cameraRotation.x += lookSenseH * locomotionInput.LookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * locomotionInput.LookInput.y, -lookLimitV, lookLimitV);

        playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * locomotionInput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0f);

        cam.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
    }
    #endregion


    #region State Checks
    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.y);

        return lateralVelocity.magnitude > movingThreshold;
    }

    private bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    private bool CanRun()
    {
        return locomotionInput.MovementInput.y >= Mathf.Abs(locomotionInput.MovementInput.x);
    }

    #endregion
}
