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

    public List<Canvas> abilitySlots;

    public List<Canvas> deckSlots;
    private int nextSlot = 0;

    public List<GameObject> activeCards = new();


    void Start()
    {
    }

    // add card to swapper page (called from Card Manager)
    public void AddCardToHand(BaseCardClass card)
    {
        // make visible
        GameObject newCard = Instantiate(card.cardUIPrefab, deckSlots[nextSlot].transform, false);

        // make obtainable (can move from hand to equipped)
        Button cardButton = deckSlots[nextSlot].GetComponentInChildren<Button>();
        cardButton.onClick.AddListener(() => EquipCard(card));

        // for next
        nextSlot++;
    }

    // also called from Card Manager
    private void EquipCard(BaseCardClass card)
    {

        // TODO find a way to get equip slots available to use :(
        //Canvas targetCanvas = abilitySlots[deckSetup[b.EquipSlot]];
        Canvas targetCanvas = abilitySlots[0];
        if (targetCanvas.transform.childCount > 0)
        {
            foreach (Transform child in targetCanvas.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // add to equip slot
        _ = Instantiate(card, targetCanvas.transform, false);
        // using the cards should be done in player control
    }
}
