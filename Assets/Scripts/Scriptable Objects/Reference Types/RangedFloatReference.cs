using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Ranged Float")]
public class RangedFloatReference : FloatReference
{
    protected float minValue;
    protected float maxValue;
    new public float Value { get => value; set { Mathf.Clamp(value, minValue, maxValue); } }
    public float MinValue => minValue;
    public float MaxValue => maxValue;
}
