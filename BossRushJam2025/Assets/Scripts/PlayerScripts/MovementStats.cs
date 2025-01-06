using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MovementStats", menuName = "Scriptable Objects/MovementStats", order = 1)]
public class MovementStats : ScriptableObject
{
    // Will add specific stats later, will be referenced in player movement script

    public float JumpBuffer;

    public float CoyoteTime;
}
