using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// edited from mechanics module
enum MENU
{
    Start = 0,
    End = 1,
    HUD = 2,
    AbilityGUI = 3,
    CardSwap = 4,
}

public class UIManager : MonoBehaviour
{
    public List<Canvas> Menus;

    // dirties
    public bool GamePlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        // snap all canvases to view
        for (int i = 0; i < Menus.Count; ++i)
        {
            Menus[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        // start game
        SwapMenu(MENU.Start);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Menu Changing Functions
    void SwapMenu(MENU M) // USE IF YOU ONLY NEED ONE MENU OPEN
    {
        CloseAllMenus();
        Menus[(int)M].enabled = true;
    }
    void CloseAllMenus()
    {
        for (int i = 0; i < Menus.Count; ++i)
        {
            Menus[i].enabled = false;
        }
    }
    void CloseMenu(MENU M) // SINGLE MENU OP.
    {
        Menus[(int)M].enabled = false;
    }
    void OpenMenu(MENU M) // SINGLE MENU OP.
    {
        Menus[(int)M].enabled = true;
    }


    // Functions for Buttons
    public void StartGame()
    {
        CloseAllMenus();
        OpenMenu(MENU.HUD);
        OpenMenu(MENU.AbilityGUI);
        GamePlaying = true;
        // other functions to start game
    }
    public void RestartGame()
    {
        CloseAllMenus();
        // call functions to reset the game state
        // Opening HUD?

    }
}
