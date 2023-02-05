using UnityEngine;
using Mirror;

public class TreeNetworkManager : NetworkManager
{
    #region OnServer

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerAddPlayer");
        // add player at correct spawn position
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        var username = (string)conn.authenticationData;
        Debug.Log($"OnServerDisconnect {username}");
        ServerGameManager.Instance.CleanupPlayer(username);
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        Debug.Log("OnServerReady");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        Debug.Log("OnServerSceneChanged");
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        Debug.Log("OnServerChangeScene");
    }

    #endregion

    #region OnClient

    public override void OnClientNotReady()
    {
        base.OnClientNotReady();
        Debug.Log("OnClientNotReady");
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        Debug.Log("OnClientChangeScene");
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        Debug.Log("OnClientSceneChanged");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.Log("OnClientConnect");

        if (!NetworkClient.ready)
        {
            NetworkClient.Ready();
        }

        if (NetworkClient.localPlayer == null)
        {
            NetworkClient.AddPlayer();
        }
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("OnClientDisconnect");
    }

    public override void OnClientError(TransportError error, string reason)
    {
        base.OnClientError(error, reason);
        Debug.Log("OnClientError");
    }

    #endregion
}
