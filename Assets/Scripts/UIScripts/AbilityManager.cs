using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardOperations;

public class AbilityManager : MonoBehaviour
{
    // referencing from card manager
    private static AbilityManager _instance;
    public static AbilityManager Instance { get { return _instance; } }
    void Awake()
    {
        Debug.Log("ability manager exists (the one that deals with UI)");
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


    public List<Canvas> abilitySlots;
    Dictionary<BaseCardClass, GameObject> cardToUI;
    Dictionary<GameObject, BaseCardClass> UIToCard;

    public List<Canvas> deckSlots;
    private int nextSlot = 0;


    void Start()
    {
        cardToUI = new Dictionary<BaseCardClass, GameObject>();
        UIToCard = new Dictionary<GameObject, BaseCardClass>();
    }

    int findNextSlot()
    {
        for(int i = 0; i < 10; i++)
        {
            if (deckSlots[i].transform.childCount == 0)
            {
                return i;
            }
        }
        return 0;
    }

    // add card to swapper page (called from Card Manager)
    public void AddCardToHand(BaseCardClass card)
    {
        Debug.Log("(UI) Adding " + card.cardName + " card to hand");

        // make visible
        nextSlot = findNextSlot();
        GameObject newCard = Instantiate(card.cardUIPrefab, deckSlots[nextSlot].transform, false);

        // make obtainable (can move from hand to equipped)
        if (cardToUI.ContainsKey(card))
        {
            cardToUI[card] = newCard;
        }
        else
        {
            cardToUI.Add(card, newCard);
        }
        Debug.Log(newCard);
        Button cardButton = deckSlots[nextSlot].GetComponentInChildren<Button>();
        cardButton.onClick.AddListener(() => CardManager.Instance.SwapIn(card));
        // basically clicking a card calls the card manager to do it's swapping in
        // then the card manager SwapIn() calls the ability ui manager (this) to update the UI

        // for next
        nextSlot++;
    }

    // also called from Card Manager
    public void EquipCard(BaseCardClass card)
    {
        Debug.Log("(UI) Equipping " + card.cardName);

        Canvas targetCanvas = abilitySlots[(int)card.EquipPlacement];

        // clear canvas if any cards are already there
        if (targetCanvas.transform.childCount > 0)
        {
            foreach (Transform child in targetCanvas.transform)
            {
                CardComponent test = child.GetComponent<CardComponent>();
                if(test != null)
                {
                    AddCardToHand(test.CardScriptObj);
                }
                Destroy(child.gameObject);
            }
        }

        // add to equip slot
        Instantiate(card.cardUIPrefab, targetCanvas.transform, false);
        // using the cards should be done in player control
        Destroy(cardToUI[card]);
    }
}
