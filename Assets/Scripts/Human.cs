using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Human : LivingThing
{
    // Start is called before the first frame update
    void Start()
    {
        Terror = 0;
        PanicThreshhold = Random.Range(15, 20);
        investigatedInteractibles = new List<Interactible>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.Warp(transform.position);
        currentAction = Actions.Idle;
        ChooseAction();
    }

    // Update is called once per frame
    void Update()
    {
        Terror = Mathf.Max(0f, Terror - TerrorDrain);
        if (currentAction != Actions.Panic && Terror > PanicThreshhold && Random.Range(0f, 1f) > .9f)
        {
            StopCoroutine(currentActionCooroutine);
            currentActionCooroutine = Panic();
            StartCoroutine(currentActionCooroutine);
        }
    }

    public float Terror { get; protected set; }
    public float TerrorDrain { get; protected set; }
    public float PanicThreshhold { get; protected set; }

    public enum Actions { Investigate, Move, Panic, Idle }
    protected Actions currentAction;
    private IEnumerator currentActionCooroutine;
    private NavMeshAgent navMeshAgent;
    public Transform debugTarget;
    private List<Interactible> investigatedInteractibles;

    const float WalkSpeed = 4f;
    const float PanicSpeed = 4.7f;
    const float InteractDistance = 1.5f;
    const float FindInteractableDistance = 15f;

    protected void ChooseAction()
    {
        if (currentActionCooroutine != null)
            StopCoroutine(currentActionCooroutine);
        switch (currentAction)
        {
            case Actions.Panic:
                {
                    float r = Random.Range(0f, 1f);
                    if (r < .8f)
                    {
                        currentActionCooroutine = Move();
                        StartCoroutine(currentActionCooroutine);
                    }
                    else
                    {
                        currentActionCooroutine = Investigate();
                        StartCoroutine(currentActionCooroutine);
                    }
                    break;
                }
            case Actions.Idle:
                {
                    float r = Random.Range(0f, 1f);
                    if (r < .475f)
                    {
                        currentActionCooroutine = Move();
                        StartCoroutine(currentActionCooroutine);
                    }
                    else if (r < .95f)
                    {
                        currentActionCooroutine = Investigate();
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
                    if (r < .4f)
                    {
                        currentActionCooroutine = Move();
                        StartCoroutine(currentActionCooroutine);
                    }
                    else if (r < .8f)
                    {
                        currentActionCooroutine = Investigate();
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

    protected virtual void OnMonsterNoticed(object monster)
    {
        Panic();
    }

    protected IEnumerator Panic()
    {
        currentAction = Actions.Panic;
        navMeshAgent.speed = PanicSpeed;
        // Find somewhere to run away
        yield return new WaitForSeconds(4f);
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

    protected IEnumerator Investigate()
    {
        currentAction = Actions.Investigate;
        navMeshAgent.speed = WalkSpeed;

        Collider[] colliders = Physics.OverlapSphere(transform.position, FindInteractableDistance, 1 << LayerMask.NameToLayer("Interactible"));
        List<Collider> colList = new List<Collider>(colliders);
        bool success = false;
        Interactible interactible = null;
        while (colList.Count > 0)
        {
            int index = Random.Range(0, colList.Count);
            Collider col = colList[index];

            interactible = col.GetComponent<Interactible>();
            if (interactible == null)
            {
                colList.RemoveAt(index);
                continue;
            }

            if (investigatedInteractibles.Contains(interactible))
            {
                colList.RemoveAt(index);
                continue;
            }

            if (Physics.Raycast(transform.position, col.transform.position))
            {
                colList.RemoveAt(index);
                continue;
            }

            success = true;
            break;
        }

        if (!success)
        {
            yield return new WaitForSeconds(.5f);
        }
        else
        {
            investigatedInteractibles.Add(interactible);
            navMeshAgent.SetDestination(interactible.transform.position);
            yield return new WaitUntil(() =>
            {
                return Vector3.Distance(transform.position, interactible.transform.position) < InteractDistance;
            });
            navMeshAgent.speed = 0f;
            MakeNoise(60);
            yield return new WaitForSeconds(4f);
            GameObject result = interactible.FinishSearch();
        }
        ChooseAction();
    }

    protected IEnumerator Idle()
    {
        currentAction = Actions.Idle;
        navMeshAgent.speed = 0f;
        yield return new WaitForSeconds(4f);
        ChooseAction();
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, currentAction.ToString());
        Gizmos.DrawWireSphere(transform.position, FindInteractableDistance);
    }
}
