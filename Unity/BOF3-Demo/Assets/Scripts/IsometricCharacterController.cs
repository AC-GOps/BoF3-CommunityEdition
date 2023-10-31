using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    public SpriteRenderer _emoji;

    public bool isSprint;
    public bool sprinting;
    public bool autoSprint;

    public bool canMove;

    public LayerMask groundLayer;
    public float raycastDistance = 1.0f;
    public Vector3 offset;

    public bool stickToGround;

    public RandomBattleManager battleManager;

    private void Update()
    {
        SetSprite();
    }

    private void FixedUpdate()
    {
        Move();
        StopSlipping();
        if(!stickToGround)
        { return; }
        StickToGround();
    }

    public void SetEmoji()
    {
        _emoji.transform.DOScale(2, 0.1f).SetLoops(2,LoopType.Yoyo);
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

    public void GetMovementInput(InputAction.CallbackContext context)
    {
        if (this.isActiveAndEnabled == false)
        {
            return;
        }

        if (!canMove)
        {
            _animator.SetFloat("Input", 0);
            return;
        }

        Vector2 input = context.ReadValue<Vector2>();
        _input = new Vector3(input.x, 0, input.y);
        _animator.SetFloat("Input", _input.normalized.magnitude);

        if (context.performed)
        {
            battleManager.playerMoving = true;
        }
        if(context.canceled)
        {
            battleManager.playerMoving = false;
        }

        if (_input == Vector3.zero)
        {
            SetIdle();
            return;
        }

        _inputLast = _input;
    }

    public void GetSprintInput(InputAction.CallbackContext context)
    {
        // Sprint 
        if (context.performed)
        {
            sprinting = true;
        }
        if (context.canceled)
        {
            sprinting = false;
        }

        isSprint = autoSprint;

        if (sprinting)
        {
            isSprint = !isSprint;
        }

        _speed = isSprint ? _speedRun : _speedWalk;
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

    private void Move()
    {
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

