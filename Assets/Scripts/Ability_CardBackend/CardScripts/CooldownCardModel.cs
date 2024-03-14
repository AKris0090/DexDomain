using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CardOperations
{
    [CreateAssetMenu(menuName = "Card Models/Single Use Cards")]
    /// <summary>
    /// Model for cards which are single use then put on cooldown
    /// </summary>
    public class CooldownCardModel : BaseCardClass
    {
        // internal lets me use currentCooldown in other class/files
        [SerializeField] internal float cooldown;
        internal float _initialTime; // try having it be private?

        public new void OnEnable()
        {
            //Debug.Log("cooldown card model awake: " + cardName);
            _initialTime = Time.time - cooldown;
            base.OnEnable();
        }


        // checks if ability is on cooldown
        // might want to refactor into coroutine?
        internal bool OnCooldown()
        {
            // Debug.Log("current time: " + Time.time + ", initial time: " + initialTime);
            if (Time.time > _initialTime + cooldown)
            {
                //Debug.Log("is not on cooldown");
                return false;
            }
            //Debug.Log("is on cooldown");
            return true;
        }

        // returns float 0-1 for how far cooldown has progressed 
        // (was thinking itd be helpful for ui or something)
        public float CooldownProgress()
        {
            // if on cooldown return 0-1 float for percentage completed
            if (OnCooldown()) return Time.time / (_initialTime + cooldown);

            // else return 1 = 100%
            return 1;
        }

        // returns time remaining as a float
        // (maybe useful for ui or something)
        public override float  CooldownRemaining()
        {
            // return cooldown - (time since initial) = time remaining
            if (OnCooldown()) return cooldown - (Time.time - _initialTime);

            // else return 0 for no time remaining
            return 0;
        }

        // starts cooldown (or skips if repeat)
        internal void StartCooldown()
        {
            if (OnCooldown())
            {
                // ideally shouldnt hit here since UseActive checks OnCooldown() too, but just in case
                //Debug.Log("already on cooldown, avoided restarting cooldown");
                return;
            }
            _initialTime = Time.time;
            //Debug.Log("started cooldown at time: " + initialTime);
        }

        // use active for cooldown card model
        public override void UseActive(Vector2 playerPosition, Vector2 lookAt) 
        {
            // first checks if card on cooldown
            if (OnCooldown())
            {
                //Debug.Log("Card " + cardName + " on cooldown, " + CooldownRemaining() + " seconds left");
                return;
                // ability not used if on cooldown
            }
            abilityAbs.UseAbility(playerPosition, lookAt);
            // if not on cooldown, runs parent UseActive()
            //Debug.Log("starting cooldown on " + cardName);
            StartCooldown();
            base.UseActive(playerPosition, lookAt);
        }

        // (fml) makeshift function to get just basecard UseActive()
        // *super* scuffy solution to a problem i created myself by having multiuseCard
        public void BaseUseActive(Vector2 playerPosition, Vector2 lookAt)
        {
            base.UseActive(playerPosition, lookAt);
        }

        public override void SwapIn() 
        {
        
        }
        public override void SwapOut() 
        {
        
        }
    }
}
