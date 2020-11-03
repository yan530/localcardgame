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
        foreach (PlayerManager playerManager in playerManagers)
        {
            playerManager.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        Debug.Log(numPlayers);
        if (numPlayers == 0) { return false; }

        //foreach (var playerManager in playerManagers)
        //{
        //    if (!playerManager.IsReady) { return false; }
        //}

        return true;
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

    public void StartGame()
    {
        if (!IsReadyToStart()) { return; }
        LoadCards();
        PlayRounds();
    }

    //start new round, check who the player is
    public void PlayRounds()
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
            Debug.Log("the current player number is:" + count);
            currentPlayer.StartTurn();
        }
        count++;
    }

    public void EndTurn()
    {
        PlayRounds();
    }

    //when all five elements are collected
    private void ShowVictory()
    {
        //show victory in all players
    }

    //update elemtn image on all players
    public void ElementCollected(string cardType, string getQuestion)
    {
        //update element image in all players when it is collected
     
    }
}
