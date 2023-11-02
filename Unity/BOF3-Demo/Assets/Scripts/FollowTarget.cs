using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null) return;
        transform.position = target.position;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
