using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PickupsUpAndDown : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    [Header("Saw movement")]
    [SerializeField,Tooltip("How many units the enemy should move before going back")] private float distToMove = 1f;
    [SerializeField, Tooltip("Left = Down, Right = up")] private MoveDir startDirection = MoveDir.Right;

    //Private variables
    private MoveDir curDirection;
    private Vector3 startPos, pos;
    private GameObject player;
    private PlayerController playerController;
    
    private float startSpeed;

    private void Awake()
    {
        startSpeed = moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        playerController = player.GetComponent<PlayerController>();
        startPos = transform.position;
        pos = startPos;
        curDirection = startDirection;
        //REMEMBER TO LIKE AND >SUBSCRIBE<
        playerController.SugarRushActivated += StartSugarRush;
    }

    private void Update()
    {
        DirToFace();
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        if (curDirection == MoveDir.Left)
        {
            //Move down
            transform.position -= new Vector3(0f, 1f,0f) * Time.deltaTime * moveSpeed;
        }
        else if (curDirection == MoveDir.Right)
        {
            //Move up
            transform.position += new Vector3(0f, 1f,0) * Time.deltaTime * moveSpeed;
        }
    }

    private void DirToFace()
    {
        pos = transform.position;
        if (pos.y > (startPos.y + distToMove / 2))
        {
            curDirection = MoveDir.Left;
        }
        else if (pos.y < (startPos.y - distToMove / 2))
        {
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
