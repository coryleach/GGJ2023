using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[SerializeField]
public class PlayerData
{
    public string username;
    public int money;
    public int xp;
}

[Serializable]
public class GameData : ISerializationCallbackReceiver
{
    /// <summary>
    /// This dictionary contains the player's data while the game is running and is not serialized
    /// </summary>
    [NonSerialized]
    public Dictionary<string, PlayerData> playerData = new Dictionary<string, PlayerData>();

    /// <summary>
    /// This field is for serialization only. Not intended to be used directly during gameplay
    /// </summary>
    [SerializeField] private List<PlayerData> _serializedPlayerDataList = new List<PlayerData>();

    public void OnBeforeSerialize()
    {
        _serializedPlayerDataList.Clear();
        foreach (var pair in playerData)
        {
            _serializedPlayerDataList.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        playerData.Clear();
        foreach ( var player in _serializedPlayerDataList)
        {
            playerData[player.username] = player;
        }
    }
}

public class ServerGameManager : NetworkBehaviour
{
    private static ServerGameManager _instance = null;
    public static ServerGameManager Instance => _instance;

    private GameData _gameData = new GameData();

    [SerializeField]
    private bool saveAndLoad = false;

    private HashSet<string> activePlayerNames = new HashSet<string>();
    public HashSet<string> ActivePlayerNames => activePlayerNames;

    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    public Dictionary<string, PlayerController> Players => _players;

    public List<RootsController> Trees = new List<RootsController>();

    public PlayerData GetPlayerData(string username)
    {
        if (_gameData.playerData.TryGetValue(username, out var playerData))
        {
            return playerData;
        }

        playerData = new PlayerData
        {
            username = username,
        };

        _gameData.playerData[username] = playerData;

        return playerData;
    }


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        if (saveAndLoad)
        {
            Load();
        }
    }

    private void OnApplicationQuit()
    {
        if (saveAndLoad)
        {
            Save();
        }
    }

    public void CleanupPlayer(string username)
    {
        ActivePlayerNames.Remove(username);
        for(int i = Trees.Count-1; i>= 0; i--)
        {
            if(Trees[i].Owner == username)
            {
                RootsController tree = Trees[i];
                Trees.Remove(tree);
                NetworkServer.Destroy(tree.gameObject);
            }
        }
    }

    public void RegisterTree(RootsController tree)
    {
        Trees.Add(tree);
    }

    private void Save()
    {
        PlayerPrefs.SetString("GGJ2023_SaveData", JsonUtility.ToJson(_gameData));
    }

    private void Load()
    {
        var json = PlayerPrefs.GetString("GGJ2023_SaveData");
        if (!string.IsNullOrEmpty(json))
        {
            _gameData = JsonUtility.FromJson<GameData>(json);
        }

        if (_gameData == null)
        {
            _gameData = new GameData();
        }
    }

}
