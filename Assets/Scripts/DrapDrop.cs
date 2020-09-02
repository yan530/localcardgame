using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class DrapDrop : NetworkBehaviour
{
    private bool isDragging = false;
    private bool isOverDiscardZone = false;
    private bool isOverGiveZone = false;
    private GameObject dropZone = null;
    private Vector2 startPosition;
    public PlayerManager playerManager;


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
        NetworkIdentity netID = NetworkClient.connection.identity;
        playerManager = netID.GetComponent<PlayerManager>();
        isDragging = false;

        if (isOverDiscardZone)
        {
            bool checkCard = playerManager.CmdCollectElements(transform.GetChild(2).GetComponent<TMP_Text>().text, transform.GetChild(0).GetComponent<TMP_Text>().text);

            //if the player doesn't have enough element cards, this card will be returned to the player's hand
            if (!checkCard)
            {
                transform.position = startPosition;
            } else
            {
                transform.SetParent(dropZone.transform, false);
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
