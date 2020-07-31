using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardZoom : MonoBehaviour
{
    public GameObject canvas;
    private GameObject zoomCard = null;
    public GameObject dropZone;

    public void Awake()
    {
        canvas = GameObject.Find("Canvas");

    }

    public void onHoverExit()
    {
        Destroy(zoomCard);
    }

    public void onClick()
    {
        if (transform.parent.name != dropZone.name)
        {
            if (zoomCard == null)
            {
                zoomCard = Instantiate(gameObject, new Vector2(gameObject.transform.position.x, 500), Quaternion.identity);
                zoomCard.transform.SetParent(canvas.transform, false);
                zoomCard.layer = LayerMask.NameToLayer("Zoom");
                RectTransform rect = zoomCard.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(320, 410);
            } else
            {
                onHoverExit();
            }
        }
    }
}
