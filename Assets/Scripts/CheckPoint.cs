using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private CheckPointManager CheckPointManager;

    private void Start()
    {
        CheckPointManager = GameObject.FindGameObjectWithTag("CheckPoint").GetComponent<CheckPointManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPointManager.checkPointPos = transform.position;
        }
    }
}
