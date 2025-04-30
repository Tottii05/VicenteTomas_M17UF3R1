using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingBehaviour : MonoBehaviour, CharacterActions.IAttackActions, CharacterActions.IInventoryActions
{
    private FirstPersonCamera firstPersonCamera;
    private ThirdPersonCamera thirdPersonCamera;
    public bool isAiming = false;
    public bool isShooting = false;
    private CharacterActions inputActions;
    private Animator animator;
    public GameObject actualGun;
    public GameObject[] weapons;
    public AGun gunScript;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI maxAmmoText;

    private void Awake()
    {
        inputActions = new CharacterActions();
        inputActions.Attack.SetCallbacks(this);
        inputActions.Inventory.SetCallbacks(this);
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
        animator = GetComponentInChildren<Animator>();
        InitializeCameras();
        InitializeGun();
    }

    private void InitializeCameras()
    {
        Transform camerasTransform = transform.parent.Find("Cameras");
        if (camerasTransform != null)
        {
            firstPersonCamera = camerasTransform.GetComponentInChildren<FirstPersonCamera>();
            thirdPersonCamera = camerasTransform.GetComponentInChildren<ThirdPersonCamera>();
            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.gameObject.SetActive(true);
            }
            if (firstPersonCamera != null)
            {
                firstPersonCamera.SetActive(false);
            }
        }
    }

    private void InitializeGun()
    {
        if (actualGun != null)
        {
            gunScript = actualGun.GetComponent<AGun>();
            UpdateGunScript();
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

    public void OnWeapon1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchWeapon(0);
        }
    }

    public void OnWeapon2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchWeapon(1);
        }
    }

    public void OnWeapon3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchWeapon(2);
        }
    }

    private void SwitchWeapon(int v)
    {
        if (weapons.Length > v)
        {
            actualGun.SetActive(false);
            actualGun = weapons[v];
            actualGun.SetActive(true);
            gunScript = actualGun.GetComponent<AGun>();
            ammoText.text = gunScript.currentAmmo.ToString();
            maxAmmoText.text = gunScript.maxAmmo.ToString();
        }
    }

    public void UpdateGunScript()
    {
        if (actualGun != null)
        {
            gunScript = actualGun.GetComponent<AGun>();
            if (gunScript != null)
            {
                ammoText.text = gunScript.currentAmmo.ToString();
                maxAmmoText.text = gunScript.maxAmmo.ToString();
            }
        }
    }
}
