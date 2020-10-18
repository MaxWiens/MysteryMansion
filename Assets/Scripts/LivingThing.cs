using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public abstract class LivingThing : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    protected NavMeshAgent NavMeshAgent
    {
        get => _navMeshAgent;
        set
        {
            _navMeshAgent = value;
            if (_navMeshAgent != null)
                _navMeshAgent.autoTraverseOffMeshLink = false;
        }
    }

    public enum SoundSource { Human, Monster, Environment, HumanDeath }
    public class NoiseEventArgs : EventArgs
    {
        public float Volume { get; set; }
        public SoundSource Source { get; set; }
    }
    public delegate void NoiseHeardEventHandler(LivingThing sender, NoiseEventArgs args);
    protected event NoiseHeardEventHandler NoiseHeard;
    private bool manuallyTraversingOffMeshLink;
    public GameObject corpsePrefab;

    public virtual void TriggerNoiseHeard(LivingThing sender, NoiseEventArgs a)
    {
        NoiseHeard?.Invoke(sender, a);
    }

    public void MakeNoise(float volume, SoundSource source)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 60f, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Monster"));
        List<LivingThing> lts = new List<LivingThing>();
        foreach (Collider c in cols)
        {
            LivingThing lt = c.GetComponentInParent<LivingThing>();
            if (lt != null && lt != this)
                lts.Add(lt);
        }

        foreach(LivingThing lt in lts)
        {
            float distance = Vector3.Distance(transform.position, lt.transform.position);
            lt.TriggerNoiseHeard(this, new NoiseEventArgs() { Source = source, Volume = (volume * lt.NoiseSensitivity) / (distance * distance / 4) });
            //lt.TriggerNoiseHeard(this, new NoiseEventArgs() { Source = source, Volume = (volume * lt.NoiseSensitivity) / (distance / 2) });
        }
    }

    public int Health { get; protected set; }
    // Percentage from 1 to 0
    private float _noiseSensitivity = 1;
    public float NoiseSensitivity {
        get => _noiseSensitivity;
        protected set {
            if (value < 0 || value > 1)
                throw new Exception($"Noise Sensitivity out of bounds: {value}");
            _noiseSensitivity = value;
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"{this} took {damage} damage");
        if (Health <= 0)
        {
            RaycastHit raycast;
            Vector3 point;
            Vector3 raycastPosition = transform.position;
            raycastPosition.y += 5;
            if (Physics.Raycast(raycastPosition, Vector3.down, out raycast, 10f, 1 << LayerMask.NameToLayer("Default")))
            {
                point = raycast.point;
            }
            else
            {
                point = transform.position;
            }
            SpriteRenderer corpseRenderer = Instantiate(corpsePrefab, point, Quaternion.Euler(0, 0, 90)).GetComponent<SpriteRenderer>();
            SpriteRenderer myRenderer = GetMainSpriteRenderer();
            corpseRenderer.sprite = myRenderer.sprite;
            if (this is Human h)
            {
                h.DropItem();
                MakeNoise(80, SoundSource.HumanDeath);
                GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
                // Last human is dead
                if (humans.Length == 1)
                {
                    Time.timeScale = 0;
                    GameObject.FindGameObjectWithTag("Play Panel").SetActive(false);
                    GameObject uiPanel = GameObject.FindGameObjectWithTag("UI Panel");
                    for (int i = 0; i < uiPanel.transform.childCount; i++)
                    {
                        if (uiPanel.transform.GetChild(i).CompareTag("Win Overlay"))
                            uiPanel.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    protected virtual void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        manuallyTraversingOffMeshLink = false;
        Health = 10;
    }

    protected virtual void Update()
    {
        if (NavMeshAgent.isOnOffMeshLink && !manuallyTraversingOffMeshLink)
        {
            manuallyTraversingOffMeshLink = true;
            OffMeshLink offMeshLink = (OffMeshLink) NavMeshAgent.navMeshOwner;
            Door door = offMeshLink.GetComponent<Door>();

            float d1 = Vector3.Distance(transform.position, offMeshLink.startTransform.position);
            float d2 = Vector3.Distance(transform.position, offMeshLink.endTransform.position);
            Vector3 endPosition = d1 < d2 ? offMeshLink.endTransform.position : offMeshLink.startTransform.position;

            if (door != null)
            {
                if (door.CanOpen(this))
                {
                    IEnumerator<bool> doorOpen = door.Open(transform.position, this).GetEnumerator();
                    StartCoroutine(UseDoor(endPosition, doorOpen));
                }
                else
                {
                    manuallyTraversingOffMeshLink = false;
                    NavMeshAgent.Warp(transform.position);
                    HandleDoorBlocked();
                }
            }
            else
            {
                StartCoroutine(UseOffMeshLink(endPosition));
            }
        }
    }

    protected IEnumerator UseDoor(Vector3 endPosition, IEnumerator<bool> doorOpen)
    {
        Vector3 startPosition = transform.position;
        float progress = 0;
        bool moving = false;
        while (NavMeshAgent.isOnOffMeshLink)
        {
            if (!moving)
            {
                doorOpen.MoveNext();
                moving = doorOpen.Current;
            }

            if (moving)
            {
                progress += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, progress);

                if(progress >= 1f)
                {
                    NavMeshAgent.CompleteOffMeshLink();
                    manuallyTraversingOffMeshLink = false;
                    yield break;
                }
            }
            yield return null;
        }
        manuallyTraversingOffMeshLink = false;
    }

    protected IEnumerator UseOffMeshLink(Vector3 endPosition)
    {
        Vector3 startPosition = transform.position;
        float progress = 0;
        while (NavMeshAgent.isOnOffMeshLink)
        {
            progress += Time.deltaTime * 2;
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            if (progress >= 1f)
            {
                NavMeshAgent.CompleteOffMeshLink();
                manuallyTraversingOffMeshLink = false;
                yield break;
            }
            yield return null;
        }
        manuallyTraversingOffMeshLink = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thing"></param>
    /// <returns>true if the attack went through and to rest the timer, false otherwise</returns>
    public abstract bool HurtBoxTrigger(Collider thing);

    public abstract void HandleDoorBlocked();

    public virtual SpriteRenderer GetMainSpriteRenderer()
    {
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        return renderer;
    }
}
