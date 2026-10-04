using UnityEngine;

public class PlayerState : MonoBehaviour
{

    [field: SerializeField] public PlayerMovementState currentPlayerMovementState { get; private set; } = PlayerMovementState.Idle;

    public void SetPlayerMovementState(PlayerMovementState playerMovementState)
    {
        currentPlayerMovementState = playerMovementState;
    }
    

    public bool InGroundedState()
    {
        return currentPlayerMovementState == PlayerMovementState.Idle ||
               currentPlayerMovementState == PlayerMovementState.Walking ||
               currentPlayerMovementState == PlayerMovementState.Running ||
               currentPlayerMovementState == PlayerMovementState.Sprinting ||
               currentPlayerMovementState == PlayerMovementState.Crouching;
    }
}

public enum PlayerMovementState
{
    Idle = 0,
    Walking = 1,
    Running = 2,
    Sprinting = 3,
    Jumping = 4,
    Falling = 5,
    Strafing = 6,
    Crouching = 7,

}
