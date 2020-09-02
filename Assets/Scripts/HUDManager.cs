using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Mirror;

public class HUDManager : NetworkBehaviour
{
    public TMP_Text currentPlayer;
    public GameObject card;
    public GameObject playerArea;
    public GameObject discardArea;
    public GameObject gameBoard;
    public Button drawCard;
    public List<GameObject> activeCards;
    public TMP_Text feedBack;
    public GameObject island;
    public GameObject elements;
    public GameObject win;
    public string[] sentence;
    public TMP_Text textHeader;
    public TMP_Text textInstruction;
    public TMP_Text textInstruction2;
    public TMP_Text textInstruction3;
    public GameObject isLose;
    public PlayerManager playerManager;

    private int cultureCount;

    IEnumerator Type()
    {
        foreach (char letter in sentence[0].ToCharArray())
        {
            textHeader.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        foreach (char letter in sentence[1].ToCharArray())
        {
            textInstruction.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        foreach (char letter in sentence[2].ToCharArray())
        {
            textInstruction2.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        foreach (char letter in sentence[3].ToCharArray())
        {
            textInstruction3.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void Update()
    {
        //update on the UI
        //GetcurrentPlayer(gameManager.currentPlayer.GetcurrentPlayer());
        //GetPlayerActions(gameManager.currentPlayer.GetActions());
    }

    //create a card object
    public void DrawCard()
    {
        //    Cards drawCard = gameManager.DrawCardData();
        //    if (drawCard == null)
        //    {
        //        isLose.SetActive(true);
        //    } else
        //    {
        //        AddCardDataToGameObject(drawCard);
        //        CheckStatus();
        //    }
    }

    //add the given drawcard data to the gameobject card
    private void AddCardDataToGameObject(Cards drawCard)
    {
        //    GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
        //    activeCards.Add(playerCard);
        //    drawCard.SetAttachment(playerCard);

        //    //set the canvas as the parent and the card will be a child for this element
        //    playerCard.transform.SetParent(PlayerArea.transform, false);
        //    playerCard.transform.GetChild(0).GetComponent<TMP_Text>().text = drawCard.GetCardType();
        //    playerCard.transform.GetChild(2).GetComponent<TMP_Text>().text = drawCard.GetCardID().ToString();
    }

    //check how many actions is left for the current player
    private void CheckStatus()
    {
        //    if (gameManager.currentPlayer.GetActions() == 0)
        //    {
        //        //if the player runs out of actions, call func from gamemanager
        //        //the screen is not interactable until the player end their turn
        //        endTurn.gameObject.SetActive(true);
        //        drawCard.gameObject.SetActive(false);
        //    }
    }

    //check if the player collected enough element cards of the same kind
    public bool PlayCardCheck(string cardID, string cardType)
    {
        //    feedBack.text = "";
        //    //check if the type of the card
        //    if (cardType == "Culture")
        //    {
        //        string description = gameManager.FindCard(cardID).GetCardDescription();
        //        if (cultureCount < 6)
        //        {
        //            island.transform.GetChild(cultureCount).GetComponent<TMP_Text>().text = description;
        //        } else
        //        {
        //            cultureCount = 0;
        //            island.transform.GetChild(cultureCount).GetComponent<TMP_Text>().text = description;
        //        }
        //        cultureCount++;
        //    }
        //    else if (cardType == "Traveler")
        //    {
        //        feedBack.text = "Traveling";
        //    }
        //    else
        //    {
        //        Cards[] playerCards = gameManager.currentPlayer.GetPlayerCards().ToArray();
        //        int count = 0;
        //        List<Cards> checkCards = new List<Cards>();

        //        //count the number of cards of the same type are in the player's hand
        //        foreach (Cards card in playerCards)
        //        {
        //            if (card.GetCardType() == cardType)
        //            {
        //                count++;
        //                checkCards.Add(card);
        //            }
        //        }

        //        //if there're enough element cards
        //        if (count >= 4)
        //        {
        //            //remove gameobject of the card
        //            foreach (GameObject active in activeCards.ToArray())
        //            {
        //                if (active.transform.GetChild(0).GetComponent<TMP_Text>().text == cardType
        //                    && active.transform.GetChild(2).GetComponent<TMP_Text>().text != cardID)
        //                {
        //                    activeCards.Remove(active);
        //                    Destroy(active);
        //                }
        //            }

        //            //remove card object in the player's hand
        //            foreach(Cards card in checkCards)
        //            {
        //                gameManager.currentPlayer.RemovePlayerCards(card);
        //            }

        //            //show image when it is collected
        //            elements.transform.Find(cardType).GetComponent<Image>().enabled = true;
        //            gameManager.PlayCard(cardID, cardType);
        //            feedBack.text = gameManager.question;
        //            CheckStatus();
        //            return true;
        //        }else
        //        {
        //            bool collected = false;
        //            foreach(Elements element in gameManager.elements)
        //            {
        //                if (element.GetLabel() == cardType && element.IsCollected())
        //                {
        //                    collected = true;
        //                    feedBack.text = "You have already collected " + cardType + " cards.";
        //                }
        //            }
        //            if (!collected)
        //            {
        //                //the card is an element card but there aren't enough
        //                feedBack.text = "You don't have enough " + cardType + " cards.";
        //            }
        //            return false;
        //        }
        //    }
        //    gameManager.PlayCard(cardID, cardType);
        //    CheckStatus();
        return true;
    }

//press the endturn button
public void TaskOnClick()
    {
        Invoke("NewRound", 0.3f);
    }

    //new round of the game is set up for the next player
    public void NewRound()
    {
        //if (gameManager.isWin)
        //{
        //    win.SetActive(true);
        //}
        //else
        //{
        //    feedBack.text = "";
        //    endTurn.gameObject.SetActive(false);
        //    drawCard.gameObject.SetActive(true);
        //    gameManager.ChangeIndex();
        //    ClearCards();
        //    gameManager.PlayRounds();
        //    SetUpCards();
        //}
    }

    private void SetUpCards()
    {
        //List<Cards> init = gameManager.currentPlayer.GetPlayerCards();
        //if (init.Count > 0)
        //{
        //    foreach (Cards card in init)
        //    {
        //        AddCardDataToGameObject(card);
        //    }
        //}
    }

    //clear the cards in the playarea
    private void ClearCards()
    {
        foreach (GameObject card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();
    }

    //display player name
    public void GetcurrentPlayer(string inputName)
    {
        currentPlayer.text = inputName;
    }
}
