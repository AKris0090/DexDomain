using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/SpecialAbility")]
public class SpecialScript : AbilityAbstract
{
    public enum SpecialAbilityType
    {
        movementSpeed = 0,
        damage = 1
    };
    public SpecialAbilityType abilityType = SpecialAbilityType.movementSpeed;
    public float lifespan;
    public override void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        if (abilityType == SpecialAbilityType.movementSpeed)
        {
            CharacterMovement._cmInstance.StartCoroutine(CharacterMovement._cmInstance.incMS(lifespan));
        }
        else if (abilityType == SpecialAbilityType.damage)
        {
            CharacterMovement._cmInstance.StartCoroutine(CharacterMovement._cmInstance.incDmg(lifespan));
        }
    }
}
