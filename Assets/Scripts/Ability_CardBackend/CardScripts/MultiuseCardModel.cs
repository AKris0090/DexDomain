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
        private int _remainingUses;
        private bool _onCooldownMultiuse;

        public new void OnEnable()
        {
            //Debug.Log("multiuse card model awake: " + cardName);
            _remainingUses = maxUses;
            _onCooldownMultiuse = false;
            base.OnEnable();
        }

        // only allows UseActive() when remaining uses or not on cooldown
        // code here is unreasonably messy, sorry 
        public override void UseActive(Vector2 playerPosition, Vector2 lookAt)
        {
            // check if cooldown was triggered
            if (_onCooldownMultiuse)
            {
                // check if cooldown timer complete
                if (!OnCooldown())
                {
                    // cooldown finished, reset now
                    _onCooldownMultiuse = false;
                    _remainingUses = maxUses;
                    Debug.Log("reset multiuse card " + cardName);
                }
                else
                {
                    Debug.Log("Card " + _remainingUses + " on cooldown, has no uses");
                    return;
                }
            }

            // first checks if remaining uses and hasnt triggered reset
            if (_remainingUses == 0 && !_onCooldownMultiuse)
            {
                Debug.Log("Card " + name + " has no uses left, starting cooldown if needed");
                _onCooldownMultiuse = true;
                StartCooldown();
                return;
                // ability not used if on cooldown
            }
            
            // counts down remaining uses, then uses ability
            _remainingUses -= 1;
            Debug.Log("Card " + name + " has " + _remainingUses + " uses left ");
            // if ran out of uses, start card cooldown.
            if (_remainingUses == 0) 
            {
                Debug.Log("starting cooldown for multiuse");
                _onCooldownMultiuse = true;
                StartCooldown(); 
            }
            // using scuffy skip method to call to base card class
            BaseUseActive(playerPosition, lookAt);
        }
        public override void SwapIn()
        {

        }
        public override void SwapOut()
        {

        }
    }
}
