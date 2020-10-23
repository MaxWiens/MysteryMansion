using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster : LivingThing
{
    public enum Actions { Move, Idle, Chase, InvestigateSound }

    protected Actions currentAction;
    private float attackCooldown;

    public TriggerColliderScript farColl;
    public TriggerColliderScript medColl;
    public TriggerColliderScript nearColl;
    public Transform debugTarget;

    const float WalkSpeed = 1.5f;
    const float ChaseSpeed = 1.75f;

    private static int obstacleBitmask;

    protected override void Start()
    {
        base.Start();

        obstacleBitmask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Interactible") | 1 << LayerMask.NameToLayer("Haunt");

        Health = 20;
        NoiseHeard += Monster_OnNoiseHeard;
        NavMeshAgent.Warp(transform.position);
        currentAction = Actions.Idle;
        ChooseAction();
    }

    protected override void Update()
    {
        base.Update();
        bool done = false;
        if (nearColl.CollidersHit > 0)
        {
            for (int i = 0; i < nearColl.CollidersHit; i++)
            {
                if (nearColl[i] != null)
                {
                    done = true;
                    ChangeActionCoroutine(Chase(nearColl[0].transform.parent.position));
                    break;
                }
            }
        }
        if (!done && medColl.CollidersHit > 0)
        {
            for (int i = 0; i < medColl.CollidersHit; i++)
            {
                if (medColl[i] != null)
                {
                    done = true;
                    ChangeActionCoroutine(Chase(medColl[0].transform.parent.position));
                    break;
                }
            }
        }
        if (!done && farColl.CollidersHit > 0)
        {
            for (int i = 0; i < medColl.CollidersHit; i++)
            {
                if (medColl[i] != null)
                {
                    done = true;
                    ChangeActionCoroutine(Chase(farColl[0].transform.parent.position));
                    break;
                }
            }
        }

        attackCooldown = Mathf.Max(0f, attackCooldown - Time.deltaTime);
    }

    protected override void ChooseAction()
    {
        /*changeActionCoroutine(Move());*/
        switch (currentAction)
        {
            case Actions.Idle:
                {
                    float r = Random.Range(0f, 1f);
                    if (r < .7f)
                    {
                        ChangeActionCoroutine(Move());
                    }
                    else
                    {
                        ChangeActionCoroutine(Idle());
                    }
                    break;
                }
            default:
                {
                    float r = Random.Range(0f, 1f);
                    if (r < .6f)
                    {
                        ChangeActionCoroutine(Move());
                    }
                    else
                    {
                        ChangeActionCoroutine(Idle());
                    }
                    break;
                }
        }
    }

    protected IEnumerator Move()
    {
        currentAction = Actions.Move;
        NavMeshAgent.speed = WalkSpeed;
        Vector3 dir = Random.onUnitSphere;
        NavMeshAgent.SetDestination(transform.position + dir * 4);
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(transform.position, transform.forward, 2f, obstacleBitmask))
            {
                dir = -transform.forward;
                for (int n = 0; n < 20; n++)
                {
                    float angle = Random.Range(35f, 145f);
                    if (Random.value < 0.5f)
                        angle = -angle;
                    if (!Physics.Raycast(transform.position, Quaternion.Euler(0, angle, 0) * transform.forward, 4f, obstacleBitmask))
                    {
                        dir = Quaternion.Euler(0, angle, 0) * transform.forward;
                        break;
                    }
                }
                //Debug.Log($"Walking monster turned from {transform.forward} to {dir}");
            }
            else
            {
                dir = Quaternion.Euler(0, Random.Range(-25f, 25f), 0) * transform.forward;
            }
            NavMeshAgent.SetDestination(transform.position + dir * 4);
            yield return new WaitForSeconds(.5f);
        }
        /*NavMeshAgent.SetDestination(debugTarget.position);
        yield return new WaitForSeconds(4f);*/
        ChooseAction();
    }

    protected IEnumerator Idle()
    {
        currentAction = Actions.Idle;
        NavMeshAgent.speed = 0f;
        yield return new WaitForSeconds(4f);
        ChooseAction();
    }

    protected IEnumerator InvestigateNoise(Vector3 position)
    {
        currentAction = Actions.InvestigateSound;
        NavMeshAgent.speed = WalkSpeed;
        NavMeshAgent.SetDestination(position);

        float t1 = Time.time;
        float distance = Vector3.Distance(transform.position, position);
        yield return WaitUntilNearbyWithTimeout(position, 1.5f, distance * 1.5f);
        Debug.Log($"Monster took {Time.time - t1} seconds to investigate sound {distance} distance away");

        NavMeshAgent.speed = 0;

        yield return new WaitForSeconds(2f);

        ChooseAction();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.up, currentAction.ToString());
#endif
    }

    private void Monster_OnNoiseHeard(LivingThing sender, NoiseEventArgs a)
    {
        // Hyperbolic equation thing
        float investigateChance = (1 - (1 / (1 + a.Volume * 0.3f)));
        //Debug.Log($"{this} heard a noise with a volume of {a.Volume}, has {investigateChance} chance");
        if (currentAction != Actions.Chase && Random.value < investigateChance)
        {
            ChangeActionCoroutine(InvestigateNoise(sender.transform.position));
        }
    }

    protected IEnumerator Chase(Vector3 location)
    {
        currentAction = Actions.Chase;
        NavMeshAgent.speed = ChaseSpeed;
        NavMeshAgent.SetDestination(location);

        yield return WaitUntilNearbyWithTimeout(location, 0.5f, 10f);

        ChooseAction();
    }

    public override bool HurtBoxTrigger(Collider thing)
    {
        if (attackCooldown == 0)
        {
            Human other = thing.GetComponentInParent<Human>();
            if (other != null)
            {
                other.TakeDamage(2);
                MakeNoise(50, SoundSource.Monster);
                attackCooldown = 1;
                return true;
            }
        }

        return false;
    }

    public override void HandleDoorBlocked()
    {
        transform.forward = Quaternion.Euler(Random.Range(90, 270), 0, 0) * transform.forward;
        ChooseAction();
    }
}
