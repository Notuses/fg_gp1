using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyArcher : Enemy
{

    
    [SerializeField, Tooltip("Bolt speed towards player.")]
    private float boltSpeed = 1.0f;
    [SerializeField] private Vector2 boltOffsetVector;

    [SerializeField, Tooltip("Archer will only target player if inside the target radius.")][Range(0,20f)]
    private float targetRadius;
    
    [SerializeField, Tooltip("Archer attack cooldown.")]
    private float attackCooldown;
    [SerializeField, Tooltip("Bolt lifetime.")]
    private float boltLifetime;

    private bool canAtk = true;

    [SerializeField] 
    private GameObject bolt;

    private GameObject player;
    private PlayerController playerController;
    private SpriteRenderer sprRenderer;
    private Vector2 playerPos, archerPos;
    private float startSpeed;
    private Animator anim;
    
    private bool isSugarRushing;
    public bool IsSugarRushing => isSugarRushing;
    public float BoltSpeed => boltSpeed;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        playerController = player.GetComponent<PlayerController>();
        sprRenderer = GetComponent<SpriteRenderer>();
        //Subscribes to the sugar rush event that is triggered from the player controller (a delegate whatever the fuck that is)
        //player.GetComponent<PlayerController>().SugarRushActivated += delegate(float speedModifier, float rushTime) {StartCoroutine(SugarRush(speedModifier, rushTime));  };
        playerController.SugarRushActivated += StartSugarRush;
        

        startSpeed = boltSpeed;
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
        archerPos = transform.position;
        Vector2 relPlayerPos = playerPos - archerPos;
        float distSq = relPlayerPos.magnitude;
        bool isWithinRadius = distSq < targetRadius;
       

        DirToFace(); //Look towards player
        if (isWithinRadius && canAtk)
        {
            Vector2 dir = playerPos - (Vector2)transform.position;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + boltOffsetVector, dir);
            if (hit.collider != null && hit.collider.transform.CompareTag("Player"))
            {
                canAtk = false;
                ShootXbow(dir);
            }
        }
    }

    private void ShootXbow(Vector3 dir)
    {
        Vector2 boltDir = dir;
        float angle = Mathf.Atan2(boltDir.y, boltDir.x) * Mathf.Rad2Deg;
        GameObject boltShot = Instantiate(bolt, (Vector2)transform.position + boltOffsetVector, Quaternion.AngleAxis(angle, Vector3.forward));
        boltShot.GetComponent<Bolt>().StartIt(boltDir, boltSpeed, startSpeed, gameObject);
        StartCoroutine(AtkCooldown(attackCooldown, boltShot));
    }
    

    private void DirToFace()
    {
        //Flips the sprite to always face the player
        if (archerPos.x < playerPos.x)
        {
            sprRenderer.flipX = true;
        }
        else if (archerPos.x > playerPos.x)
        {
            sprRenderer.flipX = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + boltOffsetVector, 0.5f);

    }

    private void OnDestroy()
    {
        playerController.SugarRushActivated -= StartSugarRush;
    }

    IEnumerator SugarRush(float speedModifier, float rushTime)
    {
        boltSpeed *= speedModifier;
        isSugarRushing = true;
        yield return new WaitForSeconds(rushTime);
        boltSpeed = startSpeed;
        isSugarRushing = false;
    }

    IEnumerator AtkCooldown(float cd, GameObject boltShot)
    {
        anim.SetBool("isShooting", true);
        yield return new WaitForSeconds(cd);
        anim.SetBool("isShooting", false);
        canAtk = true;
        yield return new WaitForSeconds(boltLifetime-cd);
        Destroy(boltShot);
    }
}
