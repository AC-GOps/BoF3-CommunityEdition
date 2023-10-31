using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 target;
    private bool spooked;
    public float flySpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        spooked = true;
        animator.SetTrigger("Spooked");
        var offset = transform.position;
        offset.y += 50;
        //fly in random direction
        //UP
        var random = Random.Range(0, 20);
        if(random < 10)
        {
            offset.x += 100;
        }
        else
        {
            offset.z += 100;
            spriteRenderer.flipX = true;
        }
        target = offset;
    }

    private void Update()
    {
        if(!spooked)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime *flySpeed);
    }

}
