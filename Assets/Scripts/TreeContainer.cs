using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class TreeContainer : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private RootsController rootsPrefab;

    private RootsController currentRootsController;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click!");
        if (currentRootsController == null)
        {
            Spawn();
        }
    }

 
    private void Spawn()
    {
        currentRootsController = Instantiate(rootsPrefab, transform);
        currentRootsController.transform.localPosition = Vector3.zero;
        NetworkServer.Spawn(currentRootsController.gameObject);
    }
}
