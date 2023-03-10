using System.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : Menu
{
    [SerializeField] private TreeNetworkManager networkManager;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private bool serverAndClient = false;

    private bool cancel = false;

    public enum AuthResult
    {
        Waiting,
        Success,
        Fail
    }

    private AuthResult authResult = AuthResult.Waiting;

    private void Start()
    {
        cancelButton.onClick.AddListener(() =>
        {
            cancel = true;
        });

        networkManager.authenticator.OnClientAuthenticated.AddListener(() =>
        {
            authResult = AuthResult.Success;
        });

        (networkManager.authenticator as TreeAuthenticator)?.OnClientAuthFailed.AddListener((msg) =>
        {
            authResult = AuthResult.Fail;
            errorText.text = msg;
        });
    }

    public override void Activate()
    {
        base.Activate();
        cancel = false;
        errorText.text = string.Empty;
        cancelButton.gameObject.SetActive(false);
        SetInteractable(true);

        usernameInput.text = TreeAuthenticator.Username;
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public async void Connect()
    {
        var ip = addressInput.text;
        var username = usernameInput.text;

        if (string.IsNullOrEmpty(ip))
        {
            errorText.text = "Address not valid!";
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            errorText.text = "Please enter a username.";
            return;
        }

        TreeAuthenticator.Username = username;

        if (serverAndClient)
        {
            networkManager.StartHost();
            GoToGame();
            return;
        }

        SetInteractable(false);

        cancelButton.gameObject.SetActive(true);

        networkManager.networkAddress = ip;

        cancel = false;
        authResult = AuthResult.Waiting;
        
        networkManager.StartClient();

        //Do nothing until client connects
        while (NetworkClient.isConnecting && !cancel)
        {
            await Task.Yield();
        }

        cancelButton.gameObject.SetActive(false);

        if (NetworkClient.isConnected)
        {
            while (authResult == AuthResult.Waiting)
            {
                await Task.Yield();
            }

            if (authResult == AuthResult.Success)
            {
                //We successfully connected
                GoToGame();
                return;
            }
        }

        SetInteractable(true);

        if (cancel)
        {
            networkManager.StopClient();
            return;
        }

        if (authResult != AuthResult.Fail)
        {
            //We simply failed to connect
            errorText.text = "Failed to connect to host";
        }
    }

    private void GoToGame()
    {
        MenuManager.Instance.Activate("Client");
    }

    private void SetInteractable(bool value)
    {
        addressInput.interactable = value;
        usernameInput.interactable = value;
        connectButton.interactable = value;
        backButton.interactable = value;
    }

}
