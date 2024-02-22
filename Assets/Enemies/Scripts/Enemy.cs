using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class for enemies
public class Enemy : MonoBehaviour
{
    public int health = 1;
    // Dirty bool checking if this enemy can take damage
    // NEED TO SET THIS TO TRUE IN ALL THE CHILD STARTS OR THEY CAN'T TAKE DAMAGE
    protected bool canTakeDamage;
    public float damageCooldown = 0.5f;
    // Take damage using damage feedback and waiting for damage cooldown before being able to take damage again
    private void Start()
    {
        canTakeDamage = true;
    }
    public void Damage(int amount)
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
                Destroy(this.gameObject);
            }
        }
    }

    // Give the player some feedback for damaging the enemy
    // By default, flash red
    // Dunno how the flashing will work with sprits that are not one color, but ah well, we cross that brigde when we come to it
    IEnumerator DamageFeedback()
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
    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
}
