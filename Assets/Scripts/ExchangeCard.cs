using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExchangeCard : MonoBehaviour
{
    private GameManager gameManager;
    private HUDManager hudManager;
    private Cards card;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        hudManager = GameObject.FindObjectOfType<HUDManager>();
    }

    public void Exchange()
    {
        //get the cardID and call findcard func in gamemanager
        //then remove the card from the player's card and ui
        card = gameManager.FindCard(transform.parent.GetChild(2).GetComponent<TMP_Text>().text);
        //Debug.Log(card.GetCardID());
        gameManager.currentPlayer.RemovePlayerCards(card);
        hudManager.activeCards.Remove(transform.gameObject);

        //delay draw a new card
        Invoke("DrawCard", 0.3f);
    }

    private void DrawCard()
    {
        hudManager.DrawCard();
        //move the card to the top of the deck
        gameManager.topOfTheDeck = card;
    }
}
