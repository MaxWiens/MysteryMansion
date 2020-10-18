﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static Items;

public class Human : LivingThing
{
    public enum Actions { Investigate, Move, Panic, Idle }

    public float Terror { get; protected set; }
    public float TerrorDrain { get; protected set; }
    public float PanicThreshhold { get; protected set; }
    public Item MyItem { get; private set; }

    public Transform debugTarget;
    public TriggerColliderScript farColl;
    public TriggerColliderScript medColl;
    public TriggerColliderScript nearColl;
    public GameObject worldItemPrefab;
    public SpriteRenderer heldItemRenderer;
    public GameObject hurtbox;

    protected Actions currentAction;
    private IEnumerator currentActionCooroutine;
    private List<Interactible> investigatedInteractibles;
    private bool lastInvestigateSucceeded;
    private float attackCooldown;

    const float WalkSpeed = 4f;
    const float PanicSpeed = 4.7f;
    // keep bigger than the player's radius
    const float InteractDistance = 1f;
    const float FindInteractableDistance = 15f;
    const float MaxTerror = 30f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        MyItem = Item.None;
        Terror = 0;
        PanicThreshhold = Random.Range(20, 25);
        TerrorDrain = 0.5f;
        attackCooldown = 0f;
        lastInvestigateSucceeded = false;
        investigatedInteractibles = new List<Interactible>();
        NavMeshAgent.Warp(transform.position);
        currentAction = Actions.Idle;
        ChooseAction();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Terror = Terror - TerrorDrain * Time.deltaTime * (currentAction == Actions.Panic ? 3 : 1);


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

        Terror = Mathf.Clamp(Terror, 0f, MaxTerror);

        if (currentAction != Actions.Panic && Terror > PanicThreshhold && Random.Range(0f, 1f) > .9f)
        {
            StopCoroutine(currentActionCooroutine);
            currentActionCooroutine = Panic(transform.forward * 10);
            StartCoroutine(currentActionCooroutine);
        }

        attackCooldown = Mathf.Max(0f, attackCooldown - Time.deltaTime);
    }

    protected void ChooseAction()
    {
        if (currentActionCooroutine != null)
            StopCoroutine(currentActionCooroutine);
        /*currentActionCooroutine = Move();
        StartCoroutine(currentActionCooroutine);*/
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
        NavMeshAgent.speed = PanicSpeed;
        NavMeshAgent.SetDestination(transform.position - locationOffset);

        if (MyItem != Item.None && Random.value < .33f)
        {
            DropItem();
        }

        float timeElapsed = 0f;
        yield return null;
        while (Terror > MaxTerror / 2)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= 1f)
            {
                timeElapsed -= 1f;
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
                NavMeshAgent.SetDestination(transform.position + dir * 10);
            }
            yield return null;
        }
        ChooseAction();
    }

    protected IEnumerator Move()
    {
        currentAction = Actions.Move;
        NavMeshAgent.speed = WalkSpeed;
        /*yield return new WaitUntil(() =>
        {
            // Check proximity to destination
            return true;
        });*/
        Vector3 dir = Random.onUnitSphere;
        NavMeshAgent.SetDestination(transform.position + dir * 4);
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
            NavMeshAgent.SetDestination(transform.position + dir * 4);
            yield return new WaitForSeconds(.5f);
        }
        /*NavMeshAgent.SetDestination(debugTarget.position);
        yield return new WaitForSeconds(4f);*/
        ChooseAction();
    }

    protected IEnumerator Investigate()
    {
        currentAction = Actions.Investigate;
        NavMeshAgent.speed = WalkSpeed;

        Collider[] colliders = Physics.OverlapSphere(transform.position, FindInteractableDistance, 1 << LayerMask.NameToLayer("Interactible"));
        List<Collider> colList = new List<Collider>(colliders);
        bool success = false;
        Interactible interactible = null;
        Collider col = null;
        while (colList.Count > 0)
        {
            int index = Random.Range(0, colList.Count);
            col = colList[index];

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
            Vector3 destination = col.ClosestPointOnBounds(transform.position);
            NavMeshAgent.SetDestination(destination);
            yield return new WaitUntil(() =>
            {
                return Vector3.Distance(transform.position, destination) < InteractDistance;
            });
            NavMeshAgent.speed = 0f;
            MakeNoise(60, SoundSource.Human);
            yield return new WaitForSeconds(4f);
            if (interactible != null)
            {
                GetItem(interactible.FinishSearch(this));
                lastInvestigateSucceeded = true;
            }
        }
        ChooseAction();
    }

    protected IEnumerator Idle()
    {
        currentAction = Actions.Idle;
        NavMeshAgent.speed = 0f;
        yield return new WaitForSeconds(4f);
        ChooseAction();
    }

    public override bool HurtBoxTrigger(Collider thing)
    {
        if (attackCooldown == 0)
        {
            Monster other = thing.GetComponentInParent<Monster>();
            if (other != null)
            {
                other.TakeDamage(4);
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, currentAction.ToString());
        Gizmos.DrawWireSphere(transform.position, FindInteractableDistance);
    }

    public void DropItem()
    {
        if (MyItem == Item.None)
            return;
        RaycastHit raycast;
        Vector3 point;
        if (Physics.Raycast(transform.position, Vector3.down, out raycast, 10f, 1 << LayerMask.NameToLayer("Default")))
        {
            point = raycast.point;
        }
        else
        {
            point = transform.position;
        }
        WorldItem worldItem = Instantiate(worldItemPrefab, point, transform.rotation).GetComponent<WorldItem>();
        worldItem.Item = MyItem;
        RemoveItem();
    }

    public void GetItem(Item item)
    {
        if (item == Item.None)
            return;
        MyItem = item;
        heldItemRenderer.sprite = GetSprite(item);
        if (item == Item.Axe)
            hurtbox.SetActive(true);
    }

    public void RemoveItem()
    {
        MyItem = Item.None;
        heldItemRenderer.sprite = null;
        hurtbox.SetActive(false);
    }

    public override SpriteRenderer GetMainSpriteRenderer()
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
        {
            if (sr.CompareTag("MainSprite"))
            {
                return sr;
            }
        }

        return null;
    }

    public override void HandleDoorBlocked()
    {
        transform.forward = Quaternion.Euler(Random.Range(90, 270), 0, 0) * transform.forward;
        if (currentAction == Actions.Panic)
        {
            Terror += 5f;
            StopCoroutine(currentActionCooroutine);
            currentActionCooroutine = Panic(transform.forward * 10);
            StartCoroutine(currentActionCooroutine);
        }
        else
        {
            ChooseAction();
        }
    }
}
