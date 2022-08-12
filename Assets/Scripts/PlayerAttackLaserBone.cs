using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerAttackLaserBone : MonoBehaviour
{
    
    
    [SerializeField]
    private GameObject bone;
    private LaserBone laserBone;
    private Vector3 atkDir;
    private Vector3 firstAtkDir;
    private AudioSource audioSource;
    
    private bool boneThrowOnCD;
    [Header("Bone throw general settings")]
    [SerializeField, Tooltip("Cooldown in seconds")]
    private float boneThrowCD = 1f;
    [SerializeField, Tooltip("Throw speed of the bone")]
    private float boneThrowSpeed = 10f;
    [SerializeField, Tooltip("Rotation speed of the bone")]
    private float boneRotationSpeed = 900f;
    [SerializeField, Tooltip("How long a bone lasts in the world in seconds")]
    private float boneLifeTime = 3f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        firstAtkDir = new Vector2(Input.GetAxisRaw("HorizontalAttack"), Input.GetAxisRaw("VerticalAttack"));
        if (!boneThrowOnCD)
        {
            if (firstAtkDir != Vector3.zero)
            {
                StartCoroutine(BoneThrow(boneThrowCD, firstAtkDir));
            }
        }
    }

    private IEnumerator BoneThrow(float delay, Vector3 firstAtkDir2)
    {
        boneThrowOnCD = true;
        AudioManager.instance.PlayerAttackStart(audioSource);
        yield return new WaitForSeconds(0.02f);
        GameObject boneThrown = Instantiate(bone, transform.position, Quaternion.identity);
        Destroy(boneThrown, boneLifeTime);
        laserBone = boneThrown.GetComponent<LaserBone>();
        atkDir = new Vector2(Input.GetAxisRaw("HorizontalAttack"),Input.GetAxisRaw("VerticalAttack"));
        if (atkDir == Vector3.zero)
            atkDir = firstAtkDir2;
        laserBone.StartIt(atkDir,boneThrowSpeed, boneRotationSpeed);
        yield return new WaitForSeconds(delay);
        boneThrowOnCD = false;
    }
    
}
