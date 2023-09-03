using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class NumberBouncer : MonoBehaviour
{
    public static NumberBouncer Instance { get; private set; }

    public UIToEnemy numberbounce;
    public Transform numberTarget;
    public Animator animator;
    public TMP_Text numberText;
    public float randomnumberSpeed;
    public float bounceEndTime;


    private float time;
    private bool randomiseNums;
    private int _finalNumber;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayNumberBounceAtTarget(Transform target, int finalnumber)
    {
        numberText.text = "";
        numberTarget = target;
        numberbounce.ChangePosition(numberTarget.position);
        animator.SetTrigger("Bounce");
        randomiseNums = true;
        _finalNumber = finalnumber;
    }
    public void PlayTextBounceAtTarget(Transform target, string text)
    {
        numberText.text = "";
        numberTarget = target;
        numberbounce.ChangePosition(numberTarget.position);
        animator.SetTrigger("Bounce");
        numberText.text = text;
    }

    public void RandomiseNumbers()
    {
        time += Time.deltaTime;

        if (time> randomnumberSpeed)
        {
            numberText.text = Random.Range(0, _finalNumber + 5).ToString();
        }
        if(time> bounceEndTime)
        {
            randomiseNums=false;
            time = 0;
            numberText.text = _finalNumber.ToString();
        }

    }

    private void Update()
    {
        if(!randomiseNums)
        {
            return;
        }

        RandomiseNumbers();

    }
}
