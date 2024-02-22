using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CardOperations
{
    [CreateAssetMenu(menuName = "Card Models/Multi-Use Cards")]
    /// <summary>
    /// Model for cards which can be used multiple times before being put on cooldown
    /// </summary>
    public class MultiuseCardModel : CooldownCardModel
    {
        public int maxUses;
        private int remainingUses;

        public new void OnEnable()
        {
            //Debug.Log("multiuse card model awake: " + cardName);
            remainingUses = maxUses;
            base.OnEnable();
        }

        // TODO fix UseActive conditions. add cooldown function here.
        public override void UseActive()
        {
            // first checks if remaining uses
            if (remainingUses == 0)
            {
                Debug.Log("Card " + name + " has no uses left");
                return;
                // ability not used if on cooldown
            }
            
            // counts down remaining uses, then uses ability
            remainingUses -= 1;
            Debug.Log("Card " + name + " has uses left " + remainingUses);
            // if ran out of uses, start card cooldown.
            if (remainingUses == 0) { StartCooldown(); }
            base.UseActive();
        }
        public override void SwapIn()
        {

        }
        public override void SwapOut()
        {

        }
    }
}
