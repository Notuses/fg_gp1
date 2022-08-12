using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;

public class RandomPlayerPickup : MonoBehaviour
{
    private GameObject player;
    private PlayerStats playerStats;
    private PlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private BoxCollider2D collider;
    private Light2D light;

    [Header("Components")] [SerializeField]
    private List<Sprite> pickupSprites;

    [SerializeField, Tooltip("Amount of points gained on pickup")]
    private float points;
    [SerializeField, Tooltip("Amount of sugar rush gained on pickup")]
    private float sugarRush;
    [SerializeField, Tooltip("Amount of health gained on pickup")]
    private float healing;
    
    [SerializeField, Tooltip("Remove item when picked up")]
    private bool removeOnPickUp = true;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.transform.GetChild(0).gameObject.GetComponent<PlayerStats>();
        playerHealth = player.transform.GetChild(0).gameObject.GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<BoxCollider2D>();
        light = GetComponent<Light2D>();
    }

    private void Start()
    {
        spriteRenderer.sprite = pickupSprites[Random.Range(0, pickupSprites.Count - 1)];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Play sound on pickup
        AudioManager.instance.PickupSound(audioSource);
        //Remove on pickup
        //add points/sugar rush to PlayerStats of player
        if (points > 0)
            playerStats.Points += points;
        if (sugarRush > 0)
            playerStats.SugarRush += sugarRush;
        if (healing > 0)
            playerHealth.HealPlayer(healing);
        //Todo make ui stuff for this
        if (points > 0 || sugarRush > 0)
            playerStats.UpdateUI();
        
        if (removeOnPickUp)
        {
            collider.enabled = false;
            light.enabled = false;
            spriteRenderer.enabled = false;
            Destroy(gameObject,0.5f);
        }
    }
}