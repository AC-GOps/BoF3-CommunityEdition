using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = target.position;
    }
}
