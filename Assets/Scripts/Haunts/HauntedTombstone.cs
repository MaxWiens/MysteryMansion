using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntedTombstone : Haunt
{
    public const int MaxZombies = 3;

    public GameObject zombiePrefab;
    public Transform zombieSpawnLocation;
    public override IEnumerator HauntAction()
    {
        IsTriggered = true;

        Instantiate(zombiePrefab, zombieSpawnLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(10);

        while (true)
        {
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
            if (monsters.Length <= MaxZombies)
                break;
            yield return new WaitForSeconds(2); 
        }

        IsTriggered = false;
    }
}
