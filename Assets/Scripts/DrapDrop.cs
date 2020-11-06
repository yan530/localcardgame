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
    private string dropzoneName;

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
        dropzoneName = dropZone.name;
        Debug.Log(dropzoneName);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }

    public void StartDrag()
    {
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
            if (dropzoneName == "Panel_islandarea")
            {

                bool checkCard = hudManager.PlayCardCheck(transform.GetChild(1).GetComponent<TMP_Text>().text, transform.GetChild(0).GetComponent<TMP_Text>().text);

                //if the player doesn't have enough element cards, this card will be returned to the player's hand
                if (!checkCard)
                {
                    transform.position = startPosition;
                }
                else
                {
                    Destroy(this.gameObject);
                }
            } else if (dropzoneName == "Player2")
            {
                hudManager.GiveCard(transform.GetChild(1).GetComponent<TMP_Text>().text, transform.GetChild(0).GetComponent<TMP_Text>().text, 1);
                Destroy(this.gameObject);

            }
            //else if (dropzoneName == "Player3")
            //{
            //    hudManager.GiveCard(transform.GetChild(1).GetComponent<TMP_Text>().text, transform.GetChild(0).GetComponent<TMP_Text>().text, 2);
            //    Destroy(this.gameObject);

            //}
            else if (dropzoneName == "Player4")
            {
                hudManager.GiveCard(transform.GetChild(1).GetComponent<TMP_Text>().text, transform.GetChild(0).GetComponent<TMP_Text>().text, 3);
                Destroy(this.gameObject);
            }
            else
            {
                transform.position = startPosition;
            }

        } else
        {
            transform.position = startPosition;
        }
    }
}
