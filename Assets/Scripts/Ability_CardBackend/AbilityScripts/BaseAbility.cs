using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// since abilities are visual, should prob be game objects, so monobehavior?

public class BaseAbility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void UseAbility()
    {
        Debug.Log("Used Ability!");
    }
}
