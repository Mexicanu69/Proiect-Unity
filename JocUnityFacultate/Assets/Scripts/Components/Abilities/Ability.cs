using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("Info")]
    public string displayName;

    [TextArea] public string description;
    public Sprite icon;

    [Header("Visuals")]
    public GameObject particleEffect;
}