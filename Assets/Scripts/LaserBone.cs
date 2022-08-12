using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBone : MonoBehaviour
{
    private bool timeToStart;
    private Vector3 direction;
    private float speed;
    private float rotationSpeed;

    public void StartIt(Vector3 atkDir, float boneThrowSpeed, float boneRotationSpeed)
    {
        direction = atkDir;
        speed = boneThrowSpeed;
        rotationSpeed = boneRotationSpeed;
        timeToStart = true;
    }

    public void Update()
    {
        if (timeToStart)
        {
            transform.position += direction * speed * Time.deltaTime;
            transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
    
}
