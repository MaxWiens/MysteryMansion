using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class RandomManager : MonoBehaviour
{
    public GameObject humanPrefab;
    public Transform humanSpawnpoint;
    public List<Sprite> humanSprites;
    public List<Item> items;

    public const int HumansToSpawn = 6;
    // Start is called before the first frame update
    void Start()
    {
        List<Sprite> spritesLeft = new List<Sprite>(humanSprites);

        float angle = 2 * Mathf.PI / HumansToSpawn;
        for (int i = 0; i < HumansToSpawn; i++)
        {
            Vector3 pos = humanSpawnpoint.position + new Vector3(Mathf.Cos(angle * i), 0, Mathf.Sin(angle * i));
            Human h = Instantiate(humanPrefab, pos, Quaternion.identity).GetComponent<Human>();
            if (spritesLeft.Count == 0)
                spritesLeft.AddRange(humanSprites);
            int index = Random.Range(0, spritesLeft.Count);
            Sprite sprite = spritesLeft[index];
            spritesLeft.RemoveAt(index);
            SpriteRenderer sr = h.GetMainSpriteRenderer();
            if (sr != null)
                sr.sprite = sprite;
        }

        if (items.Count > 0)
        {
            List<GameObject> interactibleObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Interactable"));
            if (interactibleObjects.Count == 0)
                throw new System.Exception("No interactibles in the level, game cannot be lost.");
            else while (interactibleObjects.Count < items.Count)
                    interactibleObjects.AddRange(interactibleObjects);
            for (int i = 0; i < items.Count; i++)
            {
                int index = Random.Range(0, interactibleObjects.Count);
                Interactible interactible = interactibleObjects[index].GetComponent<Interactible>();
                if (interactible != null && !(interactible is WorldItem))
                {
                    if (interactible.items == null)
                        interactible.items = new List<Item>();

                    interactible.items.Add(items[i]);
                }
            }
        }
        
        Destroy(gameObject);
    }
}
