using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;


public enum GameState { START, PLAYERTURN, ENDTURN, WON }
public class GameManager : MonoBehaviour
{
    //set to private later
    public Cards topOfTheDeck;
    public List<Players> players;
    public List<Cards> cards;
    public List<Elements> elements;
    public List<Cards> cultures;
    public Players currentPlayer;
    public GameState state;
    public string question;
    public bool isWin;

    //debug
    public int index;
    public string playerName;



    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        players = new List<Players>();
        cards = new List<Cards>();
        elements = new List<Elements>();
        state = GameState.START;
        index = 0;
        topOfTheDeck = null;
        question = "";
        isWin = false;
    }

    //start the game, load new scene, and add players to the list
    public void StartGame(List<string> names)
    {
        foreach (string playerName in names)
        {
            players.Add(Players.CreateInstance(playerName));
        }
        SceneManager.LoadScene("Scene_GameBoard");
        LoadCards();
        PlayRounds();
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
    public void PlayRounds()
    {
        state = GameState.PLAYERTURN;
        currentPlayer = players[index];
        playerName = currentPlayer.GetPlayerName();
        question = "";
    }

    //get a card data from the cards list
    public Cards DrawCardData()
    {
        if (cards == null || cards.Count == 0)
        {
            return null;
        }
        //currentPlayer.DecreaseActions();

        //check if an card has been exchanged and moved to the top of the deck
        Cards drawCard;
        if (topOfTheDeck == null)
        {
            drawCard = cards[UnityEngine.Random.Range(0, cards.Count - 1)];
        } else
        {
            drawCard = topOfTheDeck;
            topOfTheDeck = null;
        }
        currentPlayer.AddPlayerCards(drawCard);
        cards.Remove(drawCard);
        return drawCard;
    }

    //move to the next player in the list
    public void ChangeIndex()
    {
        state = GameState.ENDTURN;
        if (index < players.Count - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }
        //currentPlayer.ResetActions();
    }

    public void PlayCard(string cardID, string type)
    {
        //currentPlayer.DecreaseActions();
        Cards card = FindCard(cardID);
        if (type == "Culture")
        {
            if (cultures.Count >= 6)
            {
                cultures.RemoveAt(0);
            }
            cultures.Add(card);
        } else if (type == "Traveler")
        {
            //implementation needed
            currentPlayer.RemovePlayerCards(card);
        } else
        {
            int collectAll = 5;
            foreach (Elements element in elements)
            {
                if (!element.IsCollected())
                {
                    if (element.GetLabel() == type)
                    {
                        question = element.GetQuestion();
                    } else
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
    }

    public Cards FindCard(string cardID)
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
}
