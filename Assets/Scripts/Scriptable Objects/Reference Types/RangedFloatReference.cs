using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Ranged Float")]
public class RangedFloatReference : FloatReference
{
    [SerializeField]
    protected float _minValue;
    [SerializeField]
    protected float _maxValue;
    new public float Value 
    { 
        get => _value; 
        set => _value = Mathf.Clamp(value, _minValue, _maxValue); 
    }
    public float MinValue => _minValue;
    public float MaxValue => _maxValue;
    public void OnValidate()
    {
        if (_minValue > _maxValue)
        {
            _minValue = _maxValue;
        }
        _defaultValue = Mathf.Clamp(_defaultValue, _minValue, _maxValue);
        _value = Mathf.Clamp(_value, _minValue, _maxValue);
    }
}
