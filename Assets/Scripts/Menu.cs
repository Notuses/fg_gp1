using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator transition;
    [SerializeField] private float transiTiontime = 1f;
    public void PlayGame()
    {
        StartCoroutine(Loadlevel(SceneManager.GetActiveScene().buildIndex + 1));
        //Loads the first scene.
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
        //Quits application. 
    }

    IEnumerator Loadlevel(int LevelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transiTiontime);
        SceneManager.LoadScene(LevelIndex);
    }

}
