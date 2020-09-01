using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Players : ScriptableObject
{
    private string playerName;
    private List<Cards> playerCards;
    private int actions;
    private bool endTurn;
    private bool isReady;

    public void Init(string inputName)
    {
        playerName = inputName;
        playerCards = new List<Cards>();
        actions = 4;
        isReady = false;
    }

    public static new Players CreateInstance(string inputName)
    {
        var data = ScriptableObject.CreateInstance<Players>();
        data.Init(inputName);
        return data;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public List<Cards> GetPlayerCards()
    {
        return playerCards;
    }

    public void AddPlayerCards(Cards card)
    {
        playerCards.Add(card);
        playerCards.Sort();
    }

    public void RemovePlayerCards(Cards card)
    {
        playerCards.Remove(card);
    }

    public void DecreaseActions()
    {
        actions--;
    }

    public void ResetActions()
    {
        actions = 4;
    }

    public int GetActions()
    {
        return actions;
    }

    public bool IsReady()
    {
        return isReady;
    }

    public void SetReady()
    {
        isReady = true;
    }

    public void NotReady()
    {
        isReady = false;
    }
}