using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;
    public float damage = 10f;
    private SingleShootGun gun;

    private void Awake()
    {
        gun = FindObjectOfType<SingleShootGun>();
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private void OnEnable()
    {
        transform.position = gun.firePoint.transform.position;
        transform.rotation = gun.firePoint.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit: " + other.name);
        gun.ReturnBulletToPool(gameObject);
    }
}