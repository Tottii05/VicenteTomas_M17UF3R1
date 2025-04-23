using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleShootGun : AGun
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
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Push(bullet);
        }
    }

    private void Update()
    {
    }

    public override void Shoot()
    {
        if (currentAmmo > 0 && bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Pop();
            bullet.SetActive(true);
            currentAmmo--;
            StartCoroutine(ReturnToPool(bullet));
        }
        else
        {
            Debug.Log("No ammo or bullet pool is empty");
        }
    }

    private IEnumerator ReturnToPool(GameObject bullet)
    {
        yield return new WaitForSeconds(5f);
        if (bullet.activeSelf)
        {
            bullet.SetActive(false);
            bulletPool.Push(bullet);
        }
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Push(bullet);
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
            currentAmmo = magazineSize;
            maxAmmo -= ammoToDiscount;
        }
    }
}