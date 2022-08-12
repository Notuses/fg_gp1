using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public enum JumpForceMode
{
    Force,
    Impulse
}

public enum CurrentMovementDirection
{
    Left,
    Right
}

public class PlayerController : MonoBehaviour
{
    #region Inspector Variables
    [Header("Components")]
    [SerializeField]Rigidbody2D rb;
    [SerializeField] private LayerMask walkableLayers;
    [SerializeField] private Volume postProcessing;
    
    [Header("Player stats")]
    [SerializeField] float moveSpeed = 10.0f;

    [Header("Jump settings")]
    //Used to figure out which type of jump feels best.
    [SerializeField, Tooltip("Forcemode.Force if checked, Forcemode.Impulse if un-checked")] JumpForceMode jumpMode = JumpForceMode.Force;
    [SerializeField, Tooltip("If forcemode is set to Force we need a big number 300+.")] float jumpPowerForce = 300f;
    [SerializeField, Tooltip("If set to Impulse it's how many units of power we give it, ~5-15")] float jumpPowerImpulse = 10.0f;
    [Space]
    [SerializeField, Tooltip("How long the player is allowed to jump after walking off an edge (Seconds)*Should be very small value*")] private float jumpForgivenessTime = 0.2f;
    [SerializeField, Tooltip("How long before the player lands that we start detecting jump input. (Seconds)*Should be very small value*")] private float jumpBufferLength = 0.1f;
    [SerializeField] private int maxJumps = 2;

    [Header("Dash settings")]
    //[SerializeField, Tooltip("Needs a big value 1000+")] float dashPowerForce = 2000f; //Used for old dash system
    [SerializeField, Tooltip("In seconds")] private float delayBetweenDashes = 1f;
    [SerializeField, Tooltip("In unity units")] private float dashDistance = 5f;
    [SerializeField, Tooltip("In unity units/second")] float dashSpeed;

    [Header("Sugar rush variables")] 
    [SerializeField] private float speedModifier = 0.1f;
    [SerializeField] private float rushTime = 4f;
    [SerializeField] private float maxSugarRush = 100f;
    [SerializeField] private float minSugarRushBeforeStart = 30f;
    [SerializeField] private AudioSource sugarRushBgAudioSource;
    #endregion
    
    #region private variables
    //Dash variables
    private float curDashDist;
    float fractionOfJourney;
    float distCovered;
    private bool isDashing = false;
    private float startTime = -10f;
    private Vector2 startPos;
    private Vector2 endPos;

    //Jumpump variables
    private float forgivenessTimer; //Variable to store how long we are allowed to jump after walking off a platform.
    private float jumpBufferTimer;
    private int resetMaxJumps;
    private bool isJumping = false;

    //Private dash variables
    private float timeOfLastDash;
    
    //Movement variables
    private float horMovement;
    private CurrentMovementDirection curDir; //Variable to check 
    
    //private components
    private BoxCollider2D myBoxCollider;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private AudioSource audioSource;
    private PlayerStats playerStats;

    //Knockback variables
    private bool isGettingKnockbacked = false;
    private float knockBackTime = 0.2f;
    
    //Sugar rush
    private float rushTimeModifier;
    private bool sugarRushIsActive = false;
    
    #endregion

    public delegate void OnSugarRushActivated(float speedModifier, float rushTime);
    public event OnSugarRushActivated SugarRushActivated;
    
    
    public bool IsGettingKnockbacked
    {
        get => isGettingKnockbacked;
        set => isGettingKnockbacked = value;
    }

    public float KnockBackTime
    {
        set => knockBackTime = value;
    }

    public int MaxJumps
    {
        set => maxJumps = value;
    }

    public float MaxSugarRush => maxSugarRush;

    private void Awake()
    {
        //animator test
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        playerStats = GetComponent<PlayerStats>();
        
        timeOfLastDash = Time.time;
        Physics2D.queriesStartInColliders = false; //Ignore own collider when raycasting.
        resetMaxJumps = maxJumps;
    }
    
    private void FixedUpdate()
    {
        //Make character look in the right direction & play walking sound if on ground
        PlayerMovementDirection();
        
        //Check if player is being knocked back from an enemy hit
        KnockbackCheck();
    }
    

    void Update()
    {
        //Get current horizontal movement
        horMovement = Input.GetAxisRaw("Horizontal");
        
        //Check if player is allowed to jump in the current frame
        CanPlayerJump(); 

        //Player jump
        CheckForJumpInput();

        //Player dash
        CheckForDashInput();

        //Check sugar rush
        CheckSugarRush();
        
    }

    private void CheckSugarRush()
    {
        if (Input.GetKeyDown(KeyCode.E) && CheckIfWeCanSugarRush())
        {
            Debug.Log("Rushing for: " + rushTime * rushTimeModifier + " seconds..");
            AudioManager.instance.SugarRushBGSound(sugarRushBgAudioSource, true); //Start Sugar rush background sound
            AudioManager.instance.PlayerActivatedSugarRush(audioSource); //Play sugar rush activation sound
            if (SugarRushActivated != null) SugarRushActivated(speedModifier, rushTime * rushTimeModifier);
        }
    }

    private bool CheckIfWeCanSugarRush()
    {
        float currentSugarRush = playerStats.SugarRush;
        if (currentSugarRush > minSugarRushBeforeStart && !sugarRushIsActive)
        {
            rushTimeModifier = playerStats.SugarRush / maxSugarRush;
            rushTimeModifier = Mathf.Clamp(rushTimeModifier, 0, 1);
            StartCoroutine(SugarRushPostProcessing(rushTime * rushTimeModifier));
            return true;
        }
        return false;
    }

    private void CheckForDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartDashing();
        }

        if (isDashing)
        {
            //Play dash sound and then keep dashing
            AudioManager.instance.PlayerDashSound(audioSource);
            KeepDashingUntilDistanceIsReached(startTime, startPos, endPos);
        }
    }

    private void CheckForJumpInput()
    {
        if ((jumpBufferTimer >= 0 && forgivenessTimer > 0f) || (Input.GetKeyDown(KeyCode.Space) && maxJumps > 0))
        {
            //If player is falling down we set the downward velocity to 0 so that the jump feels better even when we're falling down.
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
            
            DoJump();
            StartCoroutine(DecreaseJumpsRemainingAfterShortDelay()); //decrease jumps
            jumpBufferTimer = 0;
        }

        //Short jump if player releases space and is moving upwards
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            ShortJump();
        }
    }

    private void DoJump()
    {
        //Check if our current jumps (max jumps.. great name..) is equal to our max jumps (resetMaxJumps...)
        if (maxJumps == resetMaxJumps)
        {
            AudioManager.instance.PlayerJumpSound(audioSource);
        }
        else
        {
            anim.SetBool("doubleJump", true);
            AudioManager.instance.PlayerDoubleJumpSound(audioSource);
        }

        if (jumpMode == JumpForceMode.Force)
        {
            rb.AddForce(new Vector2(0, jumpPowerForce), ForceMode2D.Force);
        }
        else if (jumpMode == JumpForceMode.Impulse)
        {
            rb.AddForce(new Vector2(0, jumpPowerImpulse), ForceMode2D.Impulse);
        }
    }

    private void ShortJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }

    //Old dash (Very teleporty)
    /*private void DoDash()
    {
        if (Time.time > timeOfLastDash + delayBetweenDashes  )
        {
            timeOfLastDash = Time.time;
            rb.AddForce(new Vector2(dashPowerForce * horMovement, 0), ForceMode2D.Force);
        }
    }*/
    
    private void StartDashing()
    {
        if (Time.time > startTime + delayBetweenDashes && horMovement != 0f)
        {
            startTime = Time.time; //When we started the dash
            //Check if we will hit something during the dash
            //RaycastHit2D hit = Physics2D.Raycast(myBoxCollider.bounds.center, new Vector2(horMovement, 0), Mathf.Infinity, walkableLayers);
            RaycastHit2D hit = Physics2D.BoxCast(myBoxCollider.bounds.center, myBoxCollider.bounds.size * 0.95f, 0f,  new Vector2(horMovement, 0), Mathf.Infinity, walkableLayers);
            
            //Set the distance to dash to either go to the nearest wall or full dash distance.
            curDashDist = dashDistance;
            if (hit.collider != null)
            {
                if (dashDistance > hit.distance)
                {
                    curDashDist = hit.distance - myBoxCollider.size.x;
                }
            }else {
                curDashDist = dashDistance;
            }

            //Start and end of dash
            startPos = transform.position;
            endPos = new Vector2( transform.position.x + (horMovement * curDashDist), transform.position.y);

            //Reset distance covered (otherwise we can only dash once)
            distCovered = 0f;
            
            KeepDashingUntilDistanceIsReached(startTime, startPos, endPos);
        }
    }

    private void KeepDashingUntilDistanceIsReached(float startTime, Vector2 startPos, Vector2 endPos)
    {
        isDashing = true;
        
        anim.SetBool("isDashing", isDashing);
        if (distCovered >= curDashDist)
        {
            isDashing = false;
            anim.SetBool("isDashing", isDashing);
            rb.velocity = new Vector2(rb.velocity.x, 0f); //Reset downward velocity after dash otherwise gravity will be applied during the dash and we will have a high velocity when dash ends
            return;
        }

        distCovered = (Time.time - startTime) * dashSpeed;
        fractionOfJourney = distCovered / curDashDist;
        transform.position = Vector2.Lerp(startPos, endPos, fractionOfJourney);
    }
    
    private void CanPlayerJump()
    {
        //Jump after walking off an edge
        if (GroundCheck())
        {
            maxJumps = resetMaxJumps;
            forgivenessTimer = jumpForgivenessTime;
        }
        else
        {
            forgivenessTimer -= Time.deltaTime;
        }
        //Jump buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferLength;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    private bool GroundCheck()
    {
        //todo Either change this to a boxcast or do some kind of circle detection. This won't let the player jump if they stand too far out on a edge
        //bool isTouchingGround = Physics2D.IsTouchingLayers(myBoxCollider, walkableLayers); //Not great as we cant specify to only check under the player (Guess we could make a separate collider but meh)
        RaycastHit2D hit = Physics2D.BoxCast(myBoxCollider.bounds.center, myBoxCollider.bounds.size, 0f,  -Vector2.up, 0.1f, walkableLayers);

        if (hit.collider != null /*isTouchingGround*/)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("doubleJump", false);
            return true;
        }
        anim.SetBool("isWalking", false);
        anim.SetBool("isJumping", true);
        return false;
    }

    private void KnockbackCheck()
    {
        if (!isGettingKnockbacked)
        {
            //todo tweak this so it keeps velocity instead of just resetting it every update
            rb.velocity = new Vector2(horMovement * moveSpeed, rb.velocity.y);
        }
        else
        {
            StartCoroutine(KnockBackTimer());
        }
    }

    private void PlayerMovementDirection()
    {
        if (horMovement > 0.1 && curDir != CurrentMovementDirection.Right && !isDashing)
        {
            curDir = CurrentMovementDirection.Right;
            spriteRenderer.flipX = false;
            anim.SetBool("isWalking", true);

            if (GroundCheck())
            {
                AudioManager.instance.PlayerWalkingSounds(audioSource);
            }
        }
        else if (horMovement < -0.1 && curDir != CurrentMovementDirection.Left && !isDashing)
        {
            curDir = CurrentMovementDirection.Left;
            spriteRenderer.flipX = true;
            anim.SetBool("isWalking", true);
            if (GroundCheck())
            {
                AudioManager.instance.PlayerWalkingSounds(audioSource);
            }
        }
        else if (horMovement == 0 && !isDashing)
        {
            anim.SetBool("isWalking", false);
        }
        else if (!isDashing)
        {
            anim.SetBool("isWalking", true);
            if (GroundCheck())
            {
                AudioManager.instance.PlayerWalkingSounds(audioSource);
            }
        }
    }
    
    private void JumpAnimation()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isJumping", true);
        //anim.SetTrigger("isJumping");
    }
    
    IEnumerator SugarRushPostProcessing(float rushTimer)
    {
        float elapsedTime = 0;
        float fadeInTime = rushTimer / 5;

        sugarRushIsActive = true;
        while (elapsedTime < fadeInTime)
        {
            postProcessing.weight = Mathf.Lerp(0f, 1f, (elapsedTime / fadeInTime));
            elapsedTime += Time.deltaTime;
 
            // Yield here
            yield return null;
        }
        
        yield return new WaitForSeconds(rushTimer - (fadeInTime *2));
        
        elapsedTime = 0;
        while (elapsedTime < fadeInTime)
        {
            postProcessing.weight = Mathf.Lerp(1f, 0f, (elapsedTime / fadeInTime));
            elapsedTime += Time.deltaTime;
 
            // Yield here
            yield return null;
        }
        AudioManager.instance.SugarRushBGSound(sugarRushBgAudioSource, false); //Stop Sugar rush background sound
        sugarRushIsActive = false;
        postProcessing.weight = 0f;
    }
    
    IEnumerator KnockBackTimer()
    {
        yield return new WaitForSeconds(knockBackTime);
        isGettingKnockbacked = false;
    }
    
    IEnumerator DecreaseJumpsRemainingAfterShortDelay()
    {
        yield return new WaitForSeconds(0.1f);
        maxJumps--;
        JumpAnimation();
    }
    
}
