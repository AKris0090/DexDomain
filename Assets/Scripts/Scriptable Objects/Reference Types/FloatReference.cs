using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Float")]
public class FloatReference : ScriptableObject
{
    protected float value;
    protected float defaultValue;
    public float Value { get; set; }
    public float DefaultValue => defaultValue;

    public void Reset()
    {
        value = defaultValue;
    }

}
