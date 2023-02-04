using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class TreeContainer : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private RootsController rootsPrefab;

    private RootsController currentRootsController;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Don't call this unless this object is on the client
        if (!isClient)
        {
            return;
        }

        Debug.Log("Client Click!");

        if (PlayerController.localPlayerController != null)
        {
            PlayerController.localPlayerController.SpawnTree(this);
        }
    }

    [Server]
    public void Spawn(PlayerController owner)
    {
        //We need to check for this here now too because this code should run on server while command was executed from client
        if (currentRootsController != null)
        {
            Debug.Log("TreeContainer: Tree was already spawned here!");
            return;
        }

        Debug.Log("TreeContainer: Spawning!");
        currentRootsController = Instantiate(rootsPrefab);
        currentRootsController.transform.position = transform.position;
        NetworkServer.Spawn(currentRootsController.gameObject);
    }
}
