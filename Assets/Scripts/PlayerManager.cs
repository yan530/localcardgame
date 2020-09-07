using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerManager : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    //HUD
    public List<GameObject> activeCards = new List<GameObject>();
    public List<Cards> activeCardsData = new List<Cards>();
    public bool collectAElement;

    //instruction
    //public string[] sentence;
    //public TMP_Text textHeader;
    //public TMP_Text textInstruction;
    //public TMP_Text textInstruction2;
    //public TMP_Text textInstruction3;

    //debug
    private int playerNum;

    //network room player

    public static event Action onClientConnected;
    public static event Action onClientDisconnected;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.playerManagers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.playerManagers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var playerManager in Room.playerManagers)
            {
                if (playerManager.hasAuthority)
                {
                    playerManager.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.playerManagers.Count; i++)
        {
            playerNameTexts[i].text = Room.playerManagers[i].DisplayName;
            playerReadyTexts[i].text = Room.playerManagers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.playerManagers[0].connectionToClient != connectionToClient) { return; }
        Room.StartGame();
    }

    //display player
    private void DisplayPlayer(int i, string name)
    {
        //change player name on UI
    }


    public void HaltTask(string name)
    {
        Invoke(name, 0.3f);
    }

    public void StartTurn()
    {
        Room.ClearFeedback();
    }

    //get a card data from the cards list
    [Command]
    public void CmdDrawCardData()
    {
        Room.ClearFeedback();
        if (Room.cards == null || Room.cards.Count == 0)
        {
            Debug.Log("No cards left");
        }

        //check if an card has been exchanged and moved to the top of the deck
        Cards drawCard;
        drawCard = Room.cards[UnityEngine.Random.Range(0, Room.cards.Count - 1)];
        activeCardsData.Add(drawCard);
        activeCardsData.Sort();
        Room.cards.Remove(drawCard);

        //hud
        if (drawCard == null)
        {
            Debug.Log("Out of Cards");
        }
        else
        {
            Room.AddCardDataToGameObject(drawCard);
            Room.HaltTask("PlayRounds");
            Room.EndTurn();
        }
    }

    //check if the player collected enough element cards of the same kind
    [Command]
    public void CmdCollectElements(string cardType)
    {
        Room.ClearFeedback();
        collectAElement = true;
        //hud
        Cards[] playerCards = activeCardsData.ToArray();
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
                activeCardsData.Remove(cards);
            }
            CmdCheckElements(cardType);
            Room.EndTurn();
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
        foreach (Elements element in Room.elements)
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
        Room.GiveFeedback(giveFeedback, "feedback");
    }

    //Find card in the active cards
    private Cards FindCard(string cardID)
    {
        Cards getCard = ScriptableObject.CreateInstance<Cards>();
        foreach (Cards findCard in activeCardsData.ToArray())
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
        foreach (Elements element in Room.elements)
        {
            if (!element.IsCollected())
            {
                if (element.GetLabel() == cardType)
                {
                    //get question from the server and display it to all the players
                    getQuestion = element.GetQuestion();
                    Room.ElementCollected(cardType, getQuestion);
                }
                else
                {
                    collectAll--;
                }

            }
        }
        if (collectAll == 5)
        {
            Room.isWin = true;
        }
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
