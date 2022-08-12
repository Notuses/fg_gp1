using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected float maxHealth = 3f;
    [SerializeField] protected float damageToPlayer = 1f;
    [SerializeField] protected float deathAnimationTimer = 1f;
    [SerializeField] protected GameObject deathAnimPrefab;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    
    protected AudioSource audioSource;
    
    private Collider2D myCollider;
    protected Rigidbody2D rb;
    private float health;
    protected bool isAlive = true;

    //Getters & setters
    public float DamageToPlayer => damageToPlayer;

    public EnemyType GetEnemyType => enemyType;

    private void Start()
    {
        myCollider = transform.GetComponent<Collider2D>();
        rb = transform.GetComponent<Rigidbody2D>();
        health = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (enemyType == EnemyType.Sawblade)
        {
            //todo find a way to get this to run from the "Sawblade.cs" start/awake func
            AudioManager.instance.EnemySounds(enemyType, audioSource, EnemySound.Aggro);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("PlayerAttack"))
        {
            //todo update the attack script so we can set variable damage
            //then we should be able to do something like other.collider.damageToDeal();
            health -= 1;
            if (health <= 0)
            {
                //todo inactivate the prefab and re-use it later instead of destroying
                spriteRenderer.enabled = false;
                GameObject deathAnim = Instantiate(deathAnimPrefab, transform.position, quaternion.identity);
                Destroy(deathAnim, 0.4f);
                
                AudioManager.instance.EnemySounds(enemyType, audioSource, EnemySound.Death);
                myCollider.enabled = false;
                if (rb != null)
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                isAlive = false;
                //Destroy the enemy if health is 0
                Destroy(gameObject, deathAnimationTimer);
            }
            else
                AudioManager.instance.EnemySounds(enemyType, audioSource, EnemySound.Damage);
        }
        if (other.collider.CompareTag("Player"))
        {
            AudioManager.instance.EnemySounds(enemyType, audioSource, EnemySound.Damage);
        }
    }

}
