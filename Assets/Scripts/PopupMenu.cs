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

    [SerializeField] private TMP_Text killsLabel;

    private TreeContainer _treeContainer = null;

    private void Awake()
    {
        content.gameObject.SetActive(false);
        buildButton.onClick.AddListener(Build);
    }

    private void Start()
    {
        GameCursor.Instance.SelectionChanged.AddListener(() => { RefreshWithContainer(GameCursor.Instance.Selected); });
    }

    private void SetCurrentTreeContainer(TreeContainer treeContainer)
    {
        if (_treeContainer != null && _treeContainer.Current != null)
        {
            _treeContainer.Current.OnValueChanged.RemoveListener(RefreshCurrent);
        }

        _treeContainer = treeContainer;

        if (_treeContainer != null && _treeContainer.Current != null)
        {
            _treeContainer.Current.OnValueChanged.AddListener(RefreshCurrent);
        }
    }

    private void RefreshCurrent()
    {
        Refresh(false);
    }

    private void Refresh(bool animate = true)
    {
        if (_treeContainer == null)
        {
            panelAnimation.Play("InfoPanelHide");
            return;
        }

        content.SetActive(true);

        if (animate)
        {
            panelAnimation.Play("InfoPanelShow");
        }

        if (_treeContainer.Current == null)
        {
            towerInfo.text = $"Empty Plot {_treeContainer.Slot + 1}";
        }
        else
        {
            towerInfo.text = $"{_treeContainer.Current.Owner}'s Tree";
        }

        var localPlayer = PlayerController.localPlayerController;
        buildButton.gameObject.SetActive(_treeContainer.Current == null);
        buildButton.interactable = !TreeContainer.PlayerOwnsAnyContainer(localPlayer.Username);

        killsLabel.transform.parent.gameObject.SetActive(_treeContainer.Current != null);
        killsLabel.text = (_treeContainer.Current != null) ? $"{_treeContainer.Current.Kills}" : "";
    }

    public void RefreshWithContainer(TreeContainer treeContainer)
    {
        SetCurrentTreeContainer(treeContainer);
        Refresh(true);
    }

    private void Build()
    {
        if (_treeContainer.Build())
        {
            GameCursor.Instance.Select(null);
        }
    }
}
