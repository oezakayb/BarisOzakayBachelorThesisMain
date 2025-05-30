using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    PauseAction action;

    public bool paused = false;

    //public GameObject FoVWindow;

    private void Awake()
    {
        action = new PauseAction();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void Start()
    {
        action.Pause.PauseGame.performed += _ => DeterminePause();
    }

    private void DeterminePause()
    {
        if (paused == true)         //check whether the game state is currently "paused"
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;             //time "is not passing anymore"...
        paused = true;
        //FoVWindow.SetActive(true);      //...and the pause menu is opened
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        paused = false;
        //FoVWindow.SetActive(false);
    }
}
