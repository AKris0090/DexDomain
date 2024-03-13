using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/FireBallAbility")]
public class FireBallAbility : AbilityAbstract
{
    public GameObject fireBallRef;
    public GameObject fireBallPrefab;
    public float force;
    public float lifespan;

    public override void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        // Fireball code
        fireBallPrefab = Instantiate(fireBallRef);
        fireBallPrefab.transform.position = playerPos;
        fireBallPrefab.transform.up = lookAt;
        fireBallPrefab.transform.position += fireBallPrefab.transform.up;
        fireBallPrefab.GetComponent<PlayerProjectile>().SetTarget(lookAt, force, lifespan, fireBallPrefab.gameObject);
    }
}
