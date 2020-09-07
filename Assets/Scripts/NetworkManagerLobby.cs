using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 1;

    [Header("Room")]
    [SerializeField] private PlayerManager PlayerManager = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    //clean up some events
    public static event Action OnServerStopped;

    public List<PlayerManager> playerManagers = new List<PlayerManager>();
    
    public GameObject card;
    public List<Cards> cards = new List<Cards>();
    public List<Elements> elements = new List<Elements>();
    public List<Cards> communications = new List<Cards>();
    public string question;
    public int count = 0;
    public GameObject playerArea;
    public GameObject discardArea;
    public GameObject gameBoard;
    public GameObject endTurn;
    public GameObject feedback;
    public GameObject feedback2;
    public List<GameObject> feedbacks;
    public GameObject elementsDisplay;
    public GameObject communicationDisplay;
    public GameObject win;
    public bool isWin = false;
    public PlayerManager currentPlayer;

    public string RoomID;

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        bool isLeader = playerManagers.Count == 0;

        PlayerManager playerManager = Instantiate(PlayerManager);

        playerManager.IsLeader = isLeader;

        NetworkServer.AddPlayerForConnection(conn, playerManager.gameObject);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var playerManager = conn.identity.GetComponent<PlayerManager>();

            playerManagers.Remove(playerManager);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        playerManagers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var playerManager in playerManagers)
        {
            playerManager.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var playerManager in playerManagers)
        {
            if (!playerManager.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (!IsReadyToStart()) { return ; }
        LoadCards();
        UpdateScene();
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
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

    private void UpdateScene()
    {
        LoadScene();
        PlayRounds();
    }

    public void LoadScene()
    {
        //generate the gameboard from prefab
        GameObject newGameBoard = Instantiate(gameBoard, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(newGameBoard);
        //set the canvas as the parent and the card will be a child for this element
        //newGameBoard.transform.SetParent(GameObject.Find("Canvas").transform, false);
        //get each player area set up
        //int index = 0;
        //while (index < Room.players.Count)
        //{
        //    if (playerNum >= Room.players.Count)
        //    {
        //        playerNum = 0;
        //    }
        //    DisplayPlayer(index, Room.players[playerNum].GetPlayerName());
        //    index++;
        //    playerNum++;
        //}
        playerArea = GameObject.Find("Panel_player1");
        discardArea = GameObject.Find("Panel_discard");
    }

    //start new round, check who the player is
    private void PlayRounds()
    {
        if (count > playerManagers.Count - 1)
        {
            count = 0;
        }
        question = "";
        HaltTask("CheckTurn");
    }

    public void HaltTask(string name)
    {
        Invoke(name, 0.3f);
    }

    public void CheckTurn()
    {
        if (isWin)
        {
            ShowVictory();
        }
        else
        {
            currentPlayer = playerManagers[count];
            ClearCards();
            SetUpCards();
            ClearFeedback();
            EndTurn();
            currentPlayer.StartTurn();
        }
        count++;
    }

    //clear the cards in the playarea
    public void ClearCards()
    {
        foreach (GameObject card in currentPlayer.activeCards)
        {
            Destroy(card);
        }
        currentPlayer.activeCards.Clear();
    }

    //set up cards for the new round
    public void SetUpCards()
    {
        List<Cards> init = currentPlayer.activeCardsData;
        if (init.Count > 0)
        {
            foreach (Cards cards in init)
            {
                AddCardDataToGameObject(cards);
            }
        }
    }

    //add the given drawcard data to the gameobject card
    public void AddCardDataToGameObject(Cards drawCard)
    {
        GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(playerCard);
        currentPlayer.activeCards.Add(playerCard);
        //set the canvas as the parent and the card will be a child for this element
        playerCard.transform.SetParent(playerArea.transform, false);
        playerCard.transform.GetChild(0).GetComponent<TMP_Text>().text = drawCard.GetCardType();
        playerCard.transform.GetChild(1).GetComponent<TMP_Text>().text = drawCard.GetCardID().ToString();
    }

    //create a panel over the player's play area when it's not the player's turn
    public void EndTurn()
    {
        GameObject createEndTurn = Instantiate(endTurn, new Vector3(0, 0, 0), Quaternion.identity);
        feedbacks.Add(createEndTurn);
        NetworkServer.Spawn(createEndTurn);
        //set the canvas as the parent and the card will be a child for this element
        createEndTurn.transform.SetParent(GameObject.Find("Canvas_Game(Clone)").transform, false);
    }

    //when all five elements are collected
    private void ShowVictory()
    {
        GameObject victory = Instantiate(win, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(victory);
        //set the canvas as the parent and the card will be a child for this element
        victory.transform.SetParent(GameObject.Find("Canvas_Game(Clone)").transform, false);
    }

    public void ClearFeedback()
    {
        foreach (GameObject feedback in feedbacks) { Destroy(feedback); }
        feedbacks.Clear();
    }

    //the function that shows the feedback
    public void GiveFeedback(string giveFeedback, string type)
    {
        GameObject showFeedback = Instantiate(feedback, new Vector3(0, 0, 0), Quaternion.identity);
        showFeedback.transform.GetChild(0).GetComponent<TMP_Text>().text = giveFeedback;
        feedbacks.Add(showFeedback);
        NetworkServer.Spawn(showFeedback);
        //set the canvas as the parent and the card will be a child for this element
        showFeedback.transform.SetParent(discardArea.transform, false);

        if (type == "element question")
        {
            GameObject showButton = Instantiate(feedback2, new Vector3(0, 0, 0), Quaternion.identity);
            feedbacks.Add(showButton);
            NetworkServer.Spawn(showButton);
            //set the canvas as the parent and the card will be a child for this element
            showButton.transform.SetParent(showFeedback.transform.GetChild(0).GetComponent<TMP_Text>().transform, false);
        }
    }

    //update elemtn image on all players
    public void ElementCollected(string cardType, string getQuestion)
    {
        //show element image when it is collected
        elementsDisplay = GameObject.Find("Elements");
        elementsDisplay.transform.Find(cardType).GetComponent<Image>().enabled = true;
        GiveFeedback(getQuestion, "element question");
    }
}
