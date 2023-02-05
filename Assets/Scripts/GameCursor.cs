using UnityEngine;
using UnityEngine.Events;

public class GameCursor : MonoBehaviour
{
    public static GameCursor Instance { get; private set; }

    [SerializeField]
    private GameObject pivot;

    private TreeContainer selectedContainer = null;
    public TreeContainer Selected => selectedContainer;

    public UnityEvent SelectionChanged { get; } = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();
    }

    private void Show()
    {
        pivot.gameObject.SetActive(true);
    }

    private void Hide()
    {
        pivot.gameObject.SetActive(false);
    }

    public void Select(TreeContainer container)
    {
        if (container == selectedContainer)
        {
            return;
        }

        selectedContainer = container;

        if (selectedContainer != null)
        {
            Show();
            pivot.transform.position = container.transform.position;
        }
        else
        {
            Hide();
        }

        SelectionChanged.Invoke();
    }

}
