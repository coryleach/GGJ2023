using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private Dictionary<string, Menu> _menus = new Dictionary<string, Menu>();

    private Menu currentlyActiveMenu = null;

    public Menu startMenu = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var menus = GetComponentsInChildren<Menu>(true);

        foreach (var menu in menus)
        {
            _menus.Add(menu.name, menu);
            menu.gameObject.SetActive(false);
        }

        if (startMenu != null)
        {
            currentlyActiveMenu = startMenu;
            startMenu.Activate();
        }
    }

    public void Activate(string menuName)
    {
        if (!_menus.TryGetValue(menuName, out var menu))
        {
            Debug.LogError($"Menu {menuName} not found!");
            return;
        }

        if (currentlyActiveMenu != null)
        {
            currentlyActiveMenu.Deactivate();
        }

        currentlyActiveMenu = menu;
        currentlyActiveMenu.Activate();
    }

}
