using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private static PlayerController _localPlayerController;
    public static PlayerController localPlayerController => _localPlayerController;

    [SyncVar, SerializeField] private string username;

    public string Username => username;

    [SerializeField] private PlayerData _data = new PlayerData();
    public PlayerData Data => _data;

    private void Start()
    {
        if (isLocalPlayer)
        {
            _localPlayerController = this;
        }
        else if (isServer)
        {
            var playerName = (string)connectionToClient.authenticationData;
            //Get/Create Data for this player on the server
            username = playerName;
            _data = ServerGameManager.Instance.GetPlayerData(playerName);
            ServerGameManager.Instance.Players.Add(username,this);
        }
    }

    [ServerCallback]
    private void OnDisable()
    {
        ServerGameManager.Instance.Players.Remove(username);
    }

    /// <summary>
    /// Should be called by the client and run on the server
    /// </summary>
    [Command]
    public void SpawnTree(TreeContainer treeContainer)
    {
        Debug.Log("Spawn Tree Server");
        treeContainer.Spawn(this);
    }

    [Command]
    public void SetUsername(string username)
    {
        this.username = username;
    }
}
