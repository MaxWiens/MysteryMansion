using UnityEngine;
using System.Collections;
using static Items;

public class BookshelfHaunt : Haunt
{
    [SerializeField]
    private AudioSource _sound;
    private const float _TIME_TO_FALL = 0.5F;
    [SerializeField]
    private Collider _bookshelfCollider = null;
    public GameObject worldItemPrefab;
    public override IEnumerator HauntAction()
    {
        if (IsTriggered) yield break;
        IsTriggered = true;

        Quaternion q = transform.rotation;
        float time = 0f;
        // set collision to trigger
        _bookshelfCollider.isTrigger = true;
        Quaternion end = Quaternion.Euler(-90f, q.eulerAngles.y, 0f);

        while (time <= _TIME_TO_FALL)
        {
            //if collides with a human kill them here
            time += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(q, end, time / (_TIME_TO_FALL));
            yield return null;
        }
        transform.rotation = end;
        _sound.Play();
        // make it not kill people anymore
        _bookshelfCollider.isTrigger = false;

        yield return new WaitForSeconds(1.5f);

        float deathTimer_Max = 0.4f;
        float deathTimer = deathTimer_Max;
        Vector3 origSize = transform.localScale;

        while (deathTimer > 0)
        {
            yield return null;
            deathTimer -= Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, origSize, deathTimer / deathTimer_Max);
        }

        transform.localScale = new Vector3(0, 0, 1);

        Interactible interactible = GetComponent<Interactible>();
        Item item = interactible.GetItem();
        if (item != Item.None)
        {
            Debug.LogWarning("Spawning item");
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
            worldItem.Item = item;
        }

        yield return new WaitForEndOfFrame();

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        LivingThing lt = other.GetComponentInParent<LivingThing>();
        if (lt != null)
        {
            lt.TakeDamage(lt.Health);
        }
    }
}