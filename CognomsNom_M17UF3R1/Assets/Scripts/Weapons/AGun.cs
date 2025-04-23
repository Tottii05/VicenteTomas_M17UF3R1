using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGun : MonoBehaviour
{
    public abstract float maxAmmo { get; set; }
    public abstract float currentAmmo { get; set; }
    public abstract float magazineSize { get; set; }
    public abstract void Shoot();
    public abstract void Reload();
}
