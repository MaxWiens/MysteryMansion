using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class Interactible : MonoBehaviour
{
    private Item _item = Item.None;
    public virtual Item Item
    {
        get => _item;
        set
        {
            _item = value;
        }
    }
    public Human Claimant { get; set; }

    public virtual Item GetItem()
    {
        return Item;
    }

    public virtual Item TakeItem()
    {
        Item ret = Item;
        Item = Item.None;
        return ret;
    }
}
