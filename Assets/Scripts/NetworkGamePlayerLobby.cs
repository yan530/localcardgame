using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
    private string displayName = "Loading...";
    private Dictionary<GameObject, Cards> playerCards = new Dictionary<GameObject, Cards>();
    private bool traveling = false;
    private int actions = 4;

    public TMP_Text playername;
    public GameObject card;
    public GameObject PlayerArea;
    public GameObject IslandArea;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
        playername.text = displayName;
    }

    public void DrawCardData()
    {
        actions--;
        Cards drawCard = Room.cards[UnityEngine.Random.Range(0, Room.cards.Count - 1)];

        GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
        playerCards.Add(playerCard, drawCard);
        Debug.Log(playerCards.Count);
        Room.cards.Remove(drawCard);


        //set the canvas as the parent and the card will be a child for this element
        playerCard.transform.SetParent(PlayerArea.transform, false);
        playerCard.transform.GetChild(0).GetComponent<TMP_Text>().text = drawCard.GetCardType();
        //Debug.Log(actions);
    }

    public void PlayCard()
    {
        actions--;
        Debug.Log(playerCards.Count);
        //Debug.Log(actions);
        foreach (GameObject key in playerCards.Keys)
        {
            if (!key.activeSelf)
            {
                Cards played = playerCards[key];
                Debug.Log(played.GetType());
                break;
            }
        }
    }
}
