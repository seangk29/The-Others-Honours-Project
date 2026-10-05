using UnityEngine;
using UnityEngine.InputSystem;



[DefaultExecutionOrder(-2)]

public class PlayerLocomotionInput : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    #region Class Variables
    public bool holdToSprint = true;
    public bool sprintToggledOn {  get; private set; }

    public bool holdToCrouch = true;
    public bool crouchToggledOn { get; private set; }
    
    public InputSystem_Actions controls { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool JumpPressed { get; private set; }
    #endregion

    #region Start Up
    void OnEnable()
    {
        controls = new InputSystem_Actions();
        controls.Enable();

        controls.Player.Enable();
        controls.Player.SetCallbacks(this);

    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Player.RemoveCallbacks(this);
    }
    #endregion


    #region Late Update Logic
    private void LateUpdate()
    {
        JumpPressed = false;
    }
    #endregion


    #region Input Callbacks
    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnToggleSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sprintToggledOn = holdToSprint || !sprintToggledOn;
        }
        else if (context.canceled)
        {
            sprintToggledOn = !holdToSprint && sprintToggledOn;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        JumpPressed = true;
    }

    public void OnToggleCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        crouchToggledOn = !crouchToggledOn;
    }

    #endregion
}
