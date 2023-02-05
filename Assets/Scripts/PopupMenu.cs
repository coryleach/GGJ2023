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

    [SerializeField] private Button[] upgradeButtons;

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
        if (_treeContainer != null)
        {
            _treeContainer.OnContainerChanged.RemoveListener(RefreshCurrent);
            if (_treeContainer.Current != null)
            {
                _treeContainer.Current.OnValueChanged.RemoveListener(RefreshCurrent);
            }
        }


        _treeContainer = treeContainer;

        if (_treeContainer != null)
        {
            _treeContainer.OnContainerChanged.AddListener(RefreshCurrent);
            if (_treeContainer.Current != null)
            {
                _treeContainer.Current.OnValueChanged.AddListener(RefreshCurrent);
            }
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

        var rootsController = _treeContainer.Current;
        var localPlayer = PlayerController.localPlayerController;
        var isMine = (rootsController != null) && (rootsController.Owner == localPlayer.Username);
        var level = (rootsController != null) ? rootsController.Level : 0;
        buildButton.gameObject.SetActive(rootsController == null);
        buildButton.interactable = !TreeContainer.PlayerOwnsAnyContainer(localPlayer.Username);

        killsLabel.transform.parent.gameObject.SetActive(_treeContainer.Current != null);
        killsLabel.text = (_treeContainer.Current != null) ? $"{_treeContainer.Current.Kills}" : "";

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            var upgradeBtn = upgradeButtons[i];
            if (upgradeBtn == null)
            {
                continue;
            }

            upgradeBtn.gameObject.SetActive(isMine && level <= 2);
            upgradeBtn.interactable = level == 2;
        }
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

    /// <summary>
    /// Sends the upgrade command to the server from the local player
    /// </summary>
    /// <param name="index"></param>
    public void Upgrade(int index)
    {
        PlayerController.localPlayerController.UpgradeTree(index);
    }
}
