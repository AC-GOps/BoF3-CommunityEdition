using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationContolller : MonoBehaviour
{
    public Animator animator;
    public Animator selectedAnimator;

    public void SetAnimatorsTriggers()
    {
        animator.SetBool("Die", false);
        animator.SetTrigger("EnterBattle");
        selectedAnimator.SetTrigger("EnterBattle");
    }
}
