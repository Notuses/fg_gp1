using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : Enemy
{
    private bool timeToStart;
    private Vector3 direction;
    private float speed;
    private float startSpeed;
    private EnemyArcher enemyArcher;
    private bool isSugarRushing;
    private Vector2 shooterPos;
    private Collider2D col;


    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void StartIt(Vector2 atkDir, float boltSpeed, float boltStartSpeed, GameObject archerShooting)
    {
        enemyArcher = archerShooting.GetComponent<EnemyArcher>();
        shooterPos = archerShooting.transform.position;
        direction = atkDir.normalized;
        speed = boltSpeed;
        startSpeed = boltStartSpeed;
        timeToStart = true;
    }

    public void Update()
    {
        if (!timeToStart)
            return;
        if (!col.enabled)
        {
            if (Vector2.Distance(transform.position, shooterPos) >= 0.5f)
            {
                col.enabled = true;
            }
        }

        if (enemyArcher != null)
        {
            isSugarRushing = enemyArcher.IsSugarRushing;
            if (isSugarRushing)
                speed = enemyArcher.BoltSpeed;
            else if (!isSugarRushing && speed < startSpeed)
                speed = startSpeed;
        }
        transform.position += direction * speed * Time.deltaTime;
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
    
}
