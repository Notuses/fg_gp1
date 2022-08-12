using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float sugarRushPerSecond = 3f;
    [SerializeField] private Image sugarRushBar;
    [SerializeField] TextMeshProUGUI pointsCounter;

    private PlayerController playerController;
    //Vars for sugarrush and points
    private float maxSugarRush;
    private float sugarRush;
    private float points;
    private bool isRushing = false;
    
    //Vars for lerping the bar
    private float lerp;
    private static float t = 0.0f;
    private float sugarRushLastUpdate = 0f;
    float timeToDepleteBar;


    public float SugarRush
    {
        get => sugarRush;
        set => sugarRush = value;
    }

    public float Points
    {
        get => points;
        set => points = value;
    }

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.SugarRushActivated += SugarRushStarted;
    }

    private void Start()
    {
        maxSugarRush = playerController.MaxSugarRush;
        StartCoroutine(GivePlayerSugarRushEverySecond());
    }

    private void Update()
    {
        UpdateSugarRushBar();
    }

    private void UpdateSugarRushBar()
    {
        //todo I think this needs to be fixed, this seems like a very ghetto solution
        if (!isRushing)
        {
            if (Math.Abs(sugarRushLastUpdate - sugarRush) > 0.1f)
            {
                sugarRushLastUpdate = sugarRush;
                t = 0;
            }
            lerp = Mathf.Lerp(sugarRushLastUpdate - sugarRushPerSecond, sugarRush, t);
            t += Time.deltaTime;
            sugarRushBar.fillAmount = Mathf.Clamp01(lerp / maxSugarRush);
        }
        else
        {
            sugarRush = (sugarRush > maxSugarRush) ? maxSugarRush : sugarRush; //Make sure we never lerp from a value bigger than the bar
            lerp = Mathf.Lerp(sugarRush, 0, t);
            t += Time.deltaTime/timeToDepleteBar;
            sugarRushBar.fillAmount = Mathf.Clamp01(lerp / maxSugarRush);
        }
    }

    //todo Make UI for this
    public void UpdateUI()
    {
        pointsCounter.text = points.ToString();
    }

    void SugarRushStarted(float _ , float rushTime)
    {
        StartCoroutine(CurrentlyRushing(rushTime));
    }

    IEnumerator CurrentlyRushing(float time)
    {
        t = 0;
        timeToDepleteBar = time;
        isRushing = true;
        yield return new WaitForSeconds(time);
        sugarRush = 0;
        isRushing = false;
    }

    IEnumerator GivePlayerSugarRushEverySecond()
    {
        //todo might be nice to not have a while true
        while (true)
        {
            //Only give player sugar when they're not currently using sugar rush
            if (!isRushing)
            {
                sugarRush += sugarRushPerSecond;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
