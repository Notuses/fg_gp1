using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float playerMaxHealth = 3f;
    [SerializeField] private float playerHealth = 3f;
    [SerializeField] private List<Sprite> healthPictures = new List<Sprite>();

    [Header("Taking damage")] 
    [SerializeField] private float knockbackPower = 5f;
    [SerializeField] private float knockbackTime = 0.35f;

    [Header("Components")]
    [SerializeField] private PlayerController pcontroller;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Image playerHealthImage;

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private float maxHealth;

  

    private void Awake()
    {
        playerHealthImage.sprite = healthPictures[(int)playerHealth];
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        pcontroller.KnockBackTime = knockbackTime;
        maxHealth = playerHealth;
    }

    public void HealPlayer(float heal)
    {
        if (playerHealth < maxHealth)
        {
            if (playerHealth + heal > playerMaxHealth)
                playerHealth = playerMaxHealth;
            else
                playerHealth += heal;
            //Todo remove this
            print("Healed: " + heal + " | Current health: " + playerHealth);
            UpdateHealthImage();
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        UpdateHealthImage(); //todo put this into a check if we're colliding with a health pickup  
        
        if (other.collider.CompareTag("Enemy"))
        {
            AudioManager.instance.PlayerDamageTaken(audioSource);
            playerHealth -= (playerHealth - other.gameObject.GetComponent<Enemy>().DamageToPlayer) >= 0 ? other.gameObject.GetComponent<Enemy>().DamageToPlayer : playerHealth; //Makes the HP never go into the negatives.
            KnockbackPlayer(other.transform.position);
            UpdateHealthImage();
            
            if (playerHealth <= 0)
            {
                StartCoroutine(StartDeathSequence());
            }
        }

        if (other.collider.CompareTag("Enemy") && other.gameObject.GetComponent<Enemy>().GetEnemyType == EnemyType.NotAnEnemy)
        {
            pcontroller.MaxJumps = 2;
        }
        
    }

    private void UpdateHealthImage()
    {
        playerHealthImage.sprite = healthPictures[(int) playerHealth];
    }

    private void KnockbackPlayer(Vector3 targetPos)
    {
        if (transform.position.x > targetPos.x)
        {
            Debug.Log("Player is on the right");
            rb.AddForce( new Vector2(1f, 1f) * knockbackPower, ForceMode2D.Impulse);
            pcontroller.IsGettingKnockbacked = true;
        }
        else
        {
            Debug.Log("Player is on the left");
            rb.AddForce( new Vector2(-1f, 1f) * knockbackPower, ForceMode2D.Impulse);
            pcontroller.IsGettingKnockbacked = true;
        }
    }

    IEnumerator StartDeathSequence()
    {
        pcontroller.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        deathScreen.SetActive(true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => Input.anyKeyDown);
        LevelManager.instance.RestartLevel();
    }
    
}
