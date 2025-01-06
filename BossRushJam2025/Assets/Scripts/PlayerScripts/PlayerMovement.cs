using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // main reference: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs
    [SerializeField] private MovementStats _moveStats;

    private Rigidbody _rb;
    private Collider _col;
    private FrameInput _fInput;

    [Header("Keybinds")]
    public List<KeyCode> JumpKeys = new List<KeyCode>();

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    private float _time;

    void Update()
    {
        _time += Time.deltaTime;
        GetInput();
    }

    private void GetInput() // input handler
    {
        _fInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || AnyKeyDown(JumpKeys.ToArray()),
            JumpHeld = Input.GetButton("Jump") || AnyKeyHeld(JumpKeys.ToArray()),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        // original script has stuff to accomodate joystick drift/deadzones, axed it cause we aren't planning on adding controller support

        if (_fInput.JumpDown) // works with any of the keys in the jump list
        {
            _timeJumpWasPressed = _time;
            Debug.Log("jump pressed");
        }
    }

    #region Handle Key Lists
    private bool AnyKeyDown(IEnumerable<KeyCode> keys)
    {
        foreach (KeyCode key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }
        return false;
    }
    private bool AnyKeyHeld(IEnumerable<KeyCode> keys)
    {
        foreach (KeyCode key in keys)
        {
            if (Input.GetKey(key))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    private void FixedUpdate()
    {
        // execute jump/movement/etc.
    }

    //collisions
    private float _frameLeftGrounded;
    private bool _grounded;

    // jump
    private bool _jumpToConsume; // "to consume" as in the jump to be executed
    private bool _queuedJumpUsable;
    private bool _jumpReleasedEarly; 
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasJumpQueued => _queuedJumpUsable && _time < _timeJumpWasPressed + _moveStats.JumpBuffer; // queues up next jump if the button was pressed within specified time
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _moveStats.CoyoteTime; // allows for player to jump for short period after walking off a ledge

    private void HandleJump()
    {
        if(!_jumpReleasedEarly && !_grounded && !_fInput.JumpHeld && _rb.linearVelocity.y > 0) _jumpReleasedEarly = true; // once player has left the ground and the jump key is not being held, _jumpReleasedEarly is true


    }
    private void ExecuteJump()
    {

    }

    // coyote time
    // clamped fall speed
    // edge detection
    // apex modifiers (ie changing speed at the apex of a jump)
    // jump buffering (recording player jump input if player hasn't touched the ground yet)
}

public struct FrameInput // for storing the player's inputs
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}
