using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatReference : ScriptableObject
{
    private float value;
    private float defaultValue;
    public float Value { get; set; }
    public float DefaultValue => defaultValue;

    public void Reset()
    {
        value = defaultValue;
    }

}
