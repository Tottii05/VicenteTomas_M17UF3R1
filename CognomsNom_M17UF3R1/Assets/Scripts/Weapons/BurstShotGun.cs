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
            return;
        }

        InitializeBulletPool();
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
        }
    }

    private void Update()
    {
    }

    public override void Shoot()
    {
        if (currentAmmo >= 3 && bulletPool.Count >= 3)
        {
            StartCoroutine(BurstFire());
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
        }
    }

    public override void ReturnBulletToPool(GameObject bullet)
    {
        if (bullet != null)
        {
            bullet.SetActive(false);
            bulletPool.Push(bullet);
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
        }
    }
}