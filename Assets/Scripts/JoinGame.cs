using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class JoinGame : NetworkBehaviour
{
    public PlayerManager playerManager;
    public TMP_InputField player;


    public void OnClick()
    {
        NetworkIdentity netID = NetworkClient.connection.identity;
        playerManager = netID.GetComponent<PlayerManager>();
        playerManager.loadScene(player.text);
    }

}
