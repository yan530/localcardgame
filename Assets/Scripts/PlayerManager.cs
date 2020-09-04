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
    public GameObject lobby;
    public GameObject card;
    public GameObject playerArea;
    public GameObject discardArea;
    public GameObject gameBoard;
    public GameObject endTurn;
    public List<GameObject> activeCards;
    public GameObject feedback;
    public GameObject feedback2;
    public List<GameObject> feedbacks;
    public GameObject win;
    public GameObject elementsDisplay;
    public GameObject communicationDisplay;
    public bool collectAElement;
    public GameObject playerInfo;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    //instruction
    //public string[] sentence;
    //public TMP_Text textHeader;
    //public TMP_Text textInstruction;
    //public TMP_Text textInstruction2;
    //public TMP_Text textInstruction3;

    //debug
    public int count;
    public string playerName;
    private int playerNum;

    public static event Action onClientConnected;
    public static event Action onClientDisconnected;

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

    public override void OnStartClient()
    {
        base.OnStartClient();
        collectAElement = true;
        //var spawnablePrefab = Resources.LoadAll<GameObject>("SpawnablePrefabs");
        //foreach (var prefab in spawnablePrefab)
        //{
        //    ClientScene.RegisterPrefab(prefab);
        //}
    }

    public void LoadLobby(string playerName)
    {
        //generate the gameboard from prefab
        GameObject newLobby = Instantiate(lobby, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(newLobby, connectionToClient);

        if (hasAuthority)
        {
            //set the canvas as the parent and the card will be a child for this element
            newLobby.transform.SetParent(GameObject.Find("Canvas").transform, false);
        }
        currentPlayer = Players.CreateInstance(playerName);
        playerNum = players.Count;
        CmdAddPlayer();
    }

    public void LoadScene()
    {
        //generate the gameboard from prefab
        GameObject newGameBoard = Instantiate(gameBoard, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(newGameBoard, connectionToClient);

        if (hasAuthority)
        {
            //set the canvas as the parent and the card will be a child for this element
            newGameBoard.transform.SetParent(GameObject.Find("Canvas").transform, false);
        }

        //get each player area set up
        int index = 0;
        while (index < players.Count)
        {
            if (playerNum >= players.Count)
            {
                playerNum = 0;
            }
            DisplayPlayer(index, players[playerNum].GetPlayerName());
            index++;
            playerNum++;
        }

        //hud
        playerArea = GameObject.Find("Panel_player1");
        discardArea = GameObject.Find("Panel_discard");

        activeCards = new List<GameObject>();
    }

    //display player
    private void DisplayPlayer(int i, string name)
    {
        GameObject player;
        if (i == 0)
        {
            player = Instantiate(playerInfo, new Vector3(0, 0, 0), Quaternion.identity);

        } else if (i == 1)
        {
            player = Instantiate(player2, new Vector3(0, 0, 0), Quaternion.identity);

        } else if (i == 2)
        {
            player = Instantiate(player3, new Vector3(0, 0, 0), Quaternion.identity);

        } else
        {
            player = Instantiate(player4, new Vector3(0, 0, 0), Quaternion.identity);
        }
        NetworkServer.Spawn(player, connectionToClient);

        if (hasAuthority)
        {
            //set the canvas as the parent and the card will be a child for this element
            player.transform.SetParent(GameObject.Find("GameBoard").transform, false);
            player.transform.GetChild(1).GetComponent<TMP_Text>().text = name;
        }
    }

    [Command]
    void CmdAddPlayer()
    {
        players.Add(currentPlayer);
        RpcUpdateLobby();
    }

    [ClientRpc]
    void RpcUpdateLobby()
    {
        
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

    //create a panel over the player's play area when it's not the player's turn
    private void EndTurn()
    {
        GameObject createEndTurn = Instantiate(endTurn, new Vector3(0, 0, 0), Quaternion.identity);
        feedbacks.Add(createEndTurn);
        NetworkServer.Spawn(createEndTurn, connectionToClient);

        if (hasAuthority)
        {
            //set the canvas as the parent and the card will be a child for this element
            createEndTurn.transform.SetParent(GameObject.Find("GameBoard").transform, false);
        }
    }

    //check if the player is the currentplayer and if the game is won
    [ClientRpc]
    void RpcCheckTurn()
    {
        if (isWin)
        {
            ShowVictory();
        } else if (players[count] == currentPlayer)
        {
            ClearCards();
            SetUpCards();
            ClearFeedback();
        } else
        {
            HaltTask("EndTurn");
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

    //set up cards for the new round
    private void SetUpCards()
    {
        List<Cards> init = currentPlayer.GetPlayerCards();
        if (init.Count > 0)
        {
            foreach (Cards cards in init)
            {
                AddCardDataToGameObject(cards);
            }
        }
    }

    //add the given drawcard data to the gameobject card
    private void AddCardDataToGameObject(Cards drawCard)
    {
        GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(playerCard, connectionToClient);
        activeCards.Add(playerCard);

        if (hasAuthority)
        {
            //set the canvas as the parent and the card will be a child for this element
            playerCard.transform.SetParent(playerArea.transform, false);
            playerCard.transform.GetChild(0).GetComponent<TMP_Text>().text = drawCard.GetCardType();
            playerCard.transform.GetChild(1).GetComponent<TMP_Text>().text = drawCard.GetCardID().ToString();
        }
    }

    //get a card data from the cards list
    [Command]
    public void CmdDrawCardData()
    {
        ClearFeedback();
        if (cards == null || cards.Count == 0)
        {
            Debug.Log("No cards left");
        }

        //check if an card has been exchanged and moved to the top of the deck
        Cards drawCard;
        drawCard = cards[UnityEngine.Random.Range(0, cards.Count - 1)];
        currentPlayer.AddPlayerCards(drawCard);
        cards.Remove(drawCard);

        //hud
        if (drawCard == null)
        {
            Debug.Log("Out of Cards");
        }
        else
        {
            AddCardDataToGameObject(drawCard);
            HaltTask("PlayRounds");
        }
    }

    private void ClearFeedback()
    {
        foreach (GameObject feedback in feedbacks) { Destroy(feedback); }
        feedbacks.Clear();
    }

    //check if the player collected enough element cards of the same kind
    [Command]
    public void CmdCollectElements(string cardType)
    {
        ClearFeedback();
        collectAElement = true;
        //hud
        Cards[] playerCards = currentPlayer.GetPlayerCards().ToArray();
        int count = 0;
        List<Cards> checkCards = new List<Cards>();

        //count the number of cards of the same type are in the player's hand
        foreach (Cards cards in playerCards)
        {
            if (cards.GetCardType() == cardType)
            {
                count++;
                checkCards.Add(cards);
            }
        }

        //if there're enough element cards
        if (count >= 4)
        {
            //remove gameobject of the card
            foreach (GameObject active in activeCards.ToArray())
            {
                if (active.transform.GetChild(0).GetComponent<TMP_Text>().text == cardType)
                {
                    activeCards.Remove(active);
                    Destroy(active);
                }
            }

            //remove card object in the player's hand
            foreach (Cards cards in checkCards)
            {
                currentPlayer.RemovePlayerCards(cards);
            }
            CmdCheckElements(cardType);
        }
        else
        {
            collectAElement = false;
            NotEnoughCards(cardType);
        }
    }

    //give the currentplayer feedback on whether the element has been collected or the player does not have enough cards
    private void NotEnoughCards(string cardType)
    {
        string giveFeedback = "";
        bool collected = false;
        foreach (Elements element in elements)
        {
            if (element.GetLabel() == cardType && element.IsCollected())
            {
                collected = true;
                giveFeedback = "You have already collected " + cardType + " cards.";
            }
        }
        if (!collected)
        {
            //the card is an element card but there aren't enough
            giveFeedback = "You don't have enough " + cardType + " cards.";
        }
        GiveFeedback(giveFeedback, "feedback");
    }

    //the function that shows the feedback
    private void GiveFeedback(string giveFeedback, string type)
    {
        GameObject showFeedback = Instantiate(feedback, new Vector3(0, 0, 0), Quaternion.identity);
        showFeedback.transform.GetChild(0).GetComponent<TMP_Text>().text = giveFeedback;
        feedbacks.Add(showFeedback);
        NetworkServer.Spawn(showFeedback, connectionToClient);

        if (hasAuthority)
        {
            //set the canvas as the parent and the card will be a child for this element
            showFeedback.transform.SetParent(discardArea.transform, false);
        }

        if (type == "element question")
        {
            GameObject showButton = Instantiate(feedback2, new Vector3(0, 0, 0), Quaternion.identity);
            feedbacks.Add(showButton);
            NetworkServer.Spawn(showButton, connectionToClient);

            if (hasAuthority)
            {
                //set the canvas as the parent and the card will be a child for this element
                showButton.transform.SetParent(showFeedback.transform.GetChild(0).GetComponent<TMP_Text>().transform, false);
            }
        }

    }

    //Find card in the active cards
    private Cards FindCard(string cardID)
    {
        Cards getCard = ScriptableObject.CreateInstance<Cards>();
        List<Cards> playercards = currentPlayer.GetPlayerCards();
        foreach (Cards findCard in playercards.ToArray())
        {
            if (findCard.GetCardID().ToString() == cardID)
            {
                getCard = findCard;
            }
        }
        return getCard;
    }

    //only check when an element is going to get collected
    [Command]
    public void CmdCheckElements(string cardType)
    {
        int collectAll = 5;
        string getQuestion = "";
        foreach (Elements element in elements)
        {
            if (!element.IsCollected())
            {
                if (element.GetLabel() == cardType)
                {
                    //get question from the server and display it to all the players
                    getQuestion = element.GetQuestion();
                    RpcElementCollected(cardType, getQuestion);
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
    }

    //when all five elements are collected
    void ShowVictory()
    {
        GameObject victory = Instantiate(win, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(victory, connectionToClient);

        if (hasAuthority)
        {
            //set the canvas as the parent and the card will be a child for this element
            victory.transform.SetParent(GameObject.Find("Canvas").transform, false);
        }
    }

    //update elemtn image on all players
    [ClientRpc]
    void RpcElementCollected(string cardType, string getQuestion)
    {
        //show element image when it is collected
        elementsDisplay = GameObject.Find("Elements");
        elementsDisplay.transform.Find(cardType).GetComponent<Image>().enabled = true;
        GiveFeedback(getQuestion, "element question");
    }

    //check if the element is collected
    public bool CheckAElement()
    {
        return collectAElement;
    }

    //call the collectelements function
    public void CollectElement(string cardType)
    {
        CmdCollectElements(cardType);
    }
}
