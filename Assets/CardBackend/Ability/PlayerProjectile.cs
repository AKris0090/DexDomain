using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Bullet
{

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
        if (collision.gameObject.GetComponent<Bullet>() == null)
        {
            EnemyManager.Instance.DamageEnemy(collision.gameObject, 1);
            Destroy(this.gameObject);
        }
    }
}
