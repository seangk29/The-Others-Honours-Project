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

    public float drag;
    public float movingThreshold = 0.01f;


    [Header("Camera Settings")]
    public float lookSenseH;
    public float lookSenseV;
    public float lookLimitV = 89f;

    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    private PlayerLocomotionInput locomotionInput;
    private PlayerState playersState;
    #endregion

    #region Start Up
    private void Awake()
    {
        locomotionInput = GetComponent<PlayerLocomotionInput>();
        playersState = GetComponent<PlayerState>();
    }
    #endregion

    #region Update Logic
    private void Update()
    {
        HandleLateralMovement();
        UpdateMovementState();
    }


    void UpdateMovementState()
    {
        bool isMovementInput = locomotionInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = locomotionInput.sprintToggledOn && isMovingLaterally;

        PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
                                           isMovingLaterally || isMovementInput ? PlayerMovementState.Running : 
                                           PlayerMovementState.Idle;
        
        playersState.SetPlayerMovementState(lateralState);
    }

    
    void HandleLateralMovement()
    {
        // Create quick references for current state
        bool isSprinting = playersState.currentPlayerMovementState == PlayerMovementState.Sprinting;

        // State dependant acceleration and speed
        float lateralAcceleration = isSprinting ? sprintAcceleration : runAcceleration;
        float clampLateralMagnitude = isSprinting ? sprintSpeed : runSpeed;
        
        
        Vector3 camForwardXZ = new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z).normalized;
        Vector3 camRightXZ = new Vector3(cam.transform.right.x, 0f, cam.transform.right.z).normalized;
        Vector3 movementDirection = camRightXZ * locomotionInput.MovementInput.x + camForwardXZ * locomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);

        characterController.Move(newVelocity * Time.deltaTime);
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
    #endregion
}
