using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/SwordSlash")]
public class KnifeSlash : AbilityAbstract
{
    public GameObject weaponPrefab;
    private GameObject knifePrefab;
    public float duration = 0.15f;
    public override void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        // Fireball code
        knifePrefab = Instantiate(weaponPrefab);
        knifePrefab.transform.position = playerPos;
        knifePrefab.transform.up = lookAt;
        knifePrefab.transform.parent = CharacterMovement._cmInstance.gameObject.transform;
        knifePrefab.GetComponent<MeleeAbility>().slashSword(knifePrefab, duration);
    }
}
