using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : Enemy
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float distanceToPlayerBeforeMoving = 30f;

    //Components
    private SpriteRenderer sprRenderer;
    private MoveDir curDirection;
    private Vector3 pos, playerPos;
    private PlayerController playerController;
    private GameObject player;

    //Checks
    private bool hasAggro = false;
    private float distToPlayer;
    
    private float startSpeed;
    
    private void Awake()
    {
        startSpeed = moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        playerController = player.GetComponent<PlayerController>();
        sprRenderer = GetComponentInChildren<SpriteRenderer>();
        pos = transform.position;
        
        //Subscribes to the sugar rush event that is triggered from the player controller (a delegate whatever the fuck that is)
        //player.GetComponent<PlayerController>().SugarRushActivated += delegate(float speedModifier, float rushTime) {StartCoroutine(SugarRush(speedModifier, rushTime));  };
        playerController.SugarRushActivated += StartSugarRush;
    }

    private void FixedUpdate()
    {
        //Check distance to player and moves / sets the direction the enemy should face
        if (CheckDistToPlayer() && isAlive)
        {
            if (hasAggro == false)
            {
                AudioManager.instance.ZombieSounds(audioSource, EnemySound.Aggro);
            }
            hasAggro = true;
            MoveEnemy();
            DirToFace();
        }
        else if (isAlive)
        {
            hasAggro = false;
        }
    }

    void StartSugarRush(float speedModifier, float rushTime)
    {
        StartCoroutine(SugarRush(speedModifier, rushTime));
    }

    private bool CheckDistToPlayer()
    {
        playerPos = playerController.transform.position;
        pos = transform.position;
        distToPlayer = Vector3.Distance(playerPos, pos);

        if (distToPlayer < distanceToPlayerBeforeMoving)
        {
            return true;
        }

        return false;
    }

    private void MoveEnemy()
    {
        if (curDirection == MoveDir.Left)
        {
            //Move left
            rb.velocity = new Vector2(-1 * moveSpeed, rb.velocity.y);
        }
        else if (curDirection == MoveDir.Right)
        {
            //Move left
            rb.velocity = new Vector2(1 * moveSpeed, rb.velocity.y);
        }
    }

    private void DirToFace()
    {
        //Flips the sprite to always face the player
        if (pos.x > playerPos.x)
        {
            sprRenderer.flipX = true;
            curDirection = MoveDir.Left;
        }
        else if (pos.x < playerPos.x)
        {
            sprRenderer.flipX = false;
            curDirection = MoveDir.Right;
        }
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
