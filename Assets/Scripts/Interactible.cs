using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> items;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual GameObject FinishSearch()
    {
        if (items.Count > 0)
        {
            int index = Random.Range(0, items.Count);
            GameObject item = items[index];
            items.RemoveAt(index);
            return item;
        }

        return null;
    }
}
