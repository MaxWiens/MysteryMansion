using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerSpawnpoint;
    public List<Sprite> humanSprites;

    public const int HumansToSpawn = 6;
    // Start is called before the first frame update
    void Start()
    {
        List<Sprite> spritesLeft = new List<Sprite>(humanSprites);

        float angle = 2 * Mathf.PI / HumansToSpawn;
        for (int i = 0; i < HumansToSpawn; i++)
        {
            Vector3 pos = playerSpawnpoint.position + new Vector3(Mathf.Cos(angle * i), 0, Mathf.Sin(angle * i));
            Human h = Instantiate(playerPrefab, pos, Quaternion.identity).GetComponent<Human>();
            if (spritesLeft.Count == 0)
                spritesLeft.AddRange(humanSprites);
            int index = Random.Range(0, spritesLeft.Count);
            Sprite sprite = spritesLeft[index];
            spritesLeft.RemoveAt(index);
            SpriteRenderer sr = h.GetMainSpriteRenderer();
            if (sr != null)
                sr.sprite = sprite;
        }

        Destroy(gameObject);
    }
}
