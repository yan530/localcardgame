using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DrapDrop : MonoBehaviour
{
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private GameObject dropZone = null;
    private Vector2 startPosition;
    private HUDManager hudManager;
    public Button Exchange;

    void Start()
    {
        hudManager = GameObject.FindObjectOfType<HUDManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }

    public void StartDrag()
    {
        Exchange.gameObject.SetActive(false);
        if (!isOverDropZone)
        {
            startPosition = transform.position;
            isDragging = true;
        }
    }

    public void EndDrag()
    {
        isDragging = false;

        if (isOverDropZone)
        {
            bool checkCard = hudManager.PlayCardCheck(transform.GetChild(2).GetComponent<TMP_Text>().text, transform.GetChild(0).GetComponent<TMP_Text>().text);

            //if the player doesn't have enough element cards, this card will be returned to the player's hand
            if (!checkCard)
            {
                transform.position = startPosition;
                Exchange.gameObject.SetActive(true);
            } else
            {
                transform.SetParent(dropZone.transform, false);
                Exchange.gameObject.SetActive(false);
            }

        } else
        {
            transform.position = startPosition;
            Exchange.gameObject.SetActive(true);
        }
    }
}
