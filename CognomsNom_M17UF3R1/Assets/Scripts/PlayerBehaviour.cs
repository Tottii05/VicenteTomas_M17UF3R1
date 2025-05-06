using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour, CharacterActions.IMovementActions, CharacterActions.IExtraActions, IDamageable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    public float health = 100f;
    [SerializeField] private float crouchTransitionSpeed = 2f;

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
    private Coroutine crouchCoroutine;

    public static event Action PlayerDead = delegate { };

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

        // Activar isJumping cuando el jugador salta o cae (velocidad vertical negativa y no en el suelo)
        if (!isGrounded && rb.velocity.y < 0 && !isJumping)
        {
            isJumping = true;
            animator.SetBool("isJumping", true);
        }
        // Desactivar isJumping y disparar land solo cuando toca el suelo después de saltar o caer
        else if (isGrounded && isJumping)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
            animator.SetTrigger("land");
        }

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
            isJumping = true;
            animator.SetBool("isJumping", true);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.performed;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (crouchCoroutine != null) StopCoroutine(crouchCoroutine);
            crouchCoroutine = StartCoroutine(CrouchCoroutine(true));
        }
        else if (context.canceled)
        {
            if (crouchCoroutine != null) StopCoroutine(crouchCoroutine);
            crouchCoroutine = StartCoroutine(CrouchCoroutine(false));
        }
    }

    public void OnEmote(InputAction.CallbackContext context)
    {
        GetComponentInChildren<ShootingBehaviour>().actualGun.SetActive(false);
        animator.SetTrigger("dance");
        StartCoroutine(WaitForDance());
    }

    public IEnumerator WaitForDance()
    {
        yield return new WaitForSeconds(15.2f);
        weapon?.SetActive(true);
    }

    public void ResetPosition()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        movementInput = Vector2.zero;
        isRunning = false;
        isJumping = false;
        animator.SetBool("walking", false);
        animator.SetBool("run", false);
        animator.SetFloat("x", 0f);
        animator.SetFloat("y", 0f);
        animator.ResetTrigger("jump");
        animator.SetBool("isJumping", false);
        animator.ResetTrigger("land");

        Debug.Log("PlayerBehaviour: ResetPosition - Movement and animations reset");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("die");
        StartCoroutine(DieCoroutine());
    }

    public IEnumerator DieCoroutine()
    {
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(2f);
        transform.position = GameObject.Find("Spawn").transform.position;
        animator.SetTrigger("respawn");
        rb.isKinematic = false;
        GetComponent<Collider>().enabled = true;
        health = 100f;
        PlayerDead?.Invoke();
    }

    public IEnumerator CrouchCoroutine(bool isCrouching)
    {
        float targetWeight = isCrouching ? 1f : 0f;
        float currentWeight = animator.GetLayerWeight(1);

        while (Mathf.Abs(currentWeight - targetWeight) > 0.01f)
        {
            currentWeight = Mathf.MoveTowards(currentWeight, targetWeight, crouchTransitionSpeed * Time.deltaTime);
            animator.SetLayerWeight(1, currentWeight);
            yield return null;
        }

        animator.SetLayerWeight(1, targetWeight);
    }
}