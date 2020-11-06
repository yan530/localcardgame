using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public GameObject playerInfo;
    private GameManager gameManager;
    public GameObject card;
    public GameObject PlayerArea;
    public GameObject IslandArea;
    public GameObject endTurn;
    public Button drawCard;
    public List<GameObject> activeCards;
    public TMP_Text feedBack;
    public TMP_Text culture;
    public GameObject elements;

    public GameObject elementPanel;
    public TMP_Text elementType;
    public TMP_Text elementQuestion;
    public Image elementImage;

    public GameObject win;
    public string[] sentence;
    public TMP_Text textHeader;
    public TMP_Text textInstruction;
    public TMP_Text textInstruction2;
    public TMP_Text textInstruction3;

    private int cultureCount;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        activeCards = new List<GameObject>();
        cultureCount = 0;
        StartCoroutine(Type());
        GetPlayerName(gameManager.index);
        NewCulture();
    }

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

    //create a card object
    public void DrawCard()
    {
        Cards drawCard = gameManager.DrawCardData();
        if (drawCard == null)
        {
            feedBack.text = "Out of drawing cards. Give a card or End Turn.";
        } else
        {
            AddCardDataToGameObject(drawCard);
            CheckStatus();
        }
    }

    //add the given drawcard data to the gameobject card
    private void AddCardDataToGameObject(Cards drawCard)
    {
        GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
        activeCards.Add(playerCard);
        drawCard.SetAttachment(playerCard);

        //set the canvas as the parent and the card will be a child for this element
        playerCard.transform.SetParent(PlayerArea.transform, false);
        playerCard.transform.GetChild(0).GetComponent<TMP_Text>().text = drawCard.GetCardType();
        playerCard.transform.GetChild(1).GetComponent<TMP_Text>().text = drawCard.GetCardID() + "";
    }

    public void GiveCard(string cardID, string cardType, int num)
    {
        int receiverIndex = gameManager.index + num;
        if (receiverIndex > 3)
        {
            receiverIndex -= 4;
        }

        gameManager.GiveCard(cardID, cardType, receiverIndex);

        feedBack.text = gameManager.currentPlayer.GetPlayerName() + " gives " + cardID + " " + cardType +
                    " to " + gameManager.players[receiverIndex].GetPlayerName();
        CheckStatus();
    }

    //check if the player collected enough element cards of the same kind
    public bool PlayCardCheck(string cardID, string cardType)
    {
        feedBack.text = "";

        //check if the element is collected
        foreach (Elements element in gameManager.elements)
        {
            if (element.GetLabel() == cardType && element.IsCollected())
            {
                //if the element is collected, return false
                feedBack.text = cardType + " is already collected.";
                return false;
            }
        }

        //if the element is not collected yet, check the number of cards in the player's hand
        Cards[] playerCards = gameManager.currentPlayer.GetPlayerCards().ToArray();
        int count = 0;
        List<Cards> checkCards = new List<Cards>();

        //count the number of cards of the same type are in the player's hand
        foreach (Cards card in playerCards)
        {
            if (card.GetCardType() == cardType)
            {
                count++;
                checkCards.Add(card);
            }
        }

        //if there're enough element cards
        if (count >= 4)
        {
            //remove gameobject of the card
            foreach (GameObject active in activeCards.ToArray())
            {
                if (active.transform.GetChild(0).GetComponent<TMP_Text>().text == cardType
                    && active.transform.GetChild(1).GetComponent<TMP_Text>().text != cardID)
                {
                    activeCards.Remove(active);
                    Destroy(active);
                }
            }

            //remove card object in the player's hand
            foreach (Cards card in checkCards)
            {
                gameManager.currentPlayer.RemovePlayerCards(card);
            }

            //show image when it is collected
            elements.transform.Find(cardType).GetComponent<Image>().enabled = true;
            ShowQuestion(cardType);
            feedBack.text = "You collected " + cardType + " cards.";
            gameManager.PlayCard(cardType);
            return true;
        }
        else
        {
            //the card is an element card but there aren't enough
            feedBack.text = "You don't have enough " + cardType + " cards.";
            return false;
        }
    }

    private void ShowQuestion(string type)
    {
        foreach (Elements element in gameManager.elements)
        {
            if (element.GetLabel() == type)
            {
                elementPanel.SetActive(true);
                elementQuestion.text = element.GetQuestion();
                elementType.text = type;
                elementImage = element.GetImage();
            }
        }
    }

    //check how many actions is left for the current player
    public void CheckStatus()
    {
        if (gameManager.currentPlayer.GetActions() == 0)
        {
            //if the player runs out of actions, call func from gamemanager
            //the screen is not interactable until the player end their turn
            endTurn.gameObject.SetActive(true);
            Invoke("UpdatePlayers", 1f);
        }
    }

    public void EndTurn()
    {
        if (gameManager.currentPlayer.GetActions() > 0)
        {
            gameManager.currentPlayer.DecreaseActions();
        }
        CheckStatus();
    }

    private void UpdatePlayers()
    {
        if (gameManager.isWin)
        {
            win.SetActive(true);
        }
        else
        {
            gameManager.ChangeIndex();
            if (gameManager.newRound)
            {
                NewCulture();
            }
            feedBack.text = "";
            endTurn.gameObject.SetActive(false);
            drawCard.gameObject.SetActive(true);
            ClearCards();
            gameManager.PlayRounds();
            GetPlayerName(gameManager.index);
            SetUpCards();
        }
    }

    public void NewCulture()
    {
        Cards newCulture = gameManager.GetNewCulture();
        culture.text = newCulture.GetCardDescription();
    }

    private void SetUpCards()
    {
        List<Cards> init = gameManager.currentPlayer.GetPlayerCards();
        if (init.Count > 0)
        {
            foreach(Cards card in init)
            {
                AddCardDataToGameObject(card);
            }
        }
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
    public void GetPlayerName(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            playerInfo.transform.GetChild(i).GetComponent<TMP_Text>().text = gameManager.players[index].GetPlayerName();
            index++;
            if (index > 3)
            {
                index = 0;
            }
        }
    }
}
