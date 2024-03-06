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
    Pause = 5,
}

public class UIManager : MonoBehaviour
{
    // referencing from card manager
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    public List<Canvas> menus;

    // dirties
    public bool gamePlaying = false;
    public bool cardSwapping = false;

    // keybindings
    KeyCode PAUSEGAME_K = KeyCode.P;
    KeyCode SWAPCARDPAGE_K = KeyCode.T;
    KeyCode ENDGAME_K = KeyCode.O;

    // Start is called before the first frame update
    void Start()
    {
        // snap all canvases to view
        for (int i = 0; i < menus.Count; ++i)
        {
            menus[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        // start game
        SwapMenu(MENU.Start);
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePlaying && Input.GetKeyDown(PAUSEGAME_K))
        {
            // pause menu
            OpenMenu(MENU.Pause);
            gamePlaying = false;
        }
        if (gamePlaying && Input.GetKeyDown(SWAPCARDPAGE_K))
        {
            if (cardSwapping)
            {
                Debug.Log("finished swapping hand");
                CloseMenu(MENU.CardSwap);
                cardSwapping = false;
            }
            else
            {
                Debug.Log("swapping hand");
                OpenMenu(MENU.CardSwap);
                cardSwapping = true;
            }
        }

        // for testing
        if (Input.GetKeyDown(ENDGAME_K))
        {
            EndGame();
            gamePlaying = false;
        }
    }

    // Menu Changing Functions
    void SwapMenu(MENU M) // USE IF YOU ONLY NEED ONE MENU OPEN
    {
        CloseAllMenus();
        menus[(int)M].enabled = true;
    }
    void CloseAllMenus()
    {
        for (int i = 0; i < menus.Count; ++i)
        {
            menus[i].enabled = false;
        }
    }
    void CloseMenu(MENU M) // SINGLE MENU OP.
    {
        menus[(int)M].enabled = false;
    }
    void OpenMenu(MENU M) // SINGLE MENU OP.
    {
        menus[(int)M].enabled = true;
    }


    // Functions for Buttons
    public void StartGame()
    {
        Debug.Log("starting game");
        CloseAllMenus();
        OpenMenu(MENU.HUD);
        OpenMenu(MENU.AbilityGUI);
        gamePlaying = true;
        // other functions to start game
    }
    public void RestartGame()
    {
        Debug.Log("restarting game");
        // call functions to reset the game state
        StartGame();
        gamePlaying = true;
    }
    public void ReturnToMenu()
    {
        Debug.Log("returning to menu");
        CloseAllMenus();
        OpenMenu(MENU.Start);
        gamePlaying = false;
    }

    // other things
    void EndGame()
    {
        Debug.Log("ending game");
        CloseAllMenus();
        OpenMenu(MENU.End);
    }
}
