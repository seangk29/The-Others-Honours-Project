using UnityEngine;


[DefaultExecutionOrder(-1)]

public class PlayerController : MonoBehaviour
{


    [Header("Components")]
    public CharacterController characterController;
    public Camera cam;

    [Header("Base Movement")]
    public float runAcceleration;
    public float runSpeed;
    public float drag;


    [Header("Camera Settings")]
    public float lookSenseH;
    public float lookSenseV;
    public float lookLimitV = 89f;

    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    private PlayerLocomotionInput locomotionInput;

    private void Awake()
    {
        locomotionInput = GetComponent<PlayerLocomotionInput>();
    }


    private void Update()
    {
      HandleLateralMovement();
    }

    void HandleLateralMovement()
    {
        Vector3 camForwardXZ = new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z).normalized;
        Vector3 camRightXZ = new Vector3(cam.transform.right.x, 0f, cam.transform.right.z).normalized;
        Vector3 movementDirection = camRightXZ * locomotionInput.MovementInput.x + camForwardXZ * locomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        characterController.Move(newVelocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        cameraRotation.x += lookSenseH * locomotionInput.LookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * locomotionInput.LookInput.y, -lookLimitV, lookLimitV);

        playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * locomotionInput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0f);

        cam.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
    }

}
