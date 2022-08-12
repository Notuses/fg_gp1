using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyGhost : Enemy
{

    
    [SerializeField, Tooltip("Ghost movement speed towards player.")]
    private float moveSpeed = 1.0f;

    [SerializeField, Tooltip("Ghost will only target player if inside the target radius.")][Range(0,20f)]
    private float targetRadius;
    
    private GameObject player;
    private PlayerController playerController;
    private SpriteRenderer sprRenderer;
    private Vector2 playerPos, ghostPos;

    private float startSpeed;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        playerController = player.GetComponent<PlayerController>();
        sprRenderer = GetComponent<SpriteRenderer>();
        //Subscribes to the sugar rush event that is triggered from the player controller (a delegate whatever the fuck that is)
        //player.GetComponent<PlayerController>().SugarRushActivated += delegate(float speedModifier, float rushTime) {StartCoroutine(SugarRush(speedModifier, rushTime));  };
        playerController.SugarRushActivated += StartSugarRush;

        startSpeed = moveSpeed;
    }

   void StartSugarRush(float speedModifier, float rushTime)
    {
        StartCoroutine(SugarRush(speedModifier, rushTime));
    }

    private void LateUpdate()
    {
        if (!isAlive)
            return;
        playerPos = player.transform.position;
        ghostPos = transform.position;
        Vector2 relPlayerPos = playerPos - ghostPos;
        float distSq = relPlayerPos.sqrMagnitude;
        bool isWithinRadius = distSq < targetRadius * targetRadius;

        DirToFace(); //Look towards player
        if (isWithinRadius)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(ghostPos, playerPos, step);
        }
    }

    private void DirToFace()
    {
        //Flips the sprite to always face the player
        if (ghostPos.x < playerPos.x)
        {
            sprRenderer.flipX = true;
        }
        else if (ghostPos.x > playerPos.x)
        {
            sprRenderer.flipX = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRadius);
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
