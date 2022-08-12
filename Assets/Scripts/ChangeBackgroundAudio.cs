using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBackgroundAudio : MonoBehaviour
{
    [SerializeField] private MainTheme soundToPlay;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.playMainMusic(soundToPlay);
        }
    }
}
