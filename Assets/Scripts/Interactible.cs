using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class Interactible : MonoBehaviour
{
    public List<Item> items;

    public virtual Item[] GetItems()
    {
        return items.ToArray();
    }

    public virtual Item TakeItem(Human human)
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
