using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MovementStats", menuName = "Scriptable Objects/MovementStats", order = 1)]
public class MovementStats : ScriptableObject
{
    // Will add specific stats later, will be referenced in player movement script
    public LayerMask PlayerLayer;

    public float JumpPower;

    public float JumpBuffer;

    public float CoyoteTime;

    public float MaxSpeed;

    public float Acceleration;

    public float GroundDeceleration;

    public float AirDeceleration;

    public float FallAcceleration;

    public float JumpReleasedEarlyGravityModifier;

    public float MaxFallSpeed;

    [Tooltip("Applies downwards force while grounded")]
    public float GroundingForce;

    [Tooltip("'The detection distance for grounding and roof detection'"), Range (0f, 0.5f)] 
    public float GrounderDistance;
}
