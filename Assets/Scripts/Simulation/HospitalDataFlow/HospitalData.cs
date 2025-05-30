using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HospitalData : MonoBehaviour           //script acting as the "Server" of the hospital system, currently only processing the alarm system
{
    public bool redAlert;
    public GameObject[] notifyOnAlert;              //array of objects that need to know whether the alarm is active

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void redAlertPressed()           //the standard alarm (level 2) ist activated
    {
        redAlert = true;

        for(int i = 0; i < notifyOnAlert.Length; i++)
        {
            notifyOnAlert[i].GetComponent<Alarm>().redAlertPressed();
        }
    }

    public void thirdLevelAlertPressed()  
    {
        redAlert = true;

        for (int i = 0; i < notifyOnAlert.Length; i++)
        {
            notifyOnAlert[i].GetComponent<Alarm>().thirdLevelAlertPressed();
        }
    }
    public void firstLevelAlertPressed()
    {
        redAlert = true;

        for (int i = 0; i < notifyOnAlert.Length; i++)
        {
            notifyOnAlert[i].GetComponent<Alarm>().firstLevelAlertPressed();
        }
    }

    public void greenEntryPressed()                //alarm is paused 30s
    {
        redAlert = false;

        for (int i = 0; i < notifyOnAlert.Length; i++)
        {
            notifyOnAlert[i].GetComponent<Alarm>().greenEntryPressed();
        }
    }

    public void LongPausePressed()                 //alarm is paused 3 min
    {
        redAlert = false;

        for (int i = 0; i < notifyOnAlert.Length; i++)
        {
            notifyOnAlert[i].GetComponent<Alarm>().LongPausePressed();
        }
    }

    public void firstLevelAlarmGone()              //the reason for the level 1 alarm has been resolved
    {
        redAlert = false;

        for (int i = 0; i < notifyOnAlert.Length; i++)
        {
            if (i != 3)
            {
                notifyOnAlert[i].GetComponent<Alarm>().greenEntryPressed();
            }
        }
    }
}
