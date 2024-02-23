using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Float")]
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
