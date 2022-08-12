using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostTrap : MonoBehaviour
{
    [SerializeField, Tooltip("How many ghosts to spawn?")]
    private float ghostsToSpawn = 4;
    [SerializeField]
    private GameObject ghost;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            for (int i = 0; i < ghostsToSpawn; i++)
            {
                Vector2 spawnPos = new Vector2(transform.position.x + RandomizeGhostSpawnPos(),
                    transform.position.y + RandomizeGhostSpawnPos());
                Instantiate(ghost, spawnPos, Quaternion.identity);
                print("TEST");
            }

            float RandomizeGhostSpawnPos()
            {
                float randPos = Random.Range(3, 7);
                bool plusOrMinus = Random.value > 0.5;
                randPos = plusOrMinus ? randPos : -randPos;
                return randPos;
            }
        }
        
    }
}
