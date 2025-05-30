using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SimulationSetup : MonoBehaviour
{
    [Header ("Vital Sign Generators of Bed 1")]
    public GameObject LineRendererOxySat;
    public GameObject LineRendererRespR;
    public TextMeshProUGUI TextBloodPr;
    public GameObject LineRendererHeartR;

    [Header("Scenario Guiding Windows Information")]
    public GameObject GuideCanvas; 
    public TextMeshProUGUI GuideHeader;
    public TextMeshProUGUI GuideDescription;

    [Header("Illness-Specific Events")]
    public GameObject[] covid19;
    public GameObject[] heartf;
    public GameObject[] atrialf;
    public GameObject[] trachealC;
    public GameObject[] check12;
    public GameObject[] scenarioSpecificDevicesOFF;
    public int illness;

    [Header("Using Second Bed")]
    public GameObject[] ToSwitch;

    [Header ("Other Public Information")]
    public GameObject HospitalServer;
    public GameObject[] alarm;
    public Animator patient1;
    //public GameObject FoVCanvas;
    public bool[] a = new bool[] { false, false, false, false};
    public GameObject ind;
    public GameObject noLight;
    public GameObject redLight;

    int[] storage; //tracks how far into the scenario the player has progressed.
    int scenarioID;
    int[] scenarioLength;//how many guide windows do the scenarios use per scenarioID, defined in Awake()

    void Awake()
    {
        ind = GameObject.Find("Indestructible");        //Searching for the object that transports infos through scene changes
        
        if (ind.GetComponent<BackgroundInfo>().newBed == true && ind.GetComponent<BackgroundInfo>().overrule ==false)   //should the ReduSys Bed be used instead of the normal one?
        {
            useSecondBed();
        }

         scenarioLength = new int[] { 2, 6, 2, 2, 8 };                  //how many guide windows each scenario needs 

        if (ind.GetComponent<BackgroundInfo>().shortStorage != null)    //was there a scenario setting set? when choosing "Räumlichkeiten ansehen", there is none
        {
            storage = ind.GetComponent<BackgroundInfo>().shortStorage;
            
            if (storage.Length >= 4)
            {
                if (storage[0] != -1)
                {

                    if (storage[0] == 1 && storage[1] == 2 && storage[2] == 0 && storage[3] == 0) //Tracheal Cannula with Low Oxygen Saturation & Respiration Rate
                    {
                        scenarioID = 0;
                        HospitalServer.GetComponent<HospitalData>().redAlert = true;                        //There is an alarm active
                        LineRendererOxySat.GetComponent<VitalSignsSimulation>().OxySatNormal = false;       //The Oxygen Saturation of the paatient is not normal
                        LineRendererOxySat.GetComponent<VitalSignsSimulation>().OxySatCritical = 1;         //The range is indicated by the settings number 1, can be checked in VitalSignsSimulation
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRNormal = false;         //The Respiration Rate of the paatient is not normal
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRCritical = storage[1];  //The range is indicated by the settings number 2, can be checked in VitalSignsSimulation

                        a = new bool[] { true, true, false, false };                    //which alarms are active in this scenario

                        for (int i = 0; i < alarm.Length; i++)
                        {
                            alarm[i].gameObject.GetComponent<Alarm>().setAlarms(a);     //the alarms are set up

                        }
                        scenarioSpecificDevicesOFF[0].gameObject.SetActive(false);      //objects uniquely missing in this scenario are deactivated

                        displayGuide(0);                                                //the guide windows are initiated

                        storage[0] = 2;                                                 
                        if (storage[4] == 1)
                        {
                           // FoVCanvas.gameObject.SetActive(true);                       //if activated in the main menu settings, the FoV canvas is set active
                        }
                        HospitalServer.GetComponent<HospitalData>().redAlertPressed();  //the alarms are activated

                        illness = 4;                                                    
                    }                                                                   //description is analogue for the other scenarios

                    if (storage[0] == 1 && storage[1] == 3 && storage[2] == 2 && storage[3] == 0) //COVID-19 Patient
                    {
                        scenarioID = 1;
                        HospitalServer.GetComponent<HospitalData>().redAlert = true;
                        LineRendererOxySat.GetComponent<VitalSignsSimulation>().OxySatNormal = false;
                        LineRendererOxySat.GetComponent<VitalSignsSimulation>().OxySatCritical = 1;
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRNormal = false;
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRCritical = storage[1];
                        TextBloodPr.GetComponent<VitalSignsSimulation>().BloodPrNormal = false;
                        TextBloodPr.GetComponent<VitalSignsSimulation>().BloodPrCritical = storage[2];

                        a = new bool[] { true, true, true, false };

                        for (int i = 0; i < alarm.Length; i++)
                        {
                            alarm[i].gameObject.GetComponent<Alarm>().setAlarms(a);
                        }
                        scenarioSpecificDevicesOFF[3].gameObject.SetActive(false);
                        displayGuide(0);

                        storage[1] = 2;
                        if (storage[4] == 1)
                        {
                            //FoVCanvas.gameObject.SetActive(true);
                        }
                        HospitalServer.GetComponent<HospitalData>().redAlertPressed();

                        illness = 1;
                    }
                    if (storage[0] == 2 && storage[1] == 2 && storage[2] == 1 && storage[3] == 0) //Heart Failure Patient with Level 3 Alarm
                    {
                        scenarioID = 2;
                        HospitalServer.GetComponent<HospitalData>().redAlert = true;
                        LineRendererOxySat.GetComponent<VitalSignsSimulation>().OxySatNormal = false;
                        LineRendererOxySat.GetComponent<VitalSignsSimulation>().OxySatCritical = 2;
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRNormal = false;
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRCritical = storage[1];
                        TextBloodPr.GetComponent<VitalSignsSimulation>().BloodPrNormal = false;
                        TextBloodPr.GetComponent<VitalSignsSimulation>().BloodPrCritical = storage[2];

                        a = new bool[] { true, true, true, false };

                        for (int i = 0; i < alarm.Length; i++)
                        {
                            alarm[i].gameObject.GetComponent<Alarm>().setAlarms(a);
                        }

                        displayGuide(0);

                        storage[2] = 2;
                        if (storage[4] == 1)
                        {
                           // FoVCanvas.gameObject.SetActive(true);
                        }
                        HospitalServer.GetComponent<HospitalData>().thirdLevelAlertPressed();

                        illness = 2;

                    }
                    if (storage[0] == 0 && storage[1] == 2 && storage[2] == 0 && storage[3] == 2) //Atrial Fibrillation
                    {
                        scenarioID = 3;
                        HospitalServer.GetComponent<HospitalData>().redAlert = true;
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRNormal = false;
                        LineRendererRespR.GetComponent<VitalSignsSimulation>().RespRCritical = storage[1];
                        LineRendererHeartR.GetComponent<VitalSignsSimulation>().HeartRNormal = false;
                        LineRendererHeartR.GetComponent<VitalSignsSimulation>().HeartRCritical = storage[3];

                        a = new bool[] { false, true, false, true };

                        for (int i = 0; i < alarm.Length; i++)
                        {
                            alarm[i].gameObject.GetComponent<Alarm>().setAlarms(a);
                        }

                        displayGuide(0);

                        storage[3] = 2;
                        if (storage[4] == 1)
                        {
                            //FoVCanvas.gameObject.SetActive(true);
                        }
                        HospitalServer.GetComponent<HospitalData>().redAlertPressed();

                        illness = 3;
                    }
                    if (storage[0] == 0 && storage[1] == 0 && storage[2] == 0 && storage[3] == 0) //12 pm Inspection with Level 1 Alarm due to OxySat
                    {
                        scenarioID = 4;
                        for (int i = 0; i < check12.Length; i++)
                        {
                            check12[i].gameObject.SetActive(true);
                        }
                        LineRendererOxySat.GetComponent<VitalSignsSimulation>().turnOffMeasurement(5);
                        scenarioSpecificDevicesOFF[2].gameObject.SetActive(false);
                        a = new bool[] { true, false, false, false };

                        for (int i = 0; i < alarm.Length; i++)
                        {
                            alarm[i].gameObject.GetComponent<Alarm>().setAlarms(a);
                        }
                        displayGuide(0);
                        storage[4] = 2;
                        HospitalServer.GetComponent<HospitalData>().firstLevelAlertPressed();
                    }
                }
                else
                {               //using the last panel of the first scenario so there is a guide to leave in the free exploration mode of "Räumlichkeiten ansehen"
                    scenarioID = 0;
                    storage = new int[] { scenarioLength[0] };
                    displayGuide(storage[0] - 1);
                }
            }
            noLight.gameObject.SetActive(false);                    //there is always an alarm light
            redLight.gameObject.SetActive(true);

            switch (illness)                                        //objects that are unique to certain illnesses/scenarios are activated
            {
                case 0:
                    break;
                case 1:
                    for (int i = 0; i < covid19.Length; i++)
                    {
                        covid19[i].gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    for (int i = 0; i < heartf.Length; i++)
                    {
                        heartf[i].gameObject.SetActive(true);
                    }
                    break;
                case 3:
                    for (int i = 0; i < atrialf.Length; i++)
                    {
                        atrialf[i].gameObject.SetActive(true);
                    }
                    break;
                case 4:
                    for (int i = 0; i < trachealC.Length; i++)
                    {
                        trachealC[i].gameObject.SetActive(true);
                    }
                    break;
            }
        } 
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void displayGuide(int stepID) //loads position, rotation, header and description of the guide window
    {
        
        float x = this.GetComponent<ScenarioData>().scenarioPositions[stepID * 3 + scenarioID * 30];
        float y = this.GetComponent<ScenarioData>().scenarioPositions[stepID * 3 + 1 + scenarioID * 30];
        float z = this.GetComponent<ScenarioData>().scenarioPositions[stepID * 3 + 2 + scenarioID * 30];
        
        Vector3 newV = new Vector3(x, y, z);
       
        GuideCanvas.transform.position = newV;
        

        GuideCanvas.transform.eulerAngles = new Vector3(0, this.GetComponent<ScenarioData>().scenarioRotations[stepID + scenarioID * 10], 0);

        GuideHeader.GetComponent<TextMeshProUGUI>().text = this.GetComponent<ScenarioData>().scenarioHeader[stepID + scenarioID * 10];

        GuideDescription.GetComponent<TextMeshProUGUI>().text = this.GetComponent<ScenarioData>().scenarioDescription[stepID + scenarioID * 10];
    }

    public void nextGuide()
    {
        if(storage[scenarioID] - 1 < scenarioLength[scenarioID])        //after clicking one guide window, the next is opened
        {
            displayGuide(storage[scenarioID] - 1);
          
            storage[scenarioID]++;
        }
        else
        {
            Destroy(ind);                                               //if there is no next window, the background Indestructible object is destroyed because it is generated new in the menu...
            ind = null;
            SceneManager.LoadScene(0, LoadSceneMode.Single);            //...and the user returns to the main menu
            
        }
        
    }

    public void newGlobalAlarm(int sign) //boundaries changed, new alert must be triggered
    {
        a[sign] = false;
        for (int i = 0; i < alarm.Length; i++)
        {
            alarm[i].gameObject.GetComponent<Alarm>().setAlarms(a);
        }
        HospitalServer.GetComponent<HospitalData>().redAlertPressed();
    }

    void useSecondBed()
    {
        //positions that must be switched so the second bed is used through all scenarios
        for (int i = 0; i < ToSwitch.Length; i++)
        {
            Vector3 newV = ToSwitch[i+1].gameObject.transform.position;
            ToSwitch[i+1].gameObject.transform.position = ToSwitch[i].gameObject.transform.position;
            ToSwitch[i].gameObject.transform.position = newV;
            i++;
        }
        scenarioSpecificDevicesOFF[1].gameObject.SetActive(false); //measurement devices are not necessary
        scenarioSpecificDevicesOFF[2].gameObject.SetActive(false);
    }
}
