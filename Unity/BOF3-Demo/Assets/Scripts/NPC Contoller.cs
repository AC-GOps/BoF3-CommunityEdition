using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCContoller : MonoBehaviour
{
    public Animator _animator;
    public SpriteRenderer _renderer;
    public Rigidbody _body;
    public List<Transform> targets;
    public float m_Speed;
    private Vector3 direction;
    public LayerMask groundLayer;
    public Vector3 offset;
    public Vector3 TargetLocation;
    public int TargetIndex;
    public bool isActive;

    private void SetSprite()
    {
        TargetLocation = TargetLocation.ToIso();
        TargetLocation = TargetLocation.Round(0);

        // Had to Flip everything here, x to z. no idea why but ti seems to work now
        _animator.SetFloat("Input", TargetLocation.magnitude);

        _animator.SetFloat("BlendX", TargetLocation.z);
        _animator.SetFloat("BlendZ", TargetLocation.x);

        _renderer.flipX = TargetLocation.z > 0 ? true : false;
    }

    // Start is called before the first frame update
    void Start()
    {
        TargetIndex = 0;
        isActive = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isActive)
        {
            _animator.SetFloat("Input", 0);
            return;
        }

        direction = targets[TargetIndex].position - transform.position;
        TargetLocation = direction.normalized;
        TargetLocation = TargetLocation.Round(0);

        StickToGround();

        _body.MovePosition(transform.position + TargetLocation * Time.deltaTime * m_Speed);
        SetSprite();
        CheckDistance();

    }

    public void StopToTalk()
    {
        isActive = !isActive;
    }

    private void CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, targets[TargetIndex].position);
        if (distance<0.1)
        {
            TargetIndex++;
            if(TargetIndex>targets.Count-1)
            {
                TargetIndex = 0;
            }
        }
    }

    private void StickToGround()
    {
        // Modify the Rigidbody's velocity to only affect the Y-axis (vertical movement).
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10, groundLayer))
        {
            Vector3 newPosition = hit.point + offset;// 0.1f is an offset to ensure the Rigidbody stays slightly above the ground.
            //_body.MovePosition(newPosition);
            TargetLocation.y = newPosition.y;

            // Modify the Rigidbody's velocity to only affect the Y-axis (vertical movement).
            /*
            Vector3 newVelocity = _body.velocity;
            newVelocity.y = 0f;
            _body.velocity = newVelocity;
            */
        }

    }
}
static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
}
