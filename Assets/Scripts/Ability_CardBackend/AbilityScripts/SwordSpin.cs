using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/SwordSpin")]
public class SwordSpin : AbilityAbstract
{
    public GameObject weaponPrefab;
    private GameObject swordPrefab;
    public override void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        // Fireball code
        swordPrefab = Instantiate(weaponPrefab);
        swordPrefab.transform.position = playerPos;
        swordPrefab.transform.up = lookAt;
        swordPrefab.transform.parent = CharacterMovement._cmInstance.gameObject.transform;
        //swordPrefab.transform.position += swordPrefab.transform.up * 3.5f;
        swordPrefab.GetComponent<MeleeAbility>().spinSword(swordPrefab);
    }
}
