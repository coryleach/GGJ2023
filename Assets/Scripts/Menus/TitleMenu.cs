using UnityEngine;

public class TitleMenu : Menu
{
    [SerializeField] public TreeNetworkManager manager;

    public void Client()
    {
        MenuManager.Instance.Activate("Login");
    }

    public void ServerAndClient()
    {
        MenuManager.Instance.Activate("HostLogin");
    }

    public void Server()
    {
        //Start Server
        manager.StartServer();
        MenuManager.Instance.Activate("Server");
    }
}
