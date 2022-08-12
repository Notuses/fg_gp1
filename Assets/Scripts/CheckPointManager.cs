using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    private static CheckPointManager instance;
    [SerializeField, Tooltip("Player start position")]
    public Vector2 checkPointPos;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
}
