using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Human : LivingThing
{
    public enum Actions { Investigate, Move, Panic, Idle }

    public float Terror { get; protected set; }
    public float TerrorDrain { get; protected set; }
    public float PanicThreshhold { get; protected set; }

    public Transform debugTarget;
    public TriggerColliderScript farColl;
    public TriggerColliderScript medColl;
    public TriggerColliderScript nearColl;

    protected Actions currentAction;
    private IEnumerator currentActionCooroutine;
    private NavMeshAgent navMeshAgent;
    private List<Interactible> investigatedInteractibles;
    private bool lastInvestigateSucceeded;

    const float WalkSpeed = 4f;
    const float PanicSpeed = 4.7f;
    const float InteractDistance = 1.5f;
    const float FindInteractableDistance = 15f;
    const float MaxTerror = 35f;

    // Start is called before the first frame update
    void Start()
    {
        Terror = 0;
        PanicThreshhold = Random.Range(15, 20);
        TerrorDrain = 0.5f;
        lastInvestigateSucceeded = false;
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

        if (farColl.Triggered)
        {
            Terror += 1;
        }

        if (medColl.Triggered)
        {
            Terror += 1.5f;
        }

        if (nearColl.Triggered)
        {
            Terror += 3;
        }

        Terror = Mathf.Min(Terror, MaxTerror);

        if (currentAction != Actions.Panic && Terror > PanicThreshhold && Random.Range(0f, 1f) > .9f)
        {
            StopCoroutine(currentActionCooroutine);
            currentActionCooroutine = Panic(transform.forward * 10);
            StartCoroutine(currentActionCooroutine);
        }
    }

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
            case Actions.Investigate:
                if (lastInvestigateSucceeded)
                    goto default;
                else
                {
                    float r = Random.Range(0f, 1f);
                    if (r < .55f)
                    {
                        currentActionCooroutine = Move();
                        StartCoroutine(currentActionCooroutine);
                    }
                    else if (r < .9f)
                    {
                        currentActionCooroutine = Idle();
                        StartCoroutine(currentActionCooroutine);
                    }
                    else
                    {
                        // Who ever said they were smart?
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

    protected IEnumerator Panic(Vector3 locationOffset)
    {
        currentAction = Actions.Panic;
        navMeshAgent.speed = PanicSpeed;
        navMeshAgent.SetDestination(transform.position - locationOffset);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 2; i++)
        {
            Vector3 dir;
            if (Physics.Raycast(transform.position, transform.forward, 2f, 1 << LayerMask.NameToLayer("Default")))
            {
                if (!Physics.Raycast(transform.position, -transform.forward, 4f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Monster")))
                    dir = -transform.forward;
                else if (!Physics.Raycast(transform.position, Quaternion.Euler(135, 0, 0) * transform.forward, 4f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Monster")))
                    dir = Quaternion.Euler(135, 0, 0) * transform.forward;
                else if (!Physics.Raycast(transform.position, Quaternion.Euler(-135, 0, 0) * transform.forward, 4f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Monster")))
                    dir = Quaternion.Euler(-135, 0, 0) * transform.forward;
                else if (!Physics.Raycast(transform.position, Quaternion.Euler(90, 0, 0) * transform.forward, 4f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Monster")))
                    dir = Quaternion.Euler(90, 0, 0) * transform.forward;
                else if (!Physics.Raycast(transform.position, Quaternion.Euler(-90, 0, 0) * transform.forward, 4f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Monster")))
                    dir = Quaternion.Euler(-90, 0, 0) * transform.forward;
                else
                    dir = Quaternion.Euler(Random.Range(90, 270), 0, 0) * transform.forward;
                Debug.Log($"Panicing player turned from {transform.forward} to {dir}");
            }
            else
            {
                dir = transform.forward;
            }
            navMeshAgent.SetDestination(transform.position + dir * 10);
            yield return new WaitForSeconds(1f);
        }
        ChooseAction();
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
        Vector3 dir = Random.onUnitSphere;
        navMeshAgent.SetDestination(transform.position + dir * 4);
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(transform.position, transform.forward, 2f, 1 << LayerMask.NameToLayer("Default")))
            {
                dir = -transform.forward;
                for (int n = 0; n < 20; n++)
                {
                    float angle = Random.Range(35f, 145f);
                    if (Random.value < 0.5f)
                        angle = -angle;
                    if (!Physics.Raycast(transform.position, Quaternion.Euler(angle, 0, 0) * transform.forward, 4f, 1 << LayerMask.NameToLayer("Default")))
                    {
                        dir = Quaternion.Euler(angle, 0, 0) * transform.forward;
                        break;
                    }
                }
                Debug.Log($"Walking player turned from {transform.forward} to {dir}");
            }
            else
            {
                dir = Quaternion.Euler(Random.Range(-25f, 25f), 0, 0) * transform.forward;
            }
            navMeshAgent.SetDestination(transform.position + dir * 4);
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

            if (Physics.Raycast(transform.position, col.transform.position, 1 << LayerMask.NameToLayer("Default")))
            {
                colList.RemoveAt(index);
                continue;
            }

            success = true;
            break;
        }

        if (!success)
        {
            lastInvestigateSucceeded = false;
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
            MakeNoise(60, SoundSource.Human);
            yield return new WaitForSeconds(4f);
            GameObject result = interactible.FinishSearch();
            lastInvestigateSucceeded = true;
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
