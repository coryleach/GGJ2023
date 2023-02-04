using UnityEngine;
using UnityEngine.EventSystems;

public class TreeContainer : MonoBehaviour, IPointerClickHandler
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
    }
}
