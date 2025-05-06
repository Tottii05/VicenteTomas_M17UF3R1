using UnityEngine;

public class EnemyDmgSource : MonoBehaviour
{
    public EnemyController enemyController;
    public bool canDealDamage = false;
    public float damage = 25f;

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canDealDamage)
        {
            if (other.GetComponent<PlayerBehaviour>().health > 0)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                damageable.TakeDamage(damage);
                canDealDamage = false;
            }
        }
    }
}