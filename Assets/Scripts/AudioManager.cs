using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemySound
{
    Aggro,
    Damage,
    Death,
    Attacking
}

public enum EnemyType
{
    Ghost,
    Zombie,
    Bat,
    Archer,
    Sawblade,
    NotAnEnemy
}

public enum MainTheme
{
    OutsideAudio,
    CaveAudio
}

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Main music")]
    [Header("Current list index: OutsideAudio | Cave Audio ")]
    [SerializeField] private List<AudioClip> mainMusic = new List<AudioClip>();
    [SerializeField, Tooltip("This audiosource is located on the main camera")] private AudioSource mainMusicSource;
    [SerializeField, Range(0f, 1f)] float mainMusicVolume = 0.5f; 

    [Header("Player")]
    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private AudioClip playerDamageTaken;
    [SerializeField] private AudioClip playerDashSound;
    [SerializeField] private AudioClip playerJumpSound;
    [SerializeField] private AudioClip playerDoubleJumpSound;
    [SerializeField] private AudioClip playerActivatedSugarRush;
    [SerializeField, Tooltip("The music/sound that plays while sugar rush is active")] private AudioClip sugarRushBGSound;
    [Space]
    [SerializeField] private List<AudioClip> playerWalkingSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> playerAttackStart = new List<AudioClip>();
    [SerializeField] private List<AudioClip> playerAttackImpact = new List<AudioClip>();
    [Space] 
    [Header("Player variables")]
    [SerializeField] private float timeBetweenFootsteps = 0.2f;
    [SerializeField, Range(0f, 1f)] float footstepsVolume = 0.5f;

    [Header("Pickups")] [SerializeField] private AudioClip pickupSound;
    
    [Space]
    [Header("Zombie")]
    [Header("Order of sounds: | Aggro | Damage |  Death | Attacking")]
    [SerializeField] private List<AudioClip> zombieSounds = new List<AudioClip>();
    
    [Header("Ghost")]
    [SerializeField] private List<AudioClip> ghostSounds = new List<AudioClip>();
    
    [Header("Bat")]
    [SerializeField] private List<AudioClip> batSounds = new List<AudioClip>();
    
    [Header("Archer")]
    [SerializeField] private List<AudioClip> archerSounds = new List<AudioClip>();
    
    [Header("Sawblade")]
    [SerializeField] private List<AudioClip> sawbladeSounds = new List<AudioClip>();


    //Private
    private bool curWalkSound = true;
    private bool curAttackSound = true;
    private bool curImpactSound = true;
    private float lastStepTime = 0;
    
    //Volume
    private float checkMainMusicVolumeChange;
    private float checkFootstepsVolumeChange;
    

    private void Start()
    {
        playMainMusic(MainTheme.OutsideAudio);
        checkMainMusicVolumeChange = mainMusicVolume;
        checkFootstepsVolumeChange = footstepsVolume;
    }

    private void FixedUpdate()
    {
        if (checkMainMusicVolumeChange != mainMusicVolume)
        {
            playMainMusic(MainTheme.OutsideAudio);
            checkMainMusicVolumeChange = mainMusicVolume;
        }
        
        if (checkFootstepsVolumeChange != footstepsVolume)
        {
            playMainMusic(MainTheme.OutsideAudio);
            checkFootstepsVolumeChange = footstepsVolume;
        }
    }

    public void EnemySounds(EnemyType type, AudioSource source, EnemySound soundToPlay)
    {
        switch (type)
        {
            case EnemyType.Zombie:
                ZombieSounds(source, soundToPlay);
                break;
            case EnemyType.Bat:
                BatSounds(source, soundToPlay);
                break;
            case EnemyType.Ghost:
                GhostSounds(source, soundToPlay);
                break;
            case EnemyType.Archer:
                ArcherSounds(source, soundToPlay);
                break;
            case EnemyType.Sawblade:
                SawbladeSounds(source, soundToPlay);
                break;
        }
    }

    public void playMainMusic(MainTheme musicIndex)
    {
        if (mainMusicSource.clip != mainMusic[(int)musicIndex])
        {
            mainMusicSource.volume = mainMusicVolume;
            mainMusicSource.clip = mainMusic[(int)musicIndex];
            mainMusicSource.Play();
        }
    }
    
    //Player audio
    public void PlayerWalkingSounds(AudioSource source)
    {
        source.volume = footstepsVolume;
        if (Time.time > lastStepTime + timeBetweenFootsteps)
        {
            if (curWalkSound)
            {
                source.clip = playerWalkingSounds[0];
                source.Play();
            }
            else
            {
                source.clip = playerWalkingSounds[1];
                source.Play();
            }
            curWalkSound = !curWalkSound;
            lastStepTime = Time.time;
        }
    }
    
    public void PlayerActivatedSugarRush(AudioSource source)
    {
        source.clip = playerActivatedSugarRush;
        source.Play();
    }
    
    public void SugarRushBGSound(AudioSource source, bool stop)
    {
        if (!stop)
        {
            source.Stop();
        }
        else
        {
            source.clip = sugarRushBGSound;
            source.Play();
        }
        
    }

    public void PlayerDamageTaken(AudioSource source)
    {
        source.clip = playerDamageTaken;
        source.Play();
    }
    
    public void PlayerDeathSound(AudioSource source)
    {
        source.clip = playerDeathSound;
        source.Play();
    }
    
    public void PlayerDashSound(AudioSource source)
    {
        if (source.clip != playerDashSound )
        {
            source.Stop();
        }
        
        if (!source.isPlaying)
        {
            source.clip = playerDashSound;
            source.Play();
        }
    }
    
    public void PlayerJumpSound(AudioSource source)
    {
        if (source.clip != playerJumpSound)
        {
            source.Stop();
        }
        
        if (!source.isPlaying)
        {
            source.clip = playerJumpSound;
            source.Play();
        }
    }
    
    public void PlayerDoubleJumpSound(AudioSource source)
    {
        if (source.clip != playerDoubleJumpSound)
        {
            source.Stop();
        }
        
        if (!source.isPlaying)
        {
            source.clip = playerDoubleJumpSound;
            source.Play();
        }
    }
    
    public void PlayerAttackStart(AudioSource source)
    {
        if (curAttackSound)
        {
            source.clip = playerAttackStart[0];
            source.Play();
        }
        else
        {
            source.clip = playerAttackStart[1];
            source.Play();
        }

        curAttackSound = !curAttackSound;
    }
    
    public void PlayerAttackImpact(AudioSource source)
    {
        if (curImpactSound)
        {
            source.clip = playerAttackImpact[0];
            source.Play();
        }
        else
        {
            source.clip = playerAttackImpact[1];
            source.Play();
        }

        curImpactSound = !curImpactSound;
    }
    
    //Archer
    public void PickupSound(AudioSource source)
    {
        if (!source.isPlaying)
        {
            source.clip = pickupSound;
            source.Play();
        }
    }

    //Zombie
    public void ZombieSounds(AudioSource source, EnemySound sound)
    {
        if (!source.isPlaying && (zombieSounds.Count > (int)sound) )
        {
            source.clip = zombieSounds[(int)sound];
            source.Play();
        }
    }
    
    //Ghost
    public void GhostSounds(AudioSource source, EnemySound sound)
    {
        if (!source.isPlaying && (ghostSounds.Count > (int)sound))
        {
            source.clip = ghostSounds[(int)sound];
            source.Play();
        }
    }
    
    //bat
    public void BatSounds(AudioSource source, EnemySound sound)
    {
        if (!source.isPlaying && (batSounds.Count > (int)sound))
        {
            source.clip = batSounds[(int) sound];
            source.Play();
        }
    }
    
    //Archer
    public void ArcherSounds(AudioSource source, EnemySound sound)
    {
        if (!source.isPlaying && (archerSounds.Count > (int)sound))
        {
            source.clip = archerSounds[(int) sound];
            source.Play();
        }
    }
    
    //Sawblade
    public void SawbladeSounds(AudioSource source, EnemySound sound)
    {
        if (!source.isPlaying && (sawbladeSounds.Count > (int)sound))
        {
            source.clip = sawbladeSounds[(int) sound];
            source.loop = true;
            source.Play();
        }
    }

}
