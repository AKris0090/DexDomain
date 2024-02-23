using UnityEngine;

[CreateAssetMenu(menuName = "Reference Types/Int")]
public class IntReference : ScriptableObject
{
    [SerializeField]
    protected int value;
    [SerializeField]
    protected int defaultValue;
    public int Value { get; set; }
    public int DefaultValue => defaultValue;

    public void Reset()
    {
        value = defaultValue;
    }
}
