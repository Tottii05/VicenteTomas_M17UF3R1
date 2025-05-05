using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurstShotGun : AGun
{
    public override float maxAmmo { get; set; } = 45f;
    public override float currentAmmo { get; set; } = 15f;
    public override float magazineSize { get; set; } = 15f;
    public GameObject firePoint;
    public GameObject bulletPrefab;
    private Stack<GameObject> bulletPool = new Stack<GameObject>();
    private int poolSize = 15;

    private void Start()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("bulletPrefab no está asignado en el Inspector. No se puede inicializar la pool de balas.");
            return;
        }

        InitializeBulletPool();
        Debug.Log($"Bullet pool inicializada para {gameObject.name} con {bulletPool.Count} balas");
        Debug.Log($"Munición inicial: {currentAmmo}/{magazineSize}, Max ammo: {maxAmmo}");
    }

    private void InitializeBulletPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            if (bullet != null)
            {
                bullet.SetActive(false);
                bulletPool.Push(bullet);
            }
            else
            {
                Debug.LogError($"Fallo al instanciar la bala {i + 1} para {gameObject.name}. Asegúrate de que bulletPrefab sea válido.");
            }
        }
    }

    private void Update()
    {
    }

    public override void Shoot()
    {
        Debug.Log($"Intentando disparar con {gameObject.name}. Munición actual: {currentAmmo}, Balas en pool: {bulletPool.Count}");
        if (currentAmmo >= 3 && bulletPool.Count >= 3)
        {
            StartCoroutine(BurstFire());
        }
        else
        {
            Debug.Log($"No hay suficiente munición o balas en la pool para disparar en ráfaga con {gameObject.name}");
            if (currentAmmo < 3)
            {
                Debug.Log($"No hay suficiente munición: {currentAmmo}/3 requeridas");
            }
            if (bulletPool.Count < 3)
            {
                Debug.Log($"No hay suficientes balas en la pool: {bulletPool.Count}/3 requeridas");
            }
        }
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < 3; i++)
        {
            if (bulletPool.Count > 0)
            {
                GameObject bullet = bulletPool.Pop();
                bullet.SetActive(true);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null && firePoint != null)
                {
                    bulletScript.Initialize(this, firePoint);
                }
                else
                {
                    Debug.LogError("No se pudo inicializar la bala. Verifica Bullet script o firePoint.");
                }
                currentAmmo--;
                StartCoroutine(ReturnToPool(bullet));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator ReturnToPool(GameObject bullet)
    {
        yield return new WaitForSeconds(5f);
        if (bullet != null && bullet.activeSelf)
        {
            bullet.SetActive(false);
            bulletPool.Push(bullet);
            Debug.Log($"Bala devuelta a la pool de {gameObject.name}. Balas en pool: {bulletPool.Count}");
        }
    }

    public override void ReturnBulletToPool(GameObject bullet)
    {
        if (bullet != null)
        {
            bullet.SetActive(false);
            bulletPool.Push(bullet);
            Debug.Log($"Bala devuelta manualmente a la pool de {gameObject.name}. Balas en pool: {bulletPool.Count}");
        }
    }

    public override void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    public IEnumerator ReloadCoroutine()
    {
        if (maxAmmo > 0)
        {
            yield return new WaitForSeconds(2.6f);
            float ammoToDiscount = magazineSize - currentAmmo;
            int bulletsToAdd = (int)ammoToDiscount;
            currentAmmo = magazineSize;
            maxAmmo -= ammoToDiscount;

            for (int i = 0; i < bulletsToAdd && bulletPool.Count < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                if (bullet != null)
                {
                    bullet.SetActive(false);
                    bulletPool.Push(bullet);
                }
            }

            Debug.Log($"Recargado. Munición actual: {currentAmmo}/{magazineSize}, Max ammo: {maxAmmo}, Balas en pool: {bulletPool.Count}");
        }
        else
        {
            Debug.Log("No hay suficiente maxAmmo para recargar");
        }
    }
}