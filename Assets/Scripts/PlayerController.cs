using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private static PlayerController _localPlayerController;
    public static PlayerController localPlayerController => _localPlayerController;

    private void Start()
    {
        if (this.isLocalPlayer)
        {
            _localPlayerController = this;
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
