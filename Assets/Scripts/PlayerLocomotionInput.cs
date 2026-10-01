using UnityEngine;
using UnityEngine.InputSystem;



[DefaultExecutionOrder(-2)]

public class PlayerLocomotionInput : MonoBehaviour, InputSystem_Actions.IPlayerActions
{

    public InputSystem_Actions controls { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }


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

   

    

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    


}
