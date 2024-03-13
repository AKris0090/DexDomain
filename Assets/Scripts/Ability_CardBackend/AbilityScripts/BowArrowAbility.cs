using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Abstract Abilities/BowArrowAbility")]
public class BowArrowAbility : AbilityAbstract
{
    public GameObject arrowRef;
    public GameObject BowPrefab;
    private GameObject arrowPrefab;
    private GameObject bowPrefab;
    public float force;
    public float lifespan;

    public override void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        base.UseAbility(playerPos, lookAt);
        // Fireball code
        arrowPrefab = Instantiate(arrowRef);
        bowPrefab = Instantiate(BowPrefab);
        arrowPrefab.transform.position = playerPos;
        bowPrefab.transform.position = playerPos;
        arrowPrefab.transform.up = lookAt;
        bowPrefab.transform.up = lookAt;
        arrowPrefab.transform.position += arrowPrefab.transform.up;
        bowPrefab.transform.parent = CharacterMovement._cmInstance.gameObject.transform;
        arrowPrefab.GetComponent<PlayerProjectile>().SetTarget(lookAt, force, lifespan, arrowPrefab.gameObject);
        bowPrefab.GetComponent<PlayerProjectile>().startLifeSpan(.5f);
    }
}
