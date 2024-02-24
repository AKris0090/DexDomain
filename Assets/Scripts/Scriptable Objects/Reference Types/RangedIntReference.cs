using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Ranged Int")]
public class RangedIntReference : IntReference
{
    [SerializeField]
    protected int _minValue;
    [SerializeField]
    protected int _maxValue;
    new public int Value { get => _value; set { Math.Clamp(_value, _minValue, _maxValue); } }
    public int MinValue => _minValue;
    public int MaxValue => _maxValue;

    public void OnValidate()
    {
        if (_minValue > _maxValue)
        {
            _minValue = _maxValue;
        }
        _value = Math.Clamp(_value, _minValue, _maxValue);
    }
}
