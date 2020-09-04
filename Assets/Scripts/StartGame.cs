using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartGame : NetworkBehaviour
{
    public PlayerManager playerManager;

    public void OnClick()
    {
        NetworkIdentity netID = NetworkClient.connection.identity;
        playerManager = netID.GetComponent<PlayerManager>();
        //need to fix user login
        playerManager.LoadScene();
    }
}
