using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D self;
    private void Start()
    {
        self = GetComponent<Rigidbody2D>();
    }
    public void SetTarget(Transform tar, float force, float life)
    {
        self = GetComponent<Rigidbody2D>();
        // Draw a line between the target and this bullet
        Vector2 line = tar.position - transform.position;
        line = line.normalized;
        self.AddForce(line * force);
        StartCoroutine(lifespan(life));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("You sure hit something");
        EnemyManager.Instance.DamageObject(collision.gameObject);
        Destroy(this.gameObject);
    }

    IEnumerator lifespan(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
