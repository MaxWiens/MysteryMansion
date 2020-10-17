using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderScript : MonoBehaviour
{
    public const int maxCollidersStored = 5;

    public bool Triggered { get; private set; }
    private Collider[] _colliders;
    public Collider this[int i] => _colliders[i];
    public int CollidersHit { get; private set; }

    private void Start()
    {
        _colliders = new Collider[maxCollidersStored];
    }

    private void FixedUpdate()
    {
        CollidersHit = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        Triggered = true;
        if (CollidersHit < maxCollidersStored)
        {
            _colliders[CollidersHit] = other;
            CollidersHit++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Triggered = false;
    }
}
