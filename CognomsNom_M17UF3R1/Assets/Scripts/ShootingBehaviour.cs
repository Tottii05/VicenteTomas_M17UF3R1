using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingBehaviour : MonoBehaviour, CharacterActions.IAttackActions
{
    private FirstPersonCamera firstPersonCamera;
    private ThirdPersonCamera thirdPersonCamera;
    public bool isAiming = false;
    public bool isShooting = false;
    private CharacterActions inputActions;
    private Animator animator;
    public GameObject actualGun;
    private AGun gunScript;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI maxAmmoText;

    private void Awake()
    {
        inputActions = new CharacterActions();
        inputActions.Attack.SetCallbacks(this);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        gunScript = actualGun.GetComponent<AGun>();
        animator = GetComponentInChildren<Animator>();
        firstPersonCamera = FindObjectOfType<FirstPersonCamera>();
        thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();
        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(true);
        }
        if (firstPersonCamera != null)
        {
            firstPersonCamera.SetActive(false);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isShooting = true;
            if (gunScript != null)
            {
                gunScript.Shoot();
            }
            if (animator != null)
            {
                animator.SetBool("shoot", isShooting);
            }
        }
        else if (context.canceled)
        {
            isShooting = false;
            if (animator != null)
            {
                animator.SetBool("shoot", isShooting);
            }
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        isAiming = context.performed;
        animator.SetBool("aim", isAiming);
        firstPersonCamera.SetActive(isAiming);
        thirdPersonCamera.gameObject.SetActive(!isAiming);
        PlayerBehaviour playerBehaviour = GetComponent<PlayerBehaviour>();
        if (playerBehaviour != null)
        {
            playerBehaviour.SetAiming(isAiming);
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (gunScript.currentAmmo != gunScript.magazineSize)
            {
                gunScript.Reload();
                animator.SetTrigger("reload");
            }
        }
    }

    public void Update()
    {
        if (gunScript != null)
        {
            ammoText.text = gunScript.currentAmmo.ToString();
            maxAmmoText.text = gunScript.maxAmmo.ToString();
        }
    }
}