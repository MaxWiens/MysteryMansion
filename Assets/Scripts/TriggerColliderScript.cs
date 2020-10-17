using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderScript : MonoBehaviour
{
    public bool Triggered { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        Triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Triggered = false;
    }
}
