using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // main reference: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs
    [SerializeField] private MovementStats _moveStats;

    private Rigidbody _rb;
    private Collider _col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // To Do list: 
    // input handler
    // jump
    // coyote time
    // clamped fall speed
    // edge detection
    // apex modifiers (ie changing speed at the apex of a jump)
    // jump buffering (recording player jump input if player hasn't touched the ground yet)
}
