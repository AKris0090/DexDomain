using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Ranged Float")]
public class RangedFloatReference : FloatReference
{
    [SerializeField]
    protected float minValue;
    [SerializeField]
    protected float maxValue;
    new public float Value { get => value; set { Mathf.Clamp(value, minValue, maxValue); } }
    public float MinValue => minValue;
    public float MaxValue => maxValue;
    public void OnValidate()
    {
        if (minValue > maxValue)
        {
            minValue = maxValue;
        }
        defaultValue = Mathf.Clamp(defaultValue, minValue, maxValue);
        value = Mathf.Clamp(value, minValue, maxValue);
    }
}
