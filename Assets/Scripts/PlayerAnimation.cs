using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator playerAnim;
    public float locomotionBlendSpeed = 0.02f;

    public PlayerLocomotionInput playerLocomotionInput;
    public PlayerState playerState;

    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int inputMagHash = Animator.StringToHash("inputMagnitude");


    Vector3 currentBlendInput = Vector3.zero;

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();

        playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        playerState = GetComponent<PlayerState>();
        
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        bool isSprinting = playerState.currentPlayerMovementState == PlayerMovementState.Sprinting;
        
        Vector2 inputTarget = isSprinting ? playerLocomotionInput.MovementInput * 1.5f : playerLocomotionInput.MovementInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

       

        playerAnim.SetFloat(inputXHash, currentBlendInput.x);
        playerAnim.SetFloat(inputYHash, currentBlendInput.y);
        playerAnim.SetFloat(inputMagHash, currentBlendInput.magnitude);
    }
}
