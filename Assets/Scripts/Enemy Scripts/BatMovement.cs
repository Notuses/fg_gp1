using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MoveDir
{
    Left,
    Right
}

public class BatMovement : Enemy
{
    [SerializeField] private float moveSpeed = 5f;

    [Header("Enemy movement")]
    [SerializeField,Tooltip("How many units the enemy should move before going back")] private float distToMove = 10f;
    [SerializeField, Tooltip("How often the enemy should go up and down ")] private float waveFrequency = 10f;
    [SerializeField, Tooltip("How far up and down the enemy should go")] private float waveMagnitude = 10f;
    [SerializeField, Tooltip("Direction to start moving")] private MoveDir startDirection = MoveDir.Right;

    //Private variables
    private SpriteRenderer sprRenderer;
    private MoveDir curDirection;
    
    private Vector3 startPos, pos;

    private GameObject player;
    private PlayerController playerController;
    
    private float startSpeed;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + distToMove/2 - startPos.x, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - distToMove/2 + startPos.x, transform.position.y));
    }

    private void Awake()
    {
        startSpeed = moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        playerController = player.GetComponent<PlayerController>();
        sprRenderer = GetComponentInChildren<SpriteRenderer>();
        startPos = transform.position;
        pos = startPos;
        curDirection = startDirection;
        
        //Subscribes to the sugar rush event that is triggered from the player controller (a delegate whatever the fuck that is)
        //player.GetComponent<PlayerController>().SugarRushActivated += delegate(float speedModifier, float rushTime) {StartCoroutine(SugarRush(speedModifier, rushTime));  };
        playerController.SugarRushActivated += StartSugarRush;
    }

    private void Update()
    {
        if (!isAlive)
            return;
        DirToFace();
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        if (curDirection == MoveDir.Left)
        {
            //Move left
            pos -= new Vector3(1f, 0f,0f) * Time.deltaTime * moveSpeed;
            FollowSinWave();
        }
        else if (curDirection == MoveDir.Right)
        {
            //Move left
            pos += new Vector3(1f, 0f,0) * Time.deltaTime * moveSpeed;
            FollowSinWave();
        }
    }

    private void FollowSinWave()
    {
        transform.position = pos + transform.up * Mathf.Sin(Time.time * waveFrequency)* waveMagnitude;
    }

    private void DirToFace()
    {
        if (pos.x > (startPos.x + distToMove / 2))
        {
            sprRenderer.flipX = true;
            curDirection = MoveDir.Left;
        }
        else if (pos.x < (startPos.x - distToMove / 2))
        {
            sprRenderer.flipX = false;
            curDirection = MoveDir.Right;
        }
    }
    
    void StartSugarRush(float speedModifier, float rushTime)
    {
        StartCoroutine(SugarRush(speedModifier, rushTime));
    }

    private void OnDestroy()
    {
        playerController.SugarRushActivated -= StartSugarRush;
    }

    IEnumerator SugarRush(float speedModifier, float rushTime)
    {
        moveSpeed *= speedModifier;
        yield return new WaitForSeconds(rushTime);
        moveSpeed = startSpeed;
    }
}
