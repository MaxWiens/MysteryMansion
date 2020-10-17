using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster : LivingThing
{
    void Start()
    {
        NoiseHeard += Monster_OnNoiseHeard;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.Warp(transform.position);
        currentAction = Actions.Idle;
        ChooseAction();
    }

    public enum Actions { Move, Idle, Chase, InvestigateSound }
    protected Actions currentAction;
    private IEnumerator currentActionCooroutine;
    private NavMeshAgent navMeshAgent;

    const float WalkSpeed = 3f;
    const float ChaseSpeed = 3.5f;

    protected void ChooseAction()
    {
        if (currentActionCooroutine != null)
            StopCoroutine(currentActionCooroutine);
        switch (currentAction)
        {
            case Actions.Idle:
                {
                    float r = Random.Range(0f, 1f);
                    if (r < .7f)
                    {
                        currentActionCooroutine = Move();
                        StartCoroutine(currentActionCooroutine);
                    }
                    else
                    {
                        currentActionCooroutine = Idle();
                        StartCoroutine(currentActionCooroutine);
                    }
                    break;
                }
            default:
                {
                    float r = Random.Range(0f, 1f);
                    if (r < .6f)
                    {
                        currentActionCooroutine = Move();
                        StartCoroutine(currentActionCooroutine);
                    }
                    else
                    {
                        currentActionCooroutine = Idle();
                        StartCoroutine(currentActionCooroutine);
                    }
                    break;
                }
        }
    }

    protected IEnumerator Move()
    {
        currentAction = Actions.Move;
        navMeshAgent.speed = WalkSpeed;
        /*yield return new WaitUntil(() =>
        {
            // Check proximity to destination
            return true;
        });*/
        Vector3 movementDirection = Random.onUnitSphere;
        navMeshAgent.SetDestination(transform.position + movementDirection * 4);
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < 5; i++)
        {
            movementDirection += Random.onUnitSphere / Random.Range(3f, 4f);
            movementDirection = movementDirection.normalized;
            navMeshAgent.SetDestination(transform.position + movementDirection * 4);
            yield return new WaitForSeconds(.5f);
        }
        /*navMeshAgent.SetDestination(debugTarget.position);
        yield return new WaitForSeconds(4f);*/
        ChooseAction();
    }

    protected IEnumerator Idle()
    {
        currentAction = Actions.Idle;
        navMeshAgent.speed = 0f;
        yield return new WaitForSeconds(4f);
        ChooseAction();
    }

    protected IEnumerator InvestigateNoise(Vector3 position)
    {
        currentAction = Actions.InvestigateSound;
        navMeshAgent.speed = WalkSpeed;
        navMeshAgent.SetDestination(position);
        yield return new WaitUntil(() =>
        {
            return Vector3.Distance(transform.position, position) < 1.5f;
        });
        navMeshAgent.speed = 0;
        yield return new WaitForSeconds(2f);
        ChooseAction();
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, currentAction.ToString());
    }

    private void Monster_OnNoiseHeard(LivingThing sender, NoiseEventArgs a)
    {
        Debug.Log($"{this} heard a noise with a volume of {a.Volume}");
        // Hyperbolic equation thing
        if (currentAction != Actions.Chase && Random.Range(0f, 1f) < (1 - (1 / (1 + a.Volume * 0.1f))))
        {
            if (currentActionCooroutine != null)
                StopCoroutine(currentActionCooroutine);
            Vector3 target = sender.transform.position;
            currentActionCooroutine = InvestigateNoise(new Vector3(target.x, target.y, target.z));
            StartCoroutine(currentActionCooroutine);
        }
    }
}
