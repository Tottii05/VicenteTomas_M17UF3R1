using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public float damage = 10f;
    private AGun gun; // Usamos AGun como base para soportar tanto SingleShotGun como BurstShotGun
    private float timer;

    public void Initialize(AGun shootingGun, GameObject firePoint)
    {
        this.gun = shootingGun;
        if (firePoint != null)
        {
            transform.position = firePoint.transform.position;
            transform.rotation = firePoint.transform.rotation;
        }
        else
        {
            Debug.LogWarning("firePoint es null en Initialize. Usando posición actual.");
        }
        timer = 0f;
    }

    private void Update()
    {
        if (gun == null)
        {
            Debug.LogWarning("gun es null en Update. La bala será destruida.");
            Destroy(gameObject);
            return;
        }

        transform.position += transform.forward * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            gun.ReturnBulletToPool(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gun != null)
        {
            Debug.Log("Bullet hit: " + other.name);
            gun.ReturnBulletToPool(gameObject);
        }
        else
        {
            Debug.LogError("gun es null en OnTriggerEnter. No se puede devolver la bala a la pool.");
            Destroy(gameObject); // Destruir la bala como fallback
        }
    }

    private void OnDisable()
    {
        timer = 0f; // Resetear el temporizador al desactivar
    }
} 