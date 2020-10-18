using UnityEngine;
using System.Collections;

public class BookshelfHaunt : Haunt
{
    [SerializeField]
    private AudioSource _sound;
    private const float _TIME_TO_FALL = 0.5F;
    [SerializeField]
    private Collider _bookshelfCollider = null;
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

        yield return new WaitForSeconds(1.5f);

        // set collision back
        _bookshelfCollider.isTrigger = false;

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