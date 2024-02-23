using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/FireBallAbility")]
public class FireBallAbility : AbilityAbstract
{
    public GameObject fireBallRef;
    private GameObject fireBallPrefab;

    public override void useAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.useAbility(playerPos, lookAt);
        // Fireball code
        fireBallPrefab = Instantiate(fireBallRef);
        fireBallPrefab.transform.position = playerPos;
        fireBallPrefab.transform.up = lookAt; 
        fireBallPrefab.GetComponent<PlayerProjectile>().SetTarget(lookAt, 200, 5.0f);
    }
}
