using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Parent class for enemies
public class Enemy : MonoBehaviour
{
    public int health = 1;
    protected bool canLookAtPlayer;
    // Dirty bool checking if this enemy can take damage
    protected bool canTakeDamage;
    protected bool canAct;
    public float damageCooldown = 0.5f;
    protected EnemyManager enemyManager;
    protected Pool<Bullet> bulletPool;
    SpriteRenderer self;
    // Take damage using damage feedback and waiting for damage cooldown before being able to take damage again
    protected virtual void Start()
    {
        canTakeDamage = true;
        canAct = true;
        enemyManager = EnemyManager.Instance;
        canLookAtPlayer = true;
        self = GetComponent<SpriteRenderer>();
        StartCoroutine(lookAtPlayer());
    }

    IEnumerator lookAtPlayer()
    {
        while (true)
        {
            if (canLookAtPlayer)
            {
                if(enemyManager.GetPlayerPosition().x < transform.position.x)
                {
                    self.flipX = false;
                }
                else
                {
                    self.flipX = true;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public virtual void Damage(int amount)
    {
        if (canTakeDamage)
        {
            health -= amount;
            StartCoroutine(DamageFeedback());
            StartCoroutine(DamageCooldown());
            canTakeDamage = false;
            Debug.Log("Damage delt to enemy");
            if (health <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(Die());
            }
        }
    }

    protected IEnumerator Die()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        GetComponent<NavMeshAgent>().SetDestination(transform.position);
        canAct = false;
        renderer.color = Color.red;
        for(int i = 0; i < 10; i++)
        {
            Color color = renderer.color;
            color.a -= 0.1f;
            renderer.color = color;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(this.gameObject);
    }

    // Give the player some feedback for damaging the enemy
    // By default, flash red
    // Dunno how the flashing will work with sprits that are not one color, but ah well, we cross that brigde when we come to it
    protected IEnumerator DamageFeedback()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color oldColor = new Color(renderer.color.r, renderer.color.g, renderer.color.b);
        for (int i = 0; i < 2; i++)
        {
            renderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.color = oldColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Resets the can take damage cooldown
    protected IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}
