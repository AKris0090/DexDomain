using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class FireBallAbility : AbilityAbstract
{
    public GameObject fireBallRef;
    private GameObject fireBallSprite;
    private float moveSpeed;
    public override void useAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.useAbility(playerPos, lookAt);
        // Fireball code
        fireBallSprite = Instantiate(fireBallRef);
        fireBallSprite.transform.position = playerPos;
        yield return new WaitForSeconds(2.0f);
        Destroy(fireBallSprite);
    }
}
