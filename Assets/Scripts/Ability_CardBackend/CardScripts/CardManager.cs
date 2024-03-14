using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardOperations;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal.Internal;

public class CardManager : MonoBehaviour
{
    
    public BaseCardClass[] equippedCards = new BaseCardClass[4];
    public List<BaseCardClass> hand; // unequipped cards here.


    // using the singleton model I used in the gameplay module
    // ref: https://medium.com/nerd-for-tech/implementing-a-game-manager-using-the-singleton-pattern-unity-eb614b9b1a74
    private static CardManager _instance;
    public static CardManager Instance { get { return _instance; } }

    void Awake()
    {
        //ScoreText = GameObject.Find("ScoreTxt").GetComponent<TextMeshProUGUI>();
        //GameObject.Find("HealthTxt").GetComponent<TextMeshProUGUI>();
        Debug.Log("game manager exists");
        if (_instance == null) // If there is no instance already
        {
            _instance = this;
        }
        else if (_instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        equippedCards = new BaseCardClass[4];
        CardTransferer transferer = new CardTransferer();
        var json = PlayerPrefs.GetString("Transferer");
        JsonUtility.FromJsonOverwrite(json, transferer);
        foreach(BaseCardClass card in transferer.selectedCards)
        {
            Debug.Log(card);
            AddCard(card);
        }
   
    }

    /// <summary>
    /// Equips given card. 
    /// assumes card is already in hand
    /// (card is removed from hand to be equippped)
    /// </summary>
    /// <param name="card"></param>
    public void SwapIn(BaseCardClass card)
    {
        Debug.Log("swapping cards");
        // making sure input card is in hand
        Debug.Assert(hand.Contains(card), "Error: Attempted to equip card not found in hand");
        int slot = (int) card._equipSlot;

        // if card already equippped, add back to hand
        if (equippedCards[slot] != null )
        {
            BaseCardClass swappedOut = equippedCards[slot];

            // can skip if want to keep cards collected in hand. moving them around not *strictly* necessary in code
            hand.Add(swappedOut);

            // marking this card as inactive
            swappedOut._isEquipped = false;
            Debug.Assert(hand.Contains(swappedOut), "Error: Card swapped out not found in hand");
        }
        // !! possible issue with hand swapping...
        // can skip if want to keep cards collected in hand. moving them around not *strictly* necessary in code
        hand.Remove(card);

        // set slot to card + activate card
        card._isEquipped = true;
        equippedCards[slot] = card;

        // Debug.Assert(!hand.Contains(card), "Error: Card still in hand after equipping");


        // update the equipped UI
        AbilityManager.Instance.EquipCard(card);
    }

    /// <summary>
    /// Adds a new card to hand
    /// </summary>
    /// <param name="newCard"></param>
    public void AddCard(BaseCardClass card)
    {
        // avoiding duplicates for now
        if (hand.Contains(card))
        {
            Debug.Log("Duplicate card found in hand, skipping");
            return;
        }
        if (equippedCards[(int) card._equipSlot] == card)
        {
            Debug.Log("Duplicate card found in slot, skipping");
            return;
        }

        // TODO make clone function and clone cards so that duplicates are possible
        // currently duplicates are not possible. maybe worth to fix in the future
        hand.Add(card);
        Debug.Log("Added new card: " + card.name);
        Debug.Assert(hand.Contains(card), "FATAL ERROR: new card not found in hand???");


        // contacting UI for UI to update!
        AbilityManager.Instance.AddCardToHand(card);
    }

    /// <summary>
    /// Removes an existing card from hand
    /// </summary>
    /// <param name="newCard"></param>
    public void RemoveCard(BaseCardClass card)
    {
        Debug.Assert(hand.Contains(card), "Error: Attempted to remove card not found in hand");

        hand.Remove(card);
        Debug.Log("Removed card: " + card.name);
    }


    // methods to use equipped cards
    // maybe move to player model?
    public void UsePrimary(Vector2 playerPosition, Vector2 lookAt)
    {
        int slot = (int) EquipSlots.Primary;
        if (equippedCards[slot] != null ) {
            equippedCards[slot].UseActive(playerPosition, lookAt);
            AbilityManager.Instance.FlipCard(equippedCards[slot]);
        }
        else
        {
            Debug.Log("no primary card equipped");
            return;
        }
    }
    public void UseSecondary(Vector2 playerPosition, Vector2 lookAt)
    {
        int slot = (int)EquipSlots.Secondary;
        if (equippedCards[slot] != null) { 
            equippedCards[slot].UseActive(playerPosition, lookAt);
            AbilityManager.Instance.FlipCard(equippedCards[slot]);
        }
        else
        {
            Debug.Log("no secondary card equipped");
            return;
        }
    }
    public void UseSpecial(Vector2 playerPosition, Vector2 lookAt)
    {
        int slot = (int)EquipSlots.Special;
        if (equippedCards[slot] != null) { 
            equippedCards[slot].UseActive(playerPosition, lookAt);
            AbilityManager.Instance.FlipCard(equippedCards[slot]);
        }
        else
        {
            Debug.Log("no special card equipped");
            return;
        }
    }
    public void UseMovement(Vector2 playerPosition, Vector2 lookAt)
    {
        int slot = (int)EquipSlots.Movement;
        if (equippedCards[slot] != null) { 
            equippedCards[slot].UseActive(playerPosition, lookAt);
            AbilityManager.Instance.FlipCard(equippedCards[slot]);
        }
        else
        {
            Debug.Log("no movement card equipped");
            return;
        }
    }
}
