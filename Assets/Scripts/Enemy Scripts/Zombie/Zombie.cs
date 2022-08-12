using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    //components
    private BoxCollider2D myCollider;
    //private AudioSource audioSource;
    //private ZombieMovement zombieMovement;
    
    private void Awake()
    {
        //zombieMovement = GetComponent<ZombieMovement>();
        audioSource = GetComponent<AudioSource>();
    }
}
