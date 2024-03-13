using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public IntReference health;
    // Start is called before the first frame update
    void Start()
    {
        health.Value = health.DefaultValue;
    }

    // public reference to health, call when enemy attacks player
    public void DealDamage(int value)
    {
        if (!CharacterMovement._cmInstance.invulnerable)
        {
            health.Value -= value;
            Debug.Log(health.Value);
            // Player death
            // TODO: Add more UI events for death screen
            if (health.Value == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
