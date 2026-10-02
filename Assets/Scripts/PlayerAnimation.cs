using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator playerAnim;
    public float locomotionBlendSpeed = 0.02f;

    public PlayerLocomotionInput playerLocomotionInput;

    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");


    Vector3 currentBlendInput = Vector3.zero;

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();

        playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        Vector2 inputTarget = playerLocomotionInput.MovementInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

        playerAnim.SetFloat(inputXHash, currentBlendInput.x);
        playerAnim.SetFloat(inputYHash, currentBlendInput.y);
    }
}
