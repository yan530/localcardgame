using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerManager : NetworkBehaviour
{
    //game
    public List<Players> players;
    public List<Cards> cards;
    public List<Elements> elements;
    public List<Cards> communications;
    public Players currentPlayer;
    public string question;
    public bool isWin;

    //HUD
    public GameObject card;
    public GameObject playerArea;
    public GameObject discardArea;
    public GameObject gameBoard;
    public GameObject endTurn;
    public List<GameObject> activeCards;
    public TMP_Text feedBack;
    public GameObject win;
    public GameObject elementsDisplay;
    public GameObject communicationDisplay;

    //instruction
    public string[] sentence;
    public TMP_Text textHeader;
    public TMP_Text textInstruction;
    public TMP_Text textInstruction2;
    public TMP_Text textInstruction3;


    private int cultureCount;

    //debug
    public int count;
    public string playerName;

    public void loadScene(string playerName)
    {
        currentPlayer = Players.CreateInstance(playerName);
        CmdAddPlayer(playerName);
        gameBoard.SetActive(true);

        //hud
        playerArea = GameObject.Find("Panel_player1");
        discardArea = GameObject.Find("Panel_discard");
        endTurn = GameObject.Find("Panel_endTurn");

        activeCards = new List<GameObject>();
        cultureCount = 0;
    }

    [Command]
    void CmdAddPlayer(string playerName)
    {
        players.Add(currentPlayer);
    }

    [Server]
    public override void OnStartServer()
    {
        players = new List<Players>();
        cards = new List<Cards>();
        elements = new List<Elements>();
        communications = new List<Cards>();
        count = 0;
        question = "";
        isWin = false;
        LoadCards();
        //PlayRounds();

    }

    //load data from csv file
    private void LoadCards()
    {
        AddData("Card_data", "cards");
        AddData("Element_data", "elements");
    }

    //load data and concatinate it into appropriate form
    private void AddData(string resource, string type)
    {
        TextAsset elementData = Resources.Load<TextAsset>(resource);

        string[] data = elementData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            if (type == "elements")
            {
                elements.Add(Elements.CreateInstance(i, row[1], row[2], false));
            }
            else
            {
                cards.Add(Cards.CreateInstance(i, row[1], row[2], row[3]));
            }
        }
    }

    //turn based method
    [Server]
    private void PlayRounds()
    {
        count++;
        if (count > players.Count - 1)
        {
            count = 0;
        }
        question = "";
        HaltTask("RpcCheckTurn");
    }

    public void HaltTask(string name)
    {
        Invoke(name, 0.3f);
    }

    [ClientRpc]
    void RpcCheckTurn()
    {
        if (isWin)
        {
            GameObject.Find("Panel_win").SetActive(true);
        } else if (players[count] == currentPlayer)
        {
            endTurn = GameObject.Find("Panel_endTurn");
            endTurn.gameObject.SetActive(false);
        }
    }

    //get a card data from the cards list
    [Command]
    public void CmdDrawCardData()
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.Log("No cards left");
        }

        //check if an card has been exchanged and moved to the top of the deck
        Cards drawCard;
        drawCard = cards[UnityEngine.Random.Range(0, cards.Count - 1)];
        currentPlayer.AddPlayerCards(drawCard);
        cards.Remove(drawCard);

        Debug.Log("Got Card Data");

        //hud
        if (drawCard == null)
        {
            Debug.Log("Out of Cards");
        }
        else
        {

            GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
            NetworkServer.Spawn(playerCard, connectionToClient);
            activeCards.Add(playerCard);
            drawCard.SetAttachment(playerCard);

            Debug.Log("Got Card from prefab");

            if (hasAuthority)
            {
                //set the canvas as the parent and the card will be a child for this element
                playerCard.transform.SetParent(playerArea.transform, false);
                playerCard.transform.GetChild(0).GetComponent<TMP_Text>().text = drawCard.GetCardType();
                playerCard.transform.GetChild(2).GetComponent<TMP_Text>().text = drawCard.GetCardID().ToString();
                Debug.Log("Add Card to the Player Area");
            }
            HaltTask("PlayRounds");
        }
    }

    //check if the player collected enough element cards of the same kind
    [Command]
    public bool CmdCollectElements(string cardID, string cardType)
    {
        Cards card = FindCard(cardID);

        //hud
        Cards[] playerCards = currentPlayer.GetPlayerCards().ToArray();
        int count = 0;
        List<Cards> checkCards = new List<Cards>();

        //count the number of cards of the same type are in the player's hand
        foreach (Cards cards in playerCards)
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
                    && active.transform.GetChild(2).GetComponent<TMP_Text>().text != cardID)
                {
                    activeCards.Remove(active);
                    Destroy(active);
                }
            }

            //remove card object in the player's hand
            foreach (Cards cards in checkCards)
            {
                currentPlayer.RemovePlayerCards(card);
            }

            question = CmdCheckElements(cardType);
            RpcElementCollected(cardType, question);
            return true;
        }
        else
        {
            bool collected = false;
            foreach (Elements element in elements)
            {
                if (element.GetLabel() == cardType && element.IsCollected())
                {
                    collected = true;
                    feedBack.text = "You have already collected " + cardType + " cards.";
                }
            }
            if (!collected)
            {
                //the card is an element card but there aren't enough
                feedBack.text = "You don't have enough " + cardType + " cards.";
            }
            return false;
        }
    }

    //Find card in the active cards
    private Cards FindCard(string cardID)
    {

        Cards card = ScriptableObject.CreateInstance<Cards>();
        List<Cards> playercards = currentPlayer.GetPlayerCards();
        foreach (Cards findCard in playercards.ToArray())
        {
            if (findCard.GetCardID().ToString() == cardID)
            {
                card = findCard;
                currentPlayer.RemovePlayerCards(findCard);
            }
        }
        return card;
    }

    [Command]
    public string CmdCheckElements(string cardType)
    {
        int collectAll = 5;
        string question = "";
        foreach (Elements element in elements)
        {
            if (!element.IsCollected())
            {
                if (element.GetLabel() == cardType)
                {
                    question = element.GetQuestion();
                }
                else
                {
                    collectAll--;
                }

            }
        }
        if (collectAll == 5)
        {
            isWin = true;
        }
        return question;
    }

    [ClientRpc]
    void RpcElementCollected(string cardType, string question)
    {
        //show image when it is collected
        elementsDisplay.transform.Find(cardType).GetComponent<Image>().enabled = true;
        feedBack.text = question;

        //click Button to playround
    }
}
