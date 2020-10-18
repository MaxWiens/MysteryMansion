using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHaunt : Haunt
{
    [SerializeField]
    private Door door;
    [SerializeField]
    private AudioSource _sound;
    public override IEnumerator HauntAction()
    {
        _sound.Play();
        IsTriggered = true;
        door.locked = true;
        yield return new WaitForSeconds(8);
        door.locked = false;
        yield return new WaitForSeconds(10);
        EnergyCost = Mathf.Min(EnergyCost + 1, 8);
        IsTriggered = false;
    }
}
