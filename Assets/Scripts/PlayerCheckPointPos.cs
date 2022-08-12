using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckPointPos : MonoBehaviour
{
    private CheckPointManager checkPointManager;
    private void Start()
    {
        checkPointManager = GameObject.FindGameObjectWithTag("CheckPoint").GetComponent<CheckPointManager>();
        transform.position = checkPointManager.checkPointPos;
    }


}
