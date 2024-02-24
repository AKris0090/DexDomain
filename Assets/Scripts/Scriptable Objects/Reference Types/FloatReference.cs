using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Float")]
public class FloatReference : ScriptableObject
{
    [SerializeField]
    protected float _value;
    [SerializeField]
    protected float _defaultValue;
    public float Value => _value;
    public float DefaultValue => _defaultValue;

    public void Reset()
    {
        _value = _defaultValue;
    }
}
