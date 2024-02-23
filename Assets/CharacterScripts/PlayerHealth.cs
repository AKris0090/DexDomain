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

    public void dealDamage(int value)
    {
        health.Value -= value;
        Debug.Log(health.Value);
        if(health.Value == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
