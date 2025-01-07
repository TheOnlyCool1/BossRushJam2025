using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // note: we should opt for Physics2D because we don't need to consider the z axis
    // main reference: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs
    [SerializeField] private MovementStats _moveStats;

    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _fInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    [Header("Keybinds")]
    public List<KeyCode> JumpKeys = new List<KeyCode>();

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
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
            _jumpToConsume = true;
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
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    #region Collisions
    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false; // don't return the collider raycasts are originating from

        // ground and ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _moveStats.GrounderDistance, ~_moveStats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _moveStats.GrounderDistance, ~_moveStats.PlayerLayer);
        // "~_moveStats.PlayerLayer" detects stuff on any layer that isnt the player layer, i think?

        if(ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y); // if player hits a cieling, set upward velocity to the smallest of the provided variables

        if (!_grounded && groundHit) // when player hits the ground from non grounded state
        {
            _grounded = true;
            _coyoteUsable = true;
            _queuedJumpUsable = true;
            _jumpReleasedEarly = false;

            // invoke event here if needed
        }
        else if (_grounded && !groundHit) // if player leaves the ground from a grounded state
        {
            _grounded = false;
            _frameLeftGrounded = _time;

            // invoke event here if needed
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }
    #endregion

    #region Jump
    private bool _jumpToConsume; // "to consume" as in the jump to be executed
    private bool _queuedJumpUsable;
    private bool _jumpReleasedEarly; 
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasJumpQueued => _queuedJumpUsable && _time < _timeJumpWasPressed + _moveStats.JumpBuffer; // queues up next jump if the button was pressed within specified time
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _moveStats.CoyoteTime; // allows for player to jump for short period after walking off a ledge

    private void HandleJump()
    {
        if(!_jumpReleasedEarly && !_grounded && !_fInput.JumpHeld && _rb.linearVelocity.y > 0) _jumpReleasedEarly = true;
        // once player has left the ground and the jump key is not being held, _jumpReleasedEarly is true (as far as I'm aware???)

        if (!_jumpToConsume && !HasJumpQueued) return; // if there are no more jumps in the queue and none to be executed, return

        if (_grounded || CanUseCoyote) ExecuteJump(); // execute jump if grounded or coyote time is useable

        _jumpToConsume = false; // to make sure the jump is only executed once
    }
    private void ExecuteJump()
    {
        _jumpReleasedEarly = false;
        _timeJumpWasPressed = 0;
        _queuedJumpUsable = false;
        _coyoteUsable = false;

        _frameVelocity.y = _moveStats.JumpPower;

        Debug.Log("Jump Executed");
        // invoke event here if needed
    }
    #endregion

    #region Horizontal Movement
    private void HandleDirection()
    {
        if (_fInput.Move.x == 0) // if theres no horizontal input, decelerate the player
        {
            var deceleration = _grounded ? _moveStats.GroundDeceleration : _moveStats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime); // velocity moves towards 0 at the rate of deceleration
        }
        else // accelerate player until they reach max speed
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _fInput.Move.x * _moveStats.MaxSpeed, _moveStats.Acceleration * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Gravity
    private void HandleGravity()
    {
        if(_grounded && _frameVelocity.y <= 0f) // while the player is grounded, apply grounding force
        {
            _frameVelocity.y = _moveStats.GroundingForce;
        }
        else
        {
            var inAirGravity = _moveStats.FallAcceleration;
            if (_jumpReleasedEarly && _frameVelocity.y > 0) inAirGravity *= _moveStats.JumpReleasedEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_moveStats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime); // bring fall speed towards max value
        }
    }
    #endregion

    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

    // edge detection
    // apex modifiers (ie changing speed at the apex of a jump), applying downward force to make player fall faster (possibly)
    // modifying jump curve
    // modifying acceleration and deceleration of player
    // jump buffering (recording player jump input if player hasn't touched the ground yet)
}

public struct FrameInput // for storing the player's inputs
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}
