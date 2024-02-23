using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityAbstract : ScriptableObject
{
    public virtual void useAbility(Vector2 playerPos, Vector2 lookAt)
    {
        Debug.Log("using ability");
    }
}
