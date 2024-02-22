using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// keeping card internals in its own namespace
namespace CardOperations
{
    public enum EquipSlots
    {
        Primary, // m1
        Secondary, // m2
        Special, // q or something
        Movement // shift prob
    }

    /// <summary>
    /// Base class for cards.
    /// Properties: 
    /// Methods: UseActive(), SwapIn(), SwapOut()
    /// </summary>
    public abstract class BaseCardClass : ScriptableObject
    {
        public string cardName;
        internal bool isEquipped; // tracks if card is in equipt slot
        // note that cards are initialized as unequipped


        // !! equiptSlot based on enum values above. designates which slot cards attach to.
        // primary = 0, secondary = 1, special = 2, movement = 3.
        //public int equipSlot;
        [SerializeField] internal EquipSlots equipSlot = EquipSlots.Primary;

        public void Awake()
        {
            Debug.Log("base card awake: " +  cardName);
            isEquipped = false;
        }

        // uses the card's active effect
        public virtual void UseActive()
        {
            if (isEquipped) {Debug.Log("Used " + cardName + " active"); }
            else { Debug.Log("Failed to use " + cardName + " active. Not Equipt"); }
        }

        // maybe replace. intended to swap in/out cards, but maybe just do that in the manager
        public abstract void SwapIn();
        public abstract void SwapOut();
    }
}