using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    public GameObject[] alarmSymbol;
    public GameObject[] flickerOxySat;
    public GameObject[] flickerRespR;
    public GameObject[] flickerBloodPr;
    public GameObject[] flickerHeartR;
    public GameObject[] lineRenderers;
    public bool[] alarms = new bool[4];
    

    public GameObject hospitalServer;

    public bool hasSound;
    public AudioClip al1; //different alert levels
    public AudioClip al2;
    public AudioClip al3;

    public int alertLevel = 2;

    bool flickerNow = false;        //current states of UI alarm depictions
    bool allActive = false;
    bool longPause = false;
    bool beepNow = true;
    bool onAlarm = false;
    bool fixedProblem = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(flickerNow == true && onAlarm == true)              //continue the flicker effect
        {
            flickerNow = false;
            Flicker();
        }
        if(beepNow == true && hasSound == true && onAlarm == true)  //continue the beep sound
        {
            beepNow = false;
            beep();
        }
    }

    public void redAlertPressed()                               //standard level 2 alarm is activated
    {
        for(int i = 0; i<alarmSymbol.Length; i++)
        {

            alarmSymbol[i].gameObject.SetActive(true);
            
            
        }
        allActive = true;
        alertLevel = 2;
        onAlarm = true;
    }

    public void thirdLevelAlertPressed()                        //level 3 alarm is activated
    {
        for (int i = 0; i < alarmSymbol.Length; i++)
        {
            if (alarmSymbol[i].gameObject.name != "GreenDeAc")  //this alarm cannot be paused/temporarily deactivated
            {
                alarmSymbol[i].gameObject.SetActive(true);
            }
        }
        allActive = true;
        if (hasSound == true)
        {
            this.gameObject.GetComponent<AudioSource>().clip = al3;     //change the audio clip depending on alarm level
            this.gameObject.GetComponent<AudioSource>().volume += 0.1f;
        }
        
        alertLevel = 3;
        onAlarm = true;
    }

    public void firstLevelAlertPressed()                           //level 1 alarm is activated
    {
        for (int i = 0; i < alarmSymbol.Length; i++)
        {
             alarmSymbol[i].gameObject.SetActive(true);
            
        }
        allActive = true;
        if (hasSound == true)
        {
            this.gameObject.GetComponent<AudioSource>().clip = al1;     //change the audio clip depending on alarm level
            this.gameObject.GetComponent<AudioSource>().volume += 0.1f;
        }

        alertLevel = 1;
        onAlarm = true;
    }

    public void firstLevelAlertOff()                        //the problem initiating the level 1 alarm has been solved
    {
        for (int i = 0; i < alarmSymbol.Length; i++)
        {
            alarmSymbol[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < flickerOxySat.Length; i++)      //the flickering stops
        {
            flickerOxySat[i].gameObject.SetActive(true);
        }
        fixedProblem = true;
        allActive = true;
        onAlarm = false;
    }

    public void greenEntryPressed()
    {
        if(alertLevel == 3)                                 //unusable if the alarm is level 3
        {
            return;
        }
        for (int i = 0; i < alarmSymbol.Length; i++)
        {
            alarmSymbol[i].gameObject.SetActive(false);
        }
        
        for (int i = 0; i < flickerOxySat.Length; i++)      //the flickering stops
        {
            flickerOxySat[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < flickerRespR.Length; i++)
        {
            flickerRespR[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < flickerBloodPr.Length; i++)
        {
            flickerBloodPr[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < flickerHeartR.Length; i++)
        {
            flickerHeartR[i].gameObject.SetActive(true);
        }
        allActive = true;
        
        
        if (longPause == false)                     //what button lead to the pause of the alarm?
        {

            StartCoroutine(RestartAlert());
        }
        else
        {
            StartCoroutine(LongPause());
        }
        onAlarm = false;
    }

    public void LongPausePressed()
    {
        longPause = true;
        greenEntryPressed();
    }

    public void setAlarms(bool[] a)             //the alarms are set up at the beginning
    {
        alarms = a;
        flickerNow = true;
    }


    void Flicker()
    {
        StartCoroutine(FlickerRate());
    }
    void beep()
    {
        switch (alertLevel)                 //alarm levels have different frequencies
        {
            case 1:
                StartCoroutine(Level1());
                break;
            case 2:
                StartCoroutine(Level2());
                break;
            case 3:
                StartCoroutine(Level3());
                break;
        }
    }

    IEnumerator FlickerRate()
    {
        
        if(alarms[0] == true)       //check which UI elements have to flicker depending on the alarm array
        {
            for (int i = 0; i < flickerOxySat.Length; i++) //iterate through the array of elements for each vital sign
            {
                if (allActive == true)
                {
                    if(lineRenderers[0].gameObject.GetComponent<VitalSignsSimulation>().confirmFlicker(0) == true || (alertLevel == 1 && onAlarm == true)) //condition for level 1 alarm in scenario 4
                    {
                        flickerOxySat[i].gameObject.SetActive(false);
                    }
                    
                }
                else
                {
                    flickerOxySat[i].gameObject.SetActive(true);
                }
            }
        }
        if (alarms[1] == true)
        {
            for (int i = 0; i < flickerRespR.Length; i++)
            {
                if (allActive == true)
                {
                    if (lineRenderers[1].gameObject.GetComponent<VitalSignsSimulation>().confirmFlicker(1) == true)
                    {
                        flickerRespR[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    flickerRespR[i].gameObject.SetActive(true);
                }
            }
        }
        if (alarms[2] == true)
        {
            for (int i = 0; i < flickerBloodPr.Length; i++)
            {
                if (allActive == true)
                {
                    if (lineRenderers[2].gameObject.GetComponent<VitalSignsSimulation>().confirmFlicker(2) == true)
                    {
                        flickerBloodPr[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    flickerBloodPr[i].gameObject.SetActive(true);
                }
            }
        }
        if (alarms[3] == true)
        {
            for (int i = 0; i < flickerHeartR.Length; i++)
            {
                if (allActive == true)
                {
                    if (lineRenderers[3].gameObject.GetComponent<VitalSignsSimulation>().confirmFlicker(3) == true)
                    {
                        flickerHeartR[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    flickerHeartR[i].gameObject.SetActive(true);
                }
            }
        }
        if(allActive == true)       
        {
            allActive = false;      //all flickering objects are inactive now
        }
        else
        {
            allActive = true;       //all flickering objects are active now
        }

        yield return new WaitForSeconds(0.1f);  //flicker frequency

        flickerNow = true;
    }

    IEnumerator RestartAlert()                  //normal pause (green buttons)
    {
        yield return new WaitForSeconds(30);
        
        if (alertLevel == 2)
        {
            hospitalServer.gameObject.GetComponent<HospitalData>().redAlertPressed();
        }
        if (fixedProblem == false && hasSound == true)
        {
            if (alertLevel == 1)
            {
                hospitalServer.gameObject.GetComponent<HospitalData>().firstLevelAlertPressed();
            }
            onAlarm = true;
        }
    }

    IEnumerator LongPause()                     //longer pause only on the bedside monitor
    {
        yield return new WaitForSeconds(180);
        if (alertLevel == 2)                    //standard behaviour (level 2)
        {
            hospitalServer.gameObject.GetComponent<HospitalData>().redAlertPressed();
        }
        if (fixedProblem == false && hasSound == true)     //behaviour for level 1 alarm that was not fixed
        {
            if (alertLevel == 1)
            {
                hospitalServer.gameObject.GetComponent<HospitalData>().firstLevelAlertPressed();
            }
            onAlarm = true;
            longPause = false;
        }
    }

    IEnumerator Level1()
    {
        yield return new WaitForSeconds(3);
        this.gameObject.GetComponent<AudioSource>().Play();
        beepNow = true;
    }
    IEnumerator Level2()
    {
        yield return new WaitForSeconds(2);
        this.gameObject.GetComponent<AudioSource>().Play();
        beepNow = true;
    }
    IEnumerator Level3()
    {
        yield return new WaitForSeconds(1);
        this.gameObject.GetComponent<AudioSource>().Play();
        beepNow = true;
    }
}
