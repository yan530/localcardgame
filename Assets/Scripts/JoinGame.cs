using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class JoinGame : NetworkBehaviour
{
    public PlayerManager playerManager;
    public TMP_InputField player;


    public void OnClick()
    {
        NetworkIdentity netID = NetworkClient.connection.identity;
        playerManager = netID.GetComponent<PlayerManager>();
        //need to fix user login
        //playerManager.LoadScene();
    }

}
