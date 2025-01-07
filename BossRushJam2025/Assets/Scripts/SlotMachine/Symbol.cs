using UnityEngine;

[CreateAssetMenu(fileName = "Symbol", menuName = "ScriptableObjects/Symbol", order = 1)]
public class Symbol : ScriptableObject
{
    public string symbolName;
    public Sprite sprite;
}
