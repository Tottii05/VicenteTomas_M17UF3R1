using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour, CharacterActions.IMovementActions, CharacterActions.IExtraActions
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody rb;
    private Animator animator;
    private Vector2 movementInput;
    private bool isGrounded;
    private bool isRunning;
    private bool isAiming;
    private bool isJumping;
    private CharacterActions inputActions;
    private ThirdPersonCamera thirdPersonCamera;
    private FirstPersonCamera firstPersonCamera;
    private GameObject weapon;
    private ShootingBehaviour shootingBehaviour;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();
        thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();
        firstPersonCamera = FindObjectOfType<FirstPersonCamera>();
        shootingBehaviour = GetComponent<ShootingBehaviour>();
        inputActions = new CharacterActions();
        inputActions.Movement.SetCallbacks(this);
        inputActions.Extra.SetCallbacks(this);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }

    private void OnEnable()
    {
        inputActions.Movement.Enable();
        inputActions.Extra.Enable();
    }

    private void OnDisable()
    {
        inputActions.Movement.Disable();
        inputActions.Extra.Disable();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float currentSpeed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;

        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        if (move.magnitude >= 0.1f)
        {
            Vector3 moveDirection = transform.TransformDirection(move);
            Vector3 velocity = moveDirection * currentSpeed;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
            animator.SetBool("walking", true);
        }
        else
        {
            Vector3 stopped = new Vector3(0, rb.velocity.y, 0);
            rb.velocity = stopped;
            animator.SetBool("walking", false);
        }

        UpdateAnimator();
        animator.SetBool("run", isRunning);
    }

    private void UpdateAnimator()
    {
        ResetAnimatorDirections();
        animator.SetFloat("x", movementInput.x);
        animator.SetFloat("y", movementInput.y);
    }

    private void ResetAnimatorDirections()
    {
        animator.SetFloat("x", 0f);
        animator.SetFloat("y", 0f);
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("jump");
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.performed;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        animator.SetLayerWeight(1, context.performed ? 1 : 0);
    }

    public void OnEmote(InputAction.CallbackContext context)
    {
        animator.SetTrigger("dance");
        StartCoroutine(WaitForDance());
    }

    public IEnumerator WaitForDance()
    {
        yield return new WaitForSeconds(15.2f);
        weapon?.SetActive(true);
    }
}
