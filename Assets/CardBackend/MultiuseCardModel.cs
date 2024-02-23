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
        private bool onCooldownMultiuse;

        public new void OnEnable()
        {
            //Debug.Log("multiuse card model awake: " + cardName);
            remainingUses = maxUses;
            onCooldownMultiuse = false;
            base.OnEnable();
        }

        // only allows UseActive() when remaining uses or not on cooldown
        // code here is unreasonably messy, sorry 
        public override void UseActive()
        {
            // check if cooldown was triggered
            if (onCooldownMultiuse)
            {
                // check if cooldown timer complete
                if (!OnCooldown())
                {
                    // cooldown finished, reset now
                    onCooldownMultiuse = false;
                    remainingUses = maxUses;
                    Debug.Log("reset multiuse card " + cardName);
                }
                else
                {
                    Debug.Log("Card " + remainingUses + " on cooldown, has no uses");
                    return;
                }
            }

            // first checks if remaining uses and hasnt triggered reset
            if (remainingUses == 0 && !onCooldownMultiuse)
            {
                Debug.Log("Card " + name + " has no uses left, starting cooldown if needed");
                onCooldownMultiuse = true;
                StartCooldown();
                return;
                // ability not used if on cooldown
            }
            
            // counts down remaining uses, then uses ability
            remainingUses -= 1;
            Debug.Log("Card " + name + " has " + remainingUses + " uses left ");
            // if ran out of uses, start card cooldown.
            if (remainingUses == 0) 
            {
                Debug.Log("starting cooldown for multiuse");
                onCooldownMultiuse = true;
                StartCooldown(); 
            }
            // using scuffy skip method to call to base card class
            BaseUseActive();
        }
        public override void SwapIn()
        {

        }
        public override void SwapOut()
        {

        }
    }
}
