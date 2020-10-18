using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class FastLoss : MonoBehaviour
{
    public Transform playerSpawnpoint;
    public GameObject worldItemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 p = playerSpawnpoint.position;
        WorldItem worldItem = Instantiate(worldItemPrefab, p, Quaternion.identity).GetComponent<WorldItem>();
        worldItem.Item = Item.HolyWater;

        p.x += 1;

        worldItem = Instantiate(worldItemPrefab, p, Quaternion.identity).GetComponent<WorldItem>();
        worldItem.Item = Item.Remains;

        Destroy(gameObject);
    }
}
