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
        internal bool _isEquipped; // tracks if card is in equipt slot
        // note that cards are initialized as unequipped

        // Ability references
        public AbilityAbstract abilityAbs;


        // !! equiptSlot based on enum values above. designates which slot cards attach to.
        // primary = 0, secondary = 1, special = 2, movement = 3.
        //public int equipSlot;
        [SerializeField] internal EquipSlots _equipSlot = EquipSlots.Primary;

        public void OnEnable()
        {
            //Debug.Log("base card awake: " +  cardName);
            _isEquipped = false;
        }

        // uses the card's active effect
        public virtual void UseActive(Vector2 playerPosition, Vector2 lookAt)
        {
            //if (_isEquipped) {Debug.Log("Used " + cardName + " active"); }
            //else { Debug.Log("Failed to use " + cardName + " active. Not Equipt"); }
        }

        public virtual float CooldownRemaining()
        {
            return 0;
        }

        // maybe replace. intended to swap in/out cards, but maybe just do that in the manager
        public abstract void SwapIn();
        public abstract void SwapOut();


        // UI access
        public EquipSlots EquipPlacement { get { return _equipSlot; } }
        public GameObject cardUIPrefab;
    }
}