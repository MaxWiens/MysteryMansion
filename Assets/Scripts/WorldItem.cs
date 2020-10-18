using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class WorldItem : Interactible
{
    private new SpriteRenderer renderer;
    private Item _item;
    public Item Item {
        get => _item;
        set {
            _item = value;
            if (renderer == null)
                renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.sprite = GetSprite(_item);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (renderer == null)
            renderer = GetComponent<SpriteRenderer>();
    }

    public override Item TakeItem(Human human)
    {
        Destroy(gameObject);
        return _item;
    }

    public override Item[] GetItems()
    {
        return new Item[] { Item };
    }
}
