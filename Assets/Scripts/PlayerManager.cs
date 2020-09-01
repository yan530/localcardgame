using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    Players player;
    public override void OnStartServer()
    {
    }

    public override void OnStartClient()
    {
        
    }

    public void loadScene(string playerName)
    {
        SceneManager.LoadScene("Scene_GameBoard");
        player = Players.CreateInstance(playerName);
    } 

    //get a card data from the cards list
    //public Cards DrawCardData()
    //{
    //    if (cards == null || cards.Count == 0)
    //    {
    //        return null;
    //    }
    //    currentPlayer.DecreaseActions();

    //    //check if an card has been exchanged and moved to the top of the deck
    //    Cards drawCard;
    //    if (topOfTheDeck == null)
    //    {
    //        drawCard = cards[UnityEngine.Random.Range(0, cards.Count - 1)];
    //    }
    //    else
    //    {
    //        drawCard = topOfTheDeck;
    //        topOfTheDeck = null;
    //    }
    //    currentPlayer.AddPlayerCards(drawCard);
    //    cards.Remove(drawCard);
    //    return drawCard;
    //}
}
