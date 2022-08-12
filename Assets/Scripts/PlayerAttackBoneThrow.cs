using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerAttackBoneThrow : MonoBehaviour
{
    [SerializeField]
    private GameObject bone;
    
    private float horAttack;
    private float verAttack;
    private Vector3 atkDir;
    
    //Bone throw attack
    private bool boneThrowOnCD;
    //General settings
    [Header("Bone throw general settings")]
    [SerializeField, Tooltip("Cooldown in seconds")]
    private float boneThrowCD = 1f;
    [SerializeField, Tooltip("How much force to apply for the bone throw")]
    private float boneThrowPower = 300f;
    [SerializeField, Tooltip("How long a bone lasts in the world in seconds")]
    private float boneLifeTime = 30f;
    [Space]
    
    //Throw direction throw modifiers
    [Header("Horizontal (↔)")]
    [SerializeField, Tooltip("Power modifier at horizontal throws")]
    private float horizontalThrowPowerModifier = 175f;
    [SerializeField, Tooltip("Left and right upwards throw horizontal modification")] [Range(0, 1)]
    private float horizontalThrowHorizontalMod = 1f;
    [SerializeField, Tooltip("Horizontal throw vertical modification")] [Range(-1, 1)]
    private float horizontalThrowVerticalMod = 0.2f;
    
    [Header("Upwards (↑)")]
    [SerializeField, Tooltip("Power modifier at upwards throws")]
    private float upwardsThrowPowerModifier = 130f;
    
    [Header("Upwards left and right (↖↗)")]
    [SerializeField, Tooltip("Power modifier at left and right upwards throws")]
    private float upwardsLeftRightThrowPowerModifier = 0f;

    [SerializeField, Tooltip("Left and right upwards throw horizontal modification")] [Range(0, 1)]
    private float upwardsLeftRightThrowHorizontalMod = 0.8f;
    [SerializeField, Tooltip("Left and right upwards throw vertical modification")] [Range(0, 1)]
    private float upwardsLeftRightThrowVerticalMod = 1f;

    [Header("Downwards (↓)")]
    [SerializeField, Tooltip("Power modifier at downwards throws")]
    private float downwardsThrowPowerModifier = 0f;
    
    [Header("Downwards left and right (↙↘)")]
    [SerializeField, Tooltip("Power modifier at left and right downwards throws")]
    private float downwardsLeftRightThrowPowerModifier = 0f;

    [SerializeField, Tooltip("Left and right downwards throw vertical modification")] [Range(-1, 0)]
    private float downwardsLeftRightThrowVerticalMod = -0.9f;
    [SerializeField, Tooltip("Left and right downwards throw vertical modification")] [Range(-1, 0)]
    private float downwardsLeftRightThrowHorizontalMod = -1f;
    
    
    private void LateUpdate()
    {
        if (Input.GetAxisRaw("HorizontalAttack") != 0 && !boneThrowOnCD || Input.GetAxisRaw("VerticalAttack") != 0 && !boneThrowOnCD)
        {
            StartCoroutine(BoneThrowCooldown(boneThrowCD));
            StartCoroutine(ThrowBoneAttack());
        }
    }

    private IEnumerator ThrowBoneAttack()
    {
        yield return new WaitForSeconds(0.02f);
        atkDir = new Vector2(Input.GetAxisRaw("HorizontalAttack"), Input.GetAxisRaw("VerticalAttack"));
        float powerModifier = PowerModifier(atkDir);
        GameObject thrownBone = Instantiate(bone, transform.position, Quaternion.Euler(0.0f,0.0f,Random.Range(0.0f, 360.0f)));
        thrownBone.GetComponent<Rigidbody2D>().AddForce(atkDir*(boneThrowPower+powerModifier));
        yield return new WaitForSeconds(0.1f);
        thrownBone.layer = 7; //Put bone inside same layer as player for collision
        yield return new WaitForSeconds(boneLifeTime);
        Destroy(thrownBone);
    }
    
    private float PowerModifier(Vector3 atkDirection)
    {
        float powerToAdd = 0;
        //Horizontal throw
        if (atkDirection.y == 0)
        {
            powerToAdd += horizontalThrowPowerModifier;
            if (atkDirection.x == -1)
                atkDir.x = -horizontalThrowHorizontalMod;
            else
                atkDir.x = horizontalThrowHorizontalMod;
            atkDir.y = horizontalThrowVerticalMod;
        }
        //up throw
        if (atkDirection.x == 0 && atkDir.y == 1)
        {
            powerToAdd = upwardsThrowPowerModifier;
        }
        //down throw
        if (atkDirection.x == 0 && atkDir.y == -1)
        {
            powerToAdd = downwardsThrowPowerModifier;
        }
        //up left and up right
        if (atkDirection.x == -1 && atkDirection.y == 1 || atkDirection.x == 1 && atkDirection.y == 1)
        {
            powerToAdd = upwardsLeftRightThrowPowerModifier;
            if (atkDirection.x == -1)
                atkDir.x = -upwardsLeftRightThrowHorizontalMod;
            else
                atkDir.x = upwardsLeftRightThrowHorizontalMod;
            atkDir.y = upwardsLeftRightThrowVerticalMod;
        }
        //down left and down right
        if (atkDirection.x == -1 && atkDirection.y == -1 || atkDirection.x == 1 && atkDirection.y == -1)
        {
            powerToAdd = downwardsLeftRightThrowPowerModifier;
            if (atkDirection.x == -1)
                atkDir.x = downwardsLeftRightThrowHorizontalMod;
            else
                atkDir.x = -downwardsLeftRightThrowHorizontalMod;
            
            atkDir.y = downwardsLeftRightThrowVerticalMod;
        }

        return powerToAdd;
    }
    
    private IEnumerator BoneThrowCooldown(float delay)
    {
        boneThrowOnCD = true;
        yield return new WaitForSeconds(delay);
        boneThrowOnCD = false;
    }

    private void OnDrawGizmos()
    {
        //Horizontal left and right
        DrawRays(-transform.right, horizontalThrowHorizontalMod, horizontalThrowVerticalMod,Color.red);
        DrawRays(transform.right, -horizontalThrowHorizontalMod, horizontalThrowVerticalMod, Color.red);
        //Upwards left and right
        DrawRays(new Vector2(-transform.right.x,transform.up.y), upwardsLeftRightThrowHorizontalMod,upwardsLeftRightThrowVerticalMod, Color.green);
        DrawRays(new Vector2(transform.right.x,transform.up.y), -upwardsLeftRightThrowHorizontalMod, upwardsLeftRightThrowVerticalMod, Color.green);

        //Downwards left and right
        DrawRays(new Vector2(-transform.right.x,-transform.up.y),downwardsLeftRightThrowHorizontalMod, downwardsLeftRightThrowVerticalMod, Color.yellow);
        DrawRays(new Vector2(transform.right.x,-transform.up.y), -downwardsLeftRightThrowHorizontalMod, downwardsLeftRightThrowVerticalMod, Color.yellow);

    }
    private void DrawRays(Vector2 toPos, float modifierHor, float modifierVer, Color color)
    {
        toPos.x = modifierHor;
        toPos.y = modifierVer;
        Gizmos.color = color;
        Gizmos.DrawRay(transform.position, toPos); 
    }
}
