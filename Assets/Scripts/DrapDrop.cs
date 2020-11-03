using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class DrapDrop : NetworkBehaviour
{
    public GameObject DiscardZone;
    //public GameObject GiveZone;

    private bool isDragging = false;
    private bool isOverDiscardZone = false;
    private bool isOverGiveZone = false;
    private GameObject dropZone;
    private Vector2 startPosition;
    public PlayerManager playerManager;

    private void Start()
    {
        DiscardZone = GameObject.Find("Panel_upper");
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
        isOverDiscardZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDiscardZone = false;
        dropZone = null;
    }

    public void StartDrag()
    {
        if (!isOverDiscardZone)
        {
            startPosition = transform.position;
            isDragging = true;
        }
    }

    public void EndDrag()
    {
        isDragging = false;

        if (isOverDiscardZone)
        {
            Debug.Log("over drop zone");
            NetworkIdentity netID = NetworkClient.connection.identity;
            playerManager = netID.GetComponent<PlayerManager>();
            playerManager.CollectElement(transform.GetChild(0).GetComponent<TMP_Text>().text);
            //if the player doesn't have enough element cards, this card will be returned to the player's hand
            if (!playerManager.CheckAElement())
            {
                transform.position = startPosition;
            }

        } else if (isOverGiveZone)
        {
            //give the card to another player
        } else
        {
            transform.position = startPosition;
        }
    }
}
