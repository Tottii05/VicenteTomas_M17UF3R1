using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float fov = 120f;

    public bool CheckPlayerInVision(GameObject player)
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < fov / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.gameObject == player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Vector3 GetLastPlayerPosition(GameObject player)
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < fov / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.gameObject == player)
                {
                    return player.transform.position;
                }
            }
        }

        return Vector3.zero;
    }
}