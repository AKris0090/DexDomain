using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Float")]
public class FloatReference : ScriptableObject
{
    [SerializeField]
    protected float value;
    [SerializeField]
    protected float defaultValue;
    public float Value { get; set; }
    public float DefaultValue => defaultValue;

    public void Reset()
    {
        value = defaultValue;
    }
}
