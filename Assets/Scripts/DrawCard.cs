using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DrawCard : NetworkBehaviour
{
    public PlayerManager playerManager;

    public void OnClick()
    {
        NetworkIdentity netID = NetworkClient.connection.identity;
        playerManager = netID.GetComponent<PlayerManager>();

        playerManager.CmdDrawCardData();

    }
}
