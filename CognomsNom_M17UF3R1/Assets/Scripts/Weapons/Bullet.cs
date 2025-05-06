using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public float damage = 10f;
    private AGun gun;
    private float timer;

    public void Initialize(AGun shootingGun, GameObject firePoint)
    {
        this.gun = shootingGun;
        if (firePoint != null)
        {
            transform.position = firePoint.transform.position;
            transform.rotation = firePoint.transform.rotation;
        }
        timer = 0f;
    }

    private void Update()
    {
        if (gun == null)
        {
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
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            gun.ReturnBulletToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        timer = 0f;
    }
} 