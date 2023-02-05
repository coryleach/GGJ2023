using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    [SerializeField] private Animation panelAnimation;

    [SerializeField] private GameObject content;

    [SerializeField] private GameObject buttonsGroup;

    [SerializeField] private TMP_Text towerInfo;

    [SerializeField] private Button buildButton;

    private TreeContainer _treeContainer = null;

    private void Awake()
    {
        content.gameObject.SetActive(false);
        buildButton.onClick.AddListener(Build);
    }

    private void Start()
    {
        GameCursor.Instance.SelectionChanged.AddListener(() =>
        {
            Refresh(GameCursor.Instance.Selected);
        });
    }

    public void Refresh(TreeContainer treeContainer)
    {
        _treeContainer = treeContainer;

        if (treeContainer == null)
        {
            panelAnimation.Play("InfoPanelHide");
            return;
        }

        content.SetActive(true);
        panelAnimation.Play("InfoPanelShow");

        if (treeContainer.Current == null)
        {
            towerInfo.text = $"Empty Plot {treeContainer.Slot+1}";
        }
        else
        {
            towerInfo.text = $"{treeContainer.Current.Owner}'s Tree";
        }

        var localPlayer = PlayerController.localPlayerController;
        buildButton.gameObject.SetActive(treeContainer.Current == null);
        buildButton.interactable = !TreeContainer.PlayerOwnsAnyContainer(localPlayer.Username);
    }

    private void Build()
    {
        if (_treeContainer.Build())
        {
            GameCursor.Instance.Select(null);
        }
    }

}
