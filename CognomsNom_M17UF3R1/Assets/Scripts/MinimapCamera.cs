using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public GameObject player;

    public void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 33, player.transform.position.z);
    }
}
