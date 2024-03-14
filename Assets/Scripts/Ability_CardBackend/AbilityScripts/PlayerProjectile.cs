using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Bullet
{
    public int damage;
    // Derives from abstract class bullet, but spawns from the player in direction of mouse
    public override void SetTarget(Vector3 tar, float force, float life, GameObject spawner)
    {
        self = GetComponent<Rigidbody2D>();
        // Draw a line between the target and this bullet
        self.AddForce(tar * force);
        StartCoroutine(Lifespan(life));
        this.spawner = spawner;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        if (collision.gameObject.GetComponent<Bullet>() == null && collision.gameObject != spawner && (ignoreRaycastLayer != collision.gameObject.layer || collision.gameObject.GetComponent<Enemy>() != null))
        {
            EnemyManager.Instance.DamageEnemy(collision.gameObject, (damage * CharacterMovement._cmInstance.dmgMod));
            Destroy(this.gameObject);
        }
    }

    public void startLifeSpan(float life)
    {
        StartCoroutine(Lifespan(life));
    }

    protected override IEnumerator Lifespan(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
