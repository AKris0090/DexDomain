using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Int")]
public class IntReference : ScriptableObject
{
    [SerializeField]
    protected int _value;
    [SerializeField]
    protected int _defaultValue;
    public int Value => _value;
    public int DefaultValue => _defaultValue;

    public void Reset()
    {
        _value = _defaultValue;
    }
}
