using UnityEngine;
using UnityEngine.EventSystems;

public class ClearSelectionClicker : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameCursor.Instance.Select(null);
    }
}
