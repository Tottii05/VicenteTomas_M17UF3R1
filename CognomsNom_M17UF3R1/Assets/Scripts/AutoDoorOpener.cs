using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoorOpener : MonoBehaviour
{
    public Animator doorAnimator;
    public void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetBool("character_nearby", true);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetBool("character_nearby", false);
        }
    }
}

