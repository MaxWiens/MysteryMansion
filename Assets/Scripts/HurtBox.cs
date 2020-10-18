using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public const int maxCollidersStored = 5;
    private Collider[] colliders;
    private float[] collisionTimes;
    private LivingThing owner;

    private void Start()
    {
        colliders = new Collider[maxCollidersStored];
        collisionTimes = new float[maxCollidersStored];
        owner = GetComponentInParent<LivingThing>();
        if (!owner)
        {
            Debug.LogError("Hurt box has no living thing parent");
            Destroy(this);
        }
    }

    private void Awake()
    {
        if (colliders != null)
        {
            for (int i = 0; i < maxCollidersStored; i++)
            {
                colliders[i] = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < maxCollidersStored; i++)
        {
            if (colliders[i] == null)
            {
                colliders[i] = other;
                collisionTimes[i] = 0f;
                break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < maxCollidersStored; i++)
        {
            if (colliders[i] == other)
            {
                collisionTimes[i] += Time.fixedDeltaTime;

                if (collisionTimes[i] >= 0.25f && owner.HurtBoxTrigger(other))
                    collisionTimes[i] = 0;
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < maxCollidersStored; i++)
        {
            if (colliders[i] == null)
            {
                colliders[i] = other;
                break;
            }
        }
    }
}
