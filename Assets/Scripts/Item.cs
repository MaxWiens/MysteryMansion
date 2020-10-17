using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Items
{
    public enum Item { Key = 0, Axe, Remains, None }
    private static Sprite[] sprites = new Sprite[Enum.GetValues(typeof(Item)).Length];

    private static string GetSpritePath(Item item)
    {
        switch (item)
        {
            case Item.Key:
                return "Art/items/key.png";
            case Item.Axe:
                return "Art/items/axe.png";
            /*case Item.Remains:
                return "remains";*/
            default:
                throw new NotImplementedException();
        }
    }

    public static Sprite GetSprite(Item item)
    {
        int index = (int)item;
        if (sprites[index] == null)
            sprites[index] = Resources.Load<Sprite>(GetSpritePath(item));
        return sprites[index];
    }
}
