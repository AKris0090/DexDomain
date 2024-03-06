using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityAbstract : ScriptableObject
{
    // Abstract method to use any type of ability. Inheriting classes should override this function to handle the visual events that occur when an ability is used.
    public virtual void UseAbility(Vector2 playerPos, Vector2 lookAt)
    {
        Debug.Log("using ability");
    }
}
