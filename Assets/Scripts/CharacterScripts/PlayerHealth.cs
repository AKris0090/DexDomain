using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public IntReference health;
    public float iframes = 0.5f;
    bool canTakeDamage;
    // Start is called before the first frame update
    void Start()
    {
        health.Value = health.DefaultValue;
        canTakeDamage = true;
    }

    // public reference to health, call when enemy attacks player
    public void DealDamage(int value)
    {
        if (!CharacterMovement._cmInstance.invulnerable && canTakeDamage)
        {
            health.Value -= value;
            canTakeDamage = false;
            StartCoroutine(damageCooldown());
            Debug.Log(health.Value);
            // Player death
            // TODO: Add more UI events for death screen
            if(health.Value < 0) { health.Value = 0; }
            if (health.Value == 0)
            {
                UIManager.Instance.EndGame();
                gameObject.SetActive(false);
            }
        }
    }

    // Probably add damage juice here as well
    IEnumerator damageCooldown()
    {
        yield return new WaitForSeconds(iframes);
        canTakeDamage = true;
    }
}
