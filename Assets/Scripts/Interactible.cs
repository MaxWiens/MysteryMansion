using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class Interactible : MonoBehaviour
{
    [SerializeField]
    private List<Item> items;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual Item FinishSearch()
    {
        if (items.Count > 0)
        {
            int index = Random.Range(0, items.Count);
            Item item = items[index];
            items.RemoveAt(index);
            return item;
        }

        return Item.None;
    }
}
