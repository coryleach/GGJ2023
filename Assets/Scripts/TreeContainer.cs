using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class TreeContainer : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private RootsController rootsPrefab;

    [SerializeField]
    private int slot; //This should be unique for each tree container
    public int Slot => slot;

    private RootsController currentRootsController;
    public RootsController Current
    {
        get => currentRootsController;
        set
        {
            if (currentRootsController != null && currentRootsController != value)
            {
                Debug.LogError($"TreeContainer {slot} is already assigned a roots controller");
            }
            currentRootsController = value;
        }
    }

    [SerializeField]
    private GameObject CircleObject;

    [SerializeField]
    private EnemySpawner spawner;

    [SerializeField]
    private AudioSource Audio;

    private static readonly List<TreeContainer> InstanceCollection = new List<TreeContainer>();

    public static TreeContainer GetSlot(int slot)
    {
        return InstanceCollection.FirstOrDefault(x => x.slot == slot);
    }

    public static TreeContainer GetContainerForPlayer(string username)
    {
        return InstanceCollection.Where(x => x.Current != null).FirstOrDefault(x => x.Current.Owner == username);
    }

    public static bool PlayerOwnsAnyContainer(string username)
    {
        return GetContainerForPlayer(username) != null;
    }

    private void Awake()
    {
        InstanceCollection.Add(this);
    }

    private void OnDestroy()
    {
        InstanceCollection.Remove(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Don't call this unless this object is on the client
        if (!isClient)
        {
            return;
        }

        GameCursor.Instance.Select(this);
    }

    [Client]
    public bool Build()
    {
        var localPlayer = PlayerController.localPlayerController;
        if (localPlayer == null)
        {
            return false;
        }

        if (PlayerOwnsAnyContainer(localPlayer.Username))
        {
            return false;
        }

        localPlayer.SpawnTree(this);

        var rend = gameObject.GetComponent<SpriteRenderer>();
        if (CircleObject != null)
        {
            CircleObject.SetActive(false);
        }

        return true;
    }

    [Server]
    public void Spawn(PlayerController player)
    {
        //We need to check for this here now too because this code should run on server while command was executed from client
        if (currentRootsController != null)
        {
            Debug.Log("TreeContainer: Tree was already spawned here!");
            return;
        }

        if (PlayerOwnsAnyContainer(player.Username))
        {
            return;
        }

        Debug.Log($"TreeContainer: Spawning for player {player.Data.username}!");
        currentRootsController = Instantiate(rootsPrefab);
        currentRootsController.transform.position = transform.position;
        currentRootsController.SetSlot(slot);
        currentRootsController.SetPlayer(player);
        NetworkServer.Spawn(currentRootsController.gameObject);
        spawner.RegisterTree(currentRootsController);
        ServerGameManager.Instance.RegisterTree(currentRootsController);
    }
}
