using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class WorldItem : MonoBehaviour
{
    private new SpriteRenderer renderer;
    private Item _item;
    public Item Item {
        get => _item;
        set {
            _item = value;
            renderer.sprite = GetSprite(_item);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
}
