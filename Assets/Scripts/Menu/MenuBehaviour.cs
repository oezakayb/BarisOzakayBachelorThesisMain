using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuBehaviour : MonoBehaviour
{
    [Header("Common Information")]
    public int[] storage;
    public GameObject[] toShow;
    public GameObject[] toHide;
    public GameObject someGO;

    [Header("Specific Information")]
    public int targetScene;
    public GameObject indestructibleObject;
    public UILineRenderer uilr;
    public GameObject hospitalServer;
    public GameObject SimController;
    public GameObject LineRenderer;
    public bool overrule;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SceneChange()
    {
        if(targetScene == 0)
        {
            Destroy(SimController.gameObject.GetComponent<SimulationSetup>().ind);
            SimController.gameObject.GetComponent<SimulationSetup>().ind = null;
        }
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

    public void SceneChangeWithStorage()
    {
        if (indestructibleObject != null)
        {
            indestructibleObject.GetComponent<BackgroundInfo>().shortStorage[0] = storage[0];
            indestructibleObject.GetComponent<BackgroundInfo>().shortStorage[1] = storage[1];
            indestructibleObject.GetComponent<BackgroundInfo>().shortStorage[2] = storage[2];
            indestructibleObject.GetComponent<BackgroundInfo>().shortStorage[3] = storage[3];
        }
        if(overrule == true) //overrule the choice of the RedSys Bed because it does not make sense in this context
        {
            indestructibleObject.GetComponent<BackgroundInfo>().overrule = true;
        }

        /* 
         * 1-4: OxySat, RespR, BP and/or HR: 0 = normal, 1 = not normal, other int = special cases
         * 
         */
        
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

    public void menuTransition()        //manages all standard menu transitions
    {
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].SetActive(false);

        }

        for (int i = 0; i < toShow.Length; i++)
        {
            toShow[i].SetActive(true);

        }

    }

    public void menuTransitionWithStorage()     //menages all menu transitions that also have additional effects on the scene. they are accessed by their input in storage, set int the inspector
    {
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].SetActive(false);

        }

        for (int i = 0; i < toShow.Length; i++)
        {
            toShow[i].SetActive(true);

        }

        switch (storage[0]){
            case 1:         //Trigger Animations of Sitting NPC
                someGO.gameObject.GetComponent<SittingNPC>().Talking();
                break;
            case 2:         
                someGO.gameObject.GetComponent<SittingNPC>().stopTalking();
                break;
            case 3:         //Trigger Animations of the bed's railing
                someGO.gameObject.GetComponent<RailingAnim>().pullRailing();
                break;
            case 4: 
                someGO.gameObject.GetComponent<RailingAnim>().pushRailing();
                break;
            case 5:        //Turn off measurement of the different vital signs
                turnOffMeasurement(storage[1]);
                break;
            case 6:        //Turn on measurement of the different vital signs
                turnOnMeasurement(storage[1]);
                break;
            case 7:         //Trigger Animation of stationary oxyen device
                someGO.gameObject.GetComponent<MoveDeviceAnim>().pushDevice();
                break;
            case 8:         //Trigger Animation of stationary oxyen device 
                someGO.gameObject.GetComponent<MoveDeviceAnim>().pullDevice();
                break;
            case 9:         //turn off and on sound of Heart Rate Device
                someGO.gameObject.GetComponent<AudioSource>().mute = true;
                break;
            case 10:
                someGO.gameObject.GetComponent<AudioSource>().mute = false;
                break;
            case 11:        //remove and add attached Medical Device
                someGO.gameObject.SetActive(false);
                LineRenderer.gameObject.GetComponent<VitalSignsSimulation>().deviceRemoved(storage[1]);
                break;
            case 12:
                someGO.gameObject.SetActive(true);
                LineRenderer.gameObject.GetComponent<VitalSignsSimulation>().turnOnMeasurement(storage[1]);
                break;
            case 13:        //Play Sound
                this.GetComponent<AudioSource>().Play();
                break;
            case 14:        //Scenario 4 OxySat back on
                LineRenderer.gameObject.GetComponent<VitalSignsSimulation>().turnOnMeasurement(2);
                someGO.gameObject.GetComponent<Alarm>().firstLevelAlertOff();
                hospitalServer.GetComponent<HospitalData>().firstLevelAlarmGone();
                break;
            case 15:        //activate FoV Canvas
                indestructibleObject.gameObject.GetComponent<BackgroundInfo>().shortStorage[4] = 1;
                break;
            case 16:        //deactivate FoV Canvas
                indestructibleObject.gameObject.GetComponent<BackgroundInfo>().shortStorage[4] = 0;
                break;
            case 17:        //Trigger Animation of MovablePC
                someGO.gameObject.GetComponent<MovablePCAnim>().pushDevice();
                break;
            case 18:        //Main Menu settings
                if(indestructibleObject.gameObject.GetComponent<BackgroundInfo>().shortStorage[4] == 0)
                {
                    toShow[2].gameObject.SetActive(false);
                }
                else
                {
                    toShow[1].gameObject.SetActive(false);
                }
                if (indestructibleObject.gameObject.GetComponent<BackgroundInfo>().newBed == false)
                {
                    toShow[4].gameObject.SetActive(false);
                }
                else
                {
                    toShow[3].gameObject.SetActive(false);
                }
                break;
            case 19:        //activate ReduSys Bed
                indestructibleObject.gameObject.GetComponent<BackgroundInfo>().newBed = true;
                break;
            case 20:        //deactivate ReduSys Bed
                indestructibleObject.gameObject.GetComponent<BackgroundInfo>().newBed = false;
                break;
        }

    }

    public void randomGraphValues()
    {
        uilr.randomlyFillGraph();
    }
    
    public void setInactiveAndOpen()                            //open door
    {
        someGO.GetComponent<DoorAnim>().setInactiveAndOpen();
    }
    public void setInactiveAndClose()                           //close door
    {
        someGO.GetComponent<DoorAnim>().setInactiveAndClose();
    }

    public void activateSanitizer()
    {
        someGO.GetComponent<SanitizerAnim>().Activate();
    }

    public void redAlertPressed()                                           //all alarm-sources known to the HospitalServer are activated
    {
        menuTransition();                                                   //the door light is set to red
        hospitalServer.GetComponent<HospitalData>().redAlertPressed();
    }

    public void greenEntryPressed()                                         //all alarm-sources known to the HospitalServer are temporarily deactivated
    {
        menuTransition();                                                   //the door light is set to green
        hospitalServer.GetComponent<HospitalData>().greenEntryPressed();
    }

    public void LongPausePressed()                                          //all alarm-sources known to the HospitalServer are temporarily deactivated (3 minutes)
    {
        hospitalServer.GetComponent<HospitalData>().LongPausePressed();
    }

    public void nextScenarioGuide()                                         //Progress to the next scenario guide window
    {
        SimController.GetComponent<SimulationSetup>().nextGuide();
    }

    public void addOneToRange()                                                                     //the range of a boundary is increased
    {
        LineRenderer.gameObject.GetComponent<VitalSignsSimulation>().addOneToRange(storage[0]);
    }

    public void subtractOneFromRange()                                                              //the range of a boundary is decreased
    {
        LineRenderer.gameObject.GetComponent<VitalSignsSimulation>().subtractOneFromRange(storage[0]);
    }

    void turnOffMeasurement(int sign)       //enumeration of sign is visible in VitalSignsSimulation
    {
        LineRenderer.gameObject.GetComponent<VitalSignsSimulation>().turnOffMeasurement(sign);
    }

    void turnOnMeasurement(int sign)
    {
        LineRenderer.gameObject.GetComponent<VitalSignsSimulation>().turnOnMeasurement(sign);
    }
}
