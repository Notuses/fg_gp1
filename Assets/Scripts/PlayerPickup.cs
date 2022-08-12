using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private GameObject player;
    private PlayerStats playerStats;
    private PlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

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
            spriteRenderer.enabled = false;
            Destroy(gameObject);
        }
    }
}