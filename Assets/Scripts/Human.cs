using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : LivingThing
{
    // Start is called before the first frame update
    void Start()
    {
        Terror = 0;
        PanicThreshhold = Random.Range(15, 20);
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentAction = Actions.Idle;
        ChooseAction();
    }

    // Update is called once per frame
    void Update()
    {
        Terror = Mathf.Max(0f, Terror - TerrorDrain);
        if (Terror > PanicThreshhold && Random.Range(0f, 1f) > .9f)
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

    protected void ChooseAction()
    {
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
        navMeshAgent.speed = 5f;
        // Find somewhere to run away
        yield return new WaitForSeconds(4f);
    }

    protected IEnumerator Move()
    {
        currentAction = Actions.Move;
        navMeshAgent.speed = 4f;
        /*yield return new WaitUntil(() =>
        {
            // Check proximity to destination
            return true;
        });*/
        Vector3 movementDirection = Random.onUnitSphere;
        navMeshAgent.Move(movementDirection);
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < 5; i++)
        {
            movementDirection += Random.onUnitSphere / Random.Range(.2f, .3f);
            navMeshAgent.Move(movementDirection);
            yield return new WaitForSeconds(.5f);
        }
        ChooseAction();
    }

    protected IEnumerator Investigate()
    {
        currentAction = Actions.Investigate;
        navMeshAgent.speed = 0f;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, LayerMask.NameToLayer("Interactible"));
        List<Collider> colList = new List<Collider>(colliders);
        bool success = false;
        Interactible interactible = null;
        while (colList.Count > 0)
        {
            int index = Random.Range(0, colList.Count);
            if (Physics.Raycast(transform.position, colList[index].transform.position))
            {
                colList.RemoveAt(index);
                continue;
            }

            interactible = colList[index].GetComponent<Interactible>();
            if (interactible == null)
            {
                colList.RemoveAt(index);
                continue;
            }

            // TODO: add a random component?
            success = true;
            break;
        }

        if (!success)
        {
            yield return new WaitForSeconds(.5f);
        }
        else
        {
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
}
