using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCharacterController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _speedRun = 10;
    [SerializeField] private float _speedWalk = 5;
    [SerializeField] private float _turnSpeed = 360;
    public Vector3 _input;
    private Vector3 _inputLast;
    public Animator _animator;

    public SpriteRenderer _renderer;

    public bool isSprint;
    public bool sprinting;
    public bool autoSprint;

    public bool canMove;

    public LayerMask groundLayer;
    public float raycastDistance = 1.0f;
    public Vector3 offset;

    public bool stickToGround;

    private void Update()
    {
        GetMovementInput();
        SetSprite();
        SetSpeed();

    }

    private void FixedUpdate()
    {
        Move();
        StopSlipping();
        if(!stickToGround)
        { return; }
        StickToGround();
    }

    private void StopSlipping()
    {
        // Perform a raycast downward to check for the ground.

        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (_input.magnitude == 0)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            return;
        }



    }

    private void StickToGround()
    {
        // Modify the Rigidbody's velocity to only affect the Y-axis (vertical movement).
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            Vector3 newPosition = hit.point + offset;// 0.1f is an offset to ensure the Rigidbody stays slightly above the ground.
            _rb.MovePosition(newPosition);

            // Modify the Rigidbody's velocity to only affect the Y-axis (vertical movement).
            Vector3 newVelocity = _rb.velocity;
            newVelocity.y = 0f; // Set the Y velocity to zero.
            _rb.velocity = newVelocity;
        }
        
    }

    private void GetMovementInput()
    {
        if (!canMove)
        {
            _animator.SetFloat("Input", 0);
            return;
        }

        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _animator.SetFloat("Input", _input.normalized.magnitude);

        if (_input == Vector3.zero)
        {
            SetIdle();
            return;
        }

        _inputLast = _input;
    }

    private void SetSprite()
    {
        _animator.SetFloat("BlendX", _inputLast.x);
        _animator.SetFloat("BlendZ", _inputLast.z);

        if(_speed ==_speedWalk)
        {
            _animator.speed = 1;
        }
        if (_speed == _speedRun)
        {
            _animator.speed = 2;
        }

        _renderer.flipX = _inputLast.x <0 ? true:false;
    }

    private void SetIdle()
    {
        Sprite idleSprite = null;
        if (_inputLast == Vector3.up)
        {
            idleSprite = _animator.GetComponent<Sprite>();
        }
        _renderer.sprite = idleSprite;
    }

    private void SetSpeed()
    {
        isSprint = autoSprint;

        if(sprinting)
        {
            isSprint = !isSprint;
        }

        // Sprint 
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprinting= false;
        }

        _speed = isSprint ? _speedRun: _speedWalk;
    }

    private void Move()
    {

        // Bad collision
        //_rb.MovePosition(transform.position + _input.ToIso().normalized * _speed * Time.fixedDeltaTime);

        // Bad movement
        if (!canMove)
        {
            return;
        }

        _rb.AddForce(_input.ToIso().normalized * _speed * Time.fixedDeltaTime,ForceMode.VelocityChange);
    }
}

public static class Helpers 
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}

