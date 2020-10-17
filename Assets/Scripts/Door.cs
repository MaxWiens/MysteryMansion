using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Transform pivot;
    [SerializeField]
    private Transform side1;
    [SerializeField]
    private Quaternion openTowardSide1;
    [SerializeField]
    private Transform side2;
    [SerializeField]
    private Quaternion openTowardSide2;
    [SerializeField]
    private Quaternion closedRotation;
    [SerializeField]
    private bool open = false;
    [SerializeField]
    private bool locked = false;

    private OffMeshLink offMeshLink;
    private IEnumerator openCoroutine;
    private bool opening;

    // Start is called before the first frame update
    void Start()
    {
        offMeshLink = GetComponent<OffMeshLink>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanOpen(Human h)
    {
        return !locked;
    }

    public IEnumerable<bool> Open(Vector3 userPosition, LivingThing user)
    {
        if (!open && !opening)
        {
            if (openCoroutine != null)
                StopCoroutine(openCoroutine);
            openCoroutine = OpenCoroutine(userPosition);
            StartCoroutine(openCoroutine);
            user.MakeNoise(40f, LivingThing.SoundSource.Environment);
        }

        while (true)
        {
            yield return open;
        }
    }

    private IEnumerator OpenCoroutine(Vector3 userPosition)
    {
        opening = true;
        float d1 = Vector3.Distance(userPosition, side1.position);
        float d2 = Vector3.Distance(userPosition, side2.position);

        Quaternion endRotation = d1 < d2 ? openTowardSide2 : openTowardSide1;

        float angle = Quaternion.Angle(pivot.localRotation, endRotation);
        if (angle > 180)
            angle = 360 - angle;
        float progress = 1 - angle / 90;

        while (true)
        {
            yield return null;
            // Take half a second to open the door, I think
            progress += 2 * Time.deltaTime;
            //Debug.Log($"{pivot.localRotation.eulerAngles} {closedRotation.eulerAngles} {endRotation.eulerAngles}");
            pivot.localRotation = Quaternion.SlerpUnclamped(closedRotation, endRotation, progress);
            if (progress >= 1f)
                break;
        }

        open = true;
        opening = false;
        yield return new WaitForSeconds(3f);

        open = false;
        progress = 0;

        while (true)
        {
            // Take half a second to open the door, I think
            progress += 2 * Time.deltaTime;
            pivot.localRotation = Quaternion.Slerp(endRotation, closedRotation, progress);
            if (progress >= 1f)
                break;
            yield return null;
        }

        yield break;
    }
}
