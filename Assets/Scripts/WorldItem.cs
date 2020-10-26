using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class WorldItem : Interactible
{
    private new SpriteRenderer renderer;

    public override Item Item {
        get => base.Item;
        set {
            base.Item = value;
            if (renderer == null)
                renderer = GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
                renderer.sprite = GetSprite(Item);
        }
    }

    public override Item TakeItem()
    {
        Destroy(gameObject);
        return Item;
    }
}
