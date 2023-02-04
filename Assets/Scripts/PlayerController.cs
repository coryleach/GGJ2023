using System;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private static PlayerController localPlayer;
    public static PlayerController LocalPlayer => localPlayer;

    private void Start()
    {
        if (this.isLocalPlayer)
        {
            localPlayer = this;
        }
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

}
