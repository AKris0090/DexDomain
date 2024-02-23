using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Ranged Int")]
public class RangedIntReference : IntReference
{
    [SerializeField]
    protected int minValue;
    [SerializeField]
    protected int maxValue;
    new public int Value { get => value; set { Math.Clamp(value, minValue, maxValue); } }
    public int MinValue => minValue;
    public int MaxValue => maxValue;

    public void OnValidate()
    {
        if (minValue > maxValue)
        {
            minValue = maxValue;
        }
        defaultValue = Math.Clamp(defaultValue, minValue, maxValue);
        value = Math.Clamp(value, minValue, maxValue);
    }
}
