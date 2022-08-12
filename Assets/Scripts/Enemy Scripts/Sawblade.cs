using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Sawblade : Enemy
{
    [SerializeField] private float moveSpeed = 5f;

    [Header("Saw movement")]
    [SerializeField,Tooltip("How many units the enemy should move before going back")] private float distToMove = 10f;
    [SerializeField, Tooltip("Direction to start moving (If up and down: Left = Down, Right = up)")] private MoveDir startDirection = MoveDir.Right;
    [SerializeField] private bool upAndDown = false;

    //Private variables
    private SpriteRenderer sprRenderer;
    private MoveDir curDirection;
    private Vector3 startPos, pos;
    private GameObject player;
    private PlayerController playerController;

    private float startSpeed;

    private void OnDrawGizmos()
    {
        if (upAndDown)
        {
            Gizmos.DrawLine(transform.position, new Vector2(transform.position.x , transform.position.y + distToMove/2 - startPos.y));
            Gizmos.DrawLine(transform.position, new Vector2(transform.position.x , transform.position.y - distToMove/2 + startPos.y));
        }
        else
        {
            Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + distToMove/2 - startPos.x, transform.position.y));
            Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - distToMove/2 + startPos.x, transform.position.y));
        }
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
        DirToFace();
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        
        if (curDirection == MoveDir.Left && !upAndDown)
        {
            //Move left
            transform.position -= new Vector3(1f, 0f,0f) * Time.deltaTime * moveSpeed;
        }
        else if (curDirection == MoveDir.Right && !upAndDown)
        {
            //Move left
            transform.position += new Vector3(1f, 0f,0) * Time.deltaTime * moveSpeed;
        }
        
        if (curDirection == MoveDir.Left && upAndDown)
        {
            //Move down
            transform.position -= new Vector3(0f, 1f,0f) * Time.deltaTime * moveSpeed;
        }
        else if (curDirection == MoveDir.Right && upAndDown)
        {
            //Move up
            transform.position += new Vector3(0f, 1f,0) * Time.deltaTime * moveSpeed;
        }
        
        
    }

    private void DirToFace()
    {
        pos = transform.position;
        if (pos.x > (startPos.x + distToMove / 2) && !upAndDown)
        {
            sprRenderer.flipX = true;
            curDirection = MoveDir.Left;
        }
        else if (pos.x < (startPos.x - distToMove / 2) && !upAndDown)
        {
            sprRenderer.flipX = false;
            curDirection = MoveDir.Right;
        }
        
        if (pos.y > (startPos.y + distToMove / 2) && upAndDown)
        {
            sprRenderer.flipX = true;
            curDirection = MoveDir.Left;
        }
        else if (pos.y < (startPos.y - distToMove / 2) && upAndDown)
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
