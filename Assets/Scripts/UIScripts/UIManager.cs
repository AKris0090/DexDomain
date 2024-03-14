using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;

// edited from mechanics module
enum MENU
{
    Start,
    End,
    HUD,
    AbilityGUI,
    CardSwap,
    Pause,
}

public class UIManager : MonoBehaviour
{
    // referencing from card manager
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }
    void Awake()
    {
        Debug.Log("menu manager exists (UIManager component)");
        if (_instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            _instance = this;
        }
        else if (_instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    public List<Canvas> menus;
    public GameObject feedbackLayer;

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
        // SwapMenu(MENU.Start);
        StartGame();
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
        CanvasGroup holder = menus[(int)M].GetComponent<CanvasGroup>();
        holder.alpha = 1;
        holder.interactable = true;
        holder.blocksRaycasts = true;
    }
    void CloseAllMenus()
    {
        for (int i = 0; i < menus.Count; ++i)
        {
            CanvasGroup holder = menus[i].GetComponent<CanvasGroup>();
            holder.alpha = 0;
            holder.interactable = false;
            holder.blocksRaycasts = false;
        }
    }
    void CloseMenu(MENU M) // SINGLE MENU OP.
    {
        CanvasGroup holder = menus[(int)M].GetComponent<CanvasGroup>();
        holder.alpha = 0;
        holder.interactable = false;
        holder.blocksRaycasts = false;
    }
    void OpenMenu(MENU M) // SINGLE MENU OP.
    {
        CanvasGroup holder = menus[(int)M].GetComponent<CanvasGroup>();
        holder.alpha = 1;
        holder.interactable = true;
        holder.blocksRaycasts = true;
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
    public void EndGame()
    {
        Debug.Log("ending game");
        CloseAllMenus();
        SceneManager.LoadScene("GameOver");
    }

    // Give the player some damage feedback
    public IEnumerator PlayerDamageFeedback(PlayerHealth health)
    {
        CanvasGroup red = feedbackLayer.GetComponent<CanvasGroup>();
        for(int i = 0; i < 2; i++)
        {
            red.alpha = 1;
            yield return new WaitForSeconds(health.iframes / 4);
            red.alpha = 0;
            yield return new WaitForSeconds(health.iframes / 4);
        }
    }
}
