using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Generic bullet, can be used for multiple enemies
    // Create a instance, then use set target
    // Give it a target, a force, and a target with set target
    protected Rigidbody2D self;
    protected GameObject spawner;
    private void Start()
    {
        self = GetComponent<Rigidbody2D>();
    }
    // Sends the bullet towards tar with force newtons for life seconds, starting at position rotation
    public virtual void SetTarget(Vector3 tar, float force, float life, GameObject spawner, Vector3 position, Quaternion rotation)
    {
        Debug.Log(force);
        transform.position = position;
        transform.rotation = rotation;
        self = GetComponent<Rigidbody2D>();
        // Draw a line between the target and this bullet
        Vector2 line = tar - transform.position;
        line = line.normalized;
        self.AddForce(line * force);
        StartCoroutine(Lifespan(life));
        this.spawner = spawner;
    }

    // TODO: Make this either ignore enemies, or damage them
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != spawner && collision.gameObject.GetComponent<Bullet>() == null)
        {
            EnemyManager.Instance.DamageObject(collision.gameObject);
            EnemyManager.Instance.ReturnBullet(this);
        }
    }

    protected virtual IEnumerator Lifespan(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EnemyManager.Instance.ReturnBullet(this);
    }
}
