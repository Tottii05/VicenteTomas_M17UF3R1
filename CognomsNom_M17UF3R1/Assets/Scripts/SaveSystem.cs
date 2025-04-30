using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveSystem : MonoBehaviour, CharacterActions.IInteractionsActions
{
    public GameObject text;
    private CharacterActions inputActions;
    private bool isPlayerInTrigger = false;
    public GameObject player;
    public ShootingBehaviour shootingBehaviour;

    private void Awake()
    {
        inputActions = new CharacterActions();
        inputActions.Interactions.SetCallbacks(this);
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
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (shootingBehaviour == null && player != null)
        {
            shootingBehaviour = player.GetComponentInChildren<ShootingBehaviour>();
        }

        if (PlayerPrefs.HasKey("PlayerPositionX"))
        {
            LoadGame();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isPlayerInTrigger)
            {
                shootingBehaviour = player.GetComponentInChildren<ShootingBehaviour>();
                SaveGame();
            }
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            player = collision.gameObject;
            if (text != null)
            {
                text.SetActive(true);
            }
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            player = null;
            if (text != null)
            {
                text.SetActive(false);
            }
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetFloat("PlayerPositionX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPositionY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPositionZ", player.transform.position.z);

        PlayerPrefs.SetFloat("PlayerRotationX", player.transform.eulerAngles.x);
        PlayerPrefs.SetFloat("PlayerRotationY", player.transform.eulerAngles.y);
        PlayerPrefs.SetFloat("PlayerRotationZ", player.transform.eulerAngles.z);

        int weaponIndex = GetCurrentWeaponIndex();
        PlayerPrefs.SetInt("CurrentWeaponIndex", weaponIndex);

        // Save all weapon names and their corresponding prefabs
        for (int i = 0; i < shootingBehaviour.weapons.Length; i++)
        {
            string weaponKey = $"Weapon{i}";
            PlayerPrefs.SetString(weaponKey, shootingBehaviour.weapons[i] != null ? shootingBehaviour.weapons[i].name : "");
        }

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        player.transform.position = new Vector3(
            PlayerPrefs.GetFloat("PlayerPositionX"),
            PlayerPrefs.GetFloat("PlayerPositionY"),
            PlayerPrefs.GetFloat("PlayerPositionZ")
        );

        player.transform.rotation = Quaternion.Euler(
            PlayerPrefs.GetFloat("PlayerRotationX"),
            PlayerPrefs.GetFloat("PlayerRotationY"),
            PlayerPrefs.GetFloat("PlayerRotationZ")
        );

        // Load weapons based on the saved names
        int weaponCount = 0;
        while (PlayerPrefs.HasKey($"Weapon{weaponCount}"))
        {
            weaponCount++;
        }

        GameObject[] loadedWeapons = new GameObject[weaponCount];
        for (int i = 0; i < weaponCount; i++)
        {
            string savedName = PlayerPrefs.GetString($"Weapon{i}", "");
            GameObject weaponPrefab = Resources.Load<GameObject>($"Weapons/{savedName}");
            if (weaponPrefab != null)
            {
                GameObject weaponInstance = Instantiate(weaponPrefab, shootingBehaviour.transform);
                weaponInstance.SetActive(false);
                loadedWeapons[i] = weaponInstance;
            }
        }

        // Clear old weapons and assign the loaded ones
        foreach (var oldWeapon in shootingBehaviour.weapons)
        {
            if (oldWeapon != null) Destroy(oldWeapon);
        }

        shootingBehaviour.weapons = loadedWeapons;

        // Set the actual gun based on the saved index
        int currentWeaponIndex = PlayerPrefs.GetInt("CurrentWeaponIndex", 0);
        if (currentWeaponIndex >= 0 && currentWeaponIndex < shootingBehaviour.weapons.Length)
        {
            shootingBehaviour.actualGun = shootingBehaviour.weapons[currentWeaponIndex];
            shootingBehaviour.actualGun.SetActive(true);
            shootingBehaviour.gunScript = shootingBehaviour.actualGun.GetComponent<AGun>();
            shootingBehaviour.UpdateGunScript();
        }
    }

    private int GetCurrentWeaponIndex()
    {
        for (int i = 0; i < shootingBehaviour.weapons.Length; i++)
        {
            if (shootingBehaviour.weapons[i] == shootingBehaviour.actualGun)
            {
                return i;
            }
        }
        return 0;
    }
}
