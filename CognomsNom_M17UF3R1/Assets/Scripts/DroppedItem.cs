using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float bobbingSpeed = 1f;
    public float bobbingHeight = 0.5f;
    private Vector3 startPosition;
    public GameObject itemPrefab;

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShootingBehaviour shootingBehaviour = other.GetComponentInChildren<ShootingBehaviour>();
            if (shootingBehaviour != null && itemPrefab != null)
            {
                GameObject[] weapons = shootingBehaviour.weapons;
                int emptySlotIndex = -1;

                for (int i = 0; i < weapons.Length; i++)
                {
                    if (emptySlotIndex == -1 && weapons[i] == null)
                    {
                        emptySlotIndex = i;
                    }
                }

                if (emptySlotIndex >= 0)
                {
                    Transform handRight = FindDeepChild(other.transform, "Hand_Right");
                    if (handRight != null)
                    {
                        GameObject newWeapon = Instantiate(itemPrefab, handRight);
                        newWeapon.SetActive(false);
                        shootingBehaviour.weapons[emptySlotIndex] = newWeapon;
                        shootingBehaviour.UpdateGunScript();
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Debug.Log("No hay espacio en el inventario de armas.");
                }
            }
            else
            {
                Debug.LogWarning("ShootingBehaviour o itemPrefab no están asignados.");
            }
        }
    }

    Transform FindDeepChild(Transform parent, string childName)
    {
        if (parent.name == childName)
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = FindDeepChild(parent.GetChild(i), childName);
            if (result != null)
                return result;
        }
        return null;
    }
}