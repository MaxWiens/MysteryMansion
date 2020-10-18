using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Items;

public class Altar : MonoBehaviour
{
    public Transform appreciationPosition;
    public Transform spot1;
    public Transform spot2;
    public GameObject altarItemPrefab;
    public Transform finalCameraPosition;

    private Item spot1Item;
    private Item spot2Item;
    private bool cameraMoveDone;

    private void Start()
    {
        spot1Item = Item.None;
        spot2Item = Item.None;
    }

    public bool Appreciate(Human h)
    {
        if (h.MyItem == Item.HolyWater || h.MyItem == Item.Remains)
        {
            if (spot1Item == Item.None && spot2Item != h.MyItem)
            {
                spot1Item = h.MyItem;
                h.RemoveItem();

                GameObject o = Instantiate(altarItemPrefab, spot1.position, Quaternion.identity);
                o.GetComponent<SpriteRenderer>().sprite = GetSprite(spot1Item);
                o.GetComponent<Billboard>().LateUpdate();

                if (spot2Item != Item.None)
                    StartCoroutine(FinishGame(o.GetComponent<Billboard>()));

                return true;
            }
            else if (spot2Item == Item.None && spot1Item != h.MyItem)
            {
                spot2Item = h.MyItem;
                h.RemoveItem();

                GameObject o = Instantiate(altarItemPrefab, spot2.position, Quaternion.identity);
                o.GetComponent<SpriteRenderer>().sprite = GetSprite(spot2Item);
                o.GetComponent<Billboard>().LateUpdate();

                if (spot1Item != Item.None)
                    StartCoroutine(FinishGame(o.GetComponent<Billboard>()));

                return true;
            }
        }

        return false;
    }

    private IEnumerator FinishGame(Billboard b)
    {
        Time.timeScale = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (player != null && camera != null)
        {
            CameraFollow cf = player.GetComponentInChildren<CameraFollow>();
            if (cf != null)
                cf.enabled = false;

            IEnumerator<bool> cameraMoveStatus = DoCameraMove(camera.transform).GetEnumerator();
            yield return new WaitUntil(() => !cameraMoveStatus.MoveNext() || cameraMoveStatus.Current);
        }

        if (b != null)
            b.LateUpdate();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameObject.FindGameObjectWithTag("Play Panel").SetActive(false);
        GameObject uiPanel = GameObject.FindGameObjectWithTag("UI Panel");
        for (int i = 0; i < uiPanel.transform.childCount; i++)
        {
            if (uiPanel.transform.GetChild(i).CompareTag("Lose Overlay"))
                uiPanel.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private IEnumerable<bool> DoCameraMove(Transform camera)
    {
        cameraMoveDone = false;

        StartCoroutine(MoveCamera(camera));

        while (true)
        {
            yield return cameraMoveDone;
        }
    }

    private IEnumerator MoveCamera(Transform camera)
    {
        float totalTime = 2f;
        float time = 0;

        Vector3 startPos = camera.position;
        Quaternion startRot = camera.rotation;
        while (time < totalTime)
        {
            // Time is stopped
            time += Time.unscaledDeltaTime;

            camera.rotation = Quaternion.Slerp(startRot, finalCameraPosition.rotation, time / totalTime);
            camera.position = Vector3.Lerp(startPos, finalCameraPosition.position, time / totalTime);
            yield return null;
        }

        cameraMoveDone = true;
    }
}
