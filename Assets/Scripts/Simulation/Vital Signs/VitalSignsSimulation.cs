using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VitalSignsSimulation : MonoBehaviour
{
    [Header ("Line Renderers / Text Objects for this Vital Sign")]
    public GameObject LineRenderer;
    public GameObject[] ExternalLineRenderers;
    public TextMeshProUGUI textObject;
    public TextMeshProUGUI[] ExternalTextObjects;
    public TextMeshProUGUI range1;
    public TextMeshProUGUI range2;
    public TextMeshProUGUI range3;
    public TextMeshProUGUI range4;
    public TextMeshProUGUI largeNum;
    public TextMeshProUGUI[] ExternalLargeNums;

    [Header ("Refresh Settings")]
    bool refreshNeccessary = false;
    public float refreshRate;

    [Header("Oxygen Saturation")]
    public bool OxySatNormal;
    public int OxySatCritical;
    public int CurrentOxySat = -1;
    public float CurrentOxySatConverted = -1;
    public List<Vector2> OxySatVector;
    public int oxySatLower = 90;
    public bool OxySatTurnedOff;
    public bool OxySatTurnedOffhard;
    public bool OxySatUnknown;

    [Header("Respiration Rate")]
    public bool RespRNormal;
    public int RespRCritical;
    public int CurrentRespR = -1;
    public float CurrentRespRConverted = -1;
    public List<Vector2> RespRVector;
    public int chanceToChangeRespR;
    public int respRUpper = 18;
    public int respRLower = 12;
    public bool RespRTurnedOff;

    [Header("Blood Pressure")]
    public bool BloodPrNormal;
    public int BloodPrCritical;
    public int CurrentBloodPr1 = -1;
    public int CurrentBloodPr2 = -1;
    public int chanceToChangeBloodPr;
    public int bloodPr1Upper = 120;
    public int bloodPr1Lower = 90;
    public int bloodPr2Upper = 80;
    public int bloodPr2Lower = 60;
    public bool BloodPrTurnedOff;
    public bool BloodPrTurnedOffhard;
    public bool BloodPrUnknown;

    [Header("Heart Rate")]
    public bool HeartRNormal;
    public int HeartRCritical;
    public int CurrentHeartR = -1;
    public List<Vector2> HeartRVector;
    public int heartRUpper = 100;
    public int heartRLower = 60;
    public AudioSource HeartMonitor;
    public bool HeartRTurnedOff;
    int step = 0;

    [Header("Other Public Information")]
    public GameObject StationaryDevice;
    public GameObject CuffWindow;
    public GameObject FingertipDeviceWindow;
    public GameObject SimulationController;
    bool justPressed;


    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        
        StartCoroutine(RefreshRate());          //actualization rate of the graph
    }

    
    void Update()
    {

        if(refreshNeccessary == true)           //a new point has to be generated on the graph
        {
            refreshNeccessary = false;
            Refresh();
        }
        if (OxySatNormal == true || OxySatCritical > 0)                                 //display the current boundaries on the screen
        {
            range1.GetComponent<TextMeshProUGUI>().text = oxySatLower + "-100%";
        }
        if (RespRNormal == true || RespRCritical > 0)
        {
            range1.GetComponent<TextMeshProUGUI>().text = respRLower + "-";
            range2.GetComponent<TextMeshProUGUI>().text = respRUpper + " AZ / min";
        }
        if (BloodPrNormal == true || BloodPrCritical > 0)
        {
            range1.GetComponent<TextMeshProUGUI>().text = bloodPr1Lower + "-";
            range2.GetComponent<TextMeshProUGUI>().text = bloodPr1Upper +"";
            range3.GetComponent<TextMeshProUGUI>().text = bloodPr2Lower + "-";
            range4.GetComponent<TextMeshProUGUI>().text = bloodPr2Upper +"";
        }
        if (HeartRNormal == true || HeartRCritical > 0)
        {
            range1.GetComponent<TextMeshProUGUI>().text = heartRLower + "-";
            range2.GetComponent<TextMeshProUGUI>().text = heartRUpper + " Schläge / min";
        }
 
    }

    public void OxySatNormalSim() //Simulating normal Oxygen Saturation
    {
        if (CurrentOxySat == -1) //first second of the sim
        {

            int randomNumber = Random.Range(oxySatLower, 100);                              //generate a random value in the accepted boundaries
            CurrentOxySat = randomNumber;
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();     //update all integer representations of the value
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();
            }

            randomNumber -= 80;
            CurrentOxySatConverted = (float) randomNumber / 2;

            OxySatVector.Add(new Vector2(12, CurrentOxySatConverted));                      

        }
        else
        {                                                                                   //every later step
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();     //update all integer representations of the value
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();
            }
            OxySatVector.Add(new Vector2(12, CurrentOxySatConverted));
        }
        
        OxySatVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(OxySatVector);   //a new point is added to the vectors of the effected line renderers

        for ( int i = 0; i< ExternalLineRenderers.Length; i++)                              
        {
            OxySatVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(OxySatVector);
        }

    }

    public void OxySatCriticalSim(int mode) //Simulating a Oxygen Saturation that is critical
    {
        if (CurrentOxySat == -1) //first second of the sim
        {
            int randomNumber = 0;
            switch (mode)          //what issue is affecting the patient
            {
                case 1:
                    randomNumber = Random.Range(80, 90);
                    break;
                case 2:
                    randomNumber = Random.Range(85, 96);
                    break;
            }
            
            CurrentOxySat = randomNumber;
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();     //update all integer representations of the value
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();
            }

            randomNumber -= 80;
            CurrentOxySatConverted = (float)randomNumber / 2;

            OxySatVector.Add(new Vector2(12, CurrentOxySatConverted));

        }
        else
        {                                                                                   //every later step
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();     //update all integer representations of the value
            for (int i = 0; i < ExternalLargeNums.Length; i++)                              
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentOxySat).ToString();
            }
            OxySatVector.Add(new Vector2(12, CurrentOxySatConverted));
        }

        OxySatVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(OxySatVector);   //a new point is added to the vectors of the effected line renderers

        for (int i = 0; i < ExternalLineRenderers.Length; i++)
        {
            OxySatVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(OxySatVector);
        }
    }                                                                                        //analogue for all other value simulations

    public void RespRNormalSim() //Simulating normal Respiratory Rate
    {
        if(CurrentRespR == -1)
        {
            int randomNumber = Random.Range(12, 19);

            CurrentRespR = randomNumber;
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
            }

            CurrentRespRConverted = randomNumber / 3;

            RespRVector.Add(new Vector2(12, CurrentRespRConverted));
        }
        else
        {
            int randomNumber = Random.Range(0, 100);

            if(randomNumber < chanceToChangeRespR) //Respiration Rate is changed 
            {
                int coinFlip = Random.Range(0, 2);

                if(coinFlip == 0)
                {
                    if(CurrentRespR < 18)
                    {
                        CurrentRespR += 1;
                        CurrentRespRConverted += 0.3333f;
                    }                   
                }
                else
                {
                    if (CurrentRespR > 12)
                    {
                        CurrentRespR -= 1;
                        CurrentRespRConverted -= 0.3333f;
                    }
                }
                largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
                for (int i = 0; i < ExternalLargeNums.Length; i++)
                {
                    ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
                }
            }

            RespRVector.Add(new Vector2(12, CurrentRespRConverted));
        }

        RespRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(RespRVector);

        for (int i = 0; i < ExternalLineRenderers.Length; i++)
        {
            RespRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(RespRVector);
        }
    }

    public void RespRCriticalSim( int mode) //Simulating a critical Respiratory Rate in different Scenarios, mode defines the degree
    {
        if (CurrentRespR == -1)
        {
            int randomNumber = 0;
            switch (mode)
            {
                case 1:
                    randomNumber = Random.Range(19, 26);
                    break;
                case 2:
                    randomNumber = Random.Range(16, 23);
                    break;
                case 3:
                    randomNumber = Random.Range(22, 27);
                    break;
            }

            CurrentRespR = randomNumber;
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
            }

            CurrentRespRConverted = randomNumber / 3;

            RespRVector.Add(new Vector2(12, CurrentRespRConverted));
        }
        else
        {
            int randomNumber = Random.Range(0, 100);

            if (randomNumber < chanceToChangeRespR) //Respiration Rate is changed 
            {
                int coinFlip = Random.Range(0, 2);

                if (coinFlip == 0)
                {
                    switch (mode)
                    {
                        case 1:
                            if (CurrentRespR < 25)
                            {
                                CurrentRespR += 1;
                                CurrentRespRConverted += 0.3333f;
                            }
                            break;
                        case 2:
                            if (CurrentRespR < 22)
                            {
                                CurrentRespR += 1;
                                CurrentRespRConverted += 0.3333f;
                            }
                            break;
                        case 3:
                            if (CurrentRespR < 26)
                            {
                                CurrentRespR += 1;
                                CurrentRespRConverted += 0.3333f;
                            }
                            break;
                    }
                    
                }
                else
                {
                    switch (mode)
                    {
                        case 1:
                            if (CurrentRespR > 19)
                            {
                                CurrentRespR -= 1;
                                CurrentRespRConverted -= 0.3333f;
                            }
                            break;
                        case 2:
                            if (CurrentRespR > 16)
                            {
                                CurrentRespR -= 1;
                                CurrentRespRConverted -= 0.3333f;
                            }
                            break;
                        case 3:
                            if (CurrentRespR > 22)
                            {
                                CurrentRespR -= 1;
                                CurrentRespRConverted -= 0.3333f;
                            }
                            break;
                    }
                    
                }
                largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
                for (int i = 0; i < ExternalLargeNums.Length; i++)
                {
                    ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentRespR).ToString();
                }
            }

            RespRVector.Add(new Vector2(12, CurrentRespRConverted));
        }

        RespRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(RespRVector);

        for (int i = 0; i < ExternalLineRenderers.Length; i++)
        {
            RespRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(RespRVector);
        }
    }

    public void HeartRNormalSim()
    {
        
        if(CurrentHeartR == -1)
        {
            int randomNumber = Random.Range(1, 6);      //randomize a heart rate from the selection
            switch (randomNumber)
            {
                case 1:
                    CurrentHeartR = 60;
                    break;
                case 2:
                    CurrentHeartR = 67;
                    break;
                case 3:
                    CurrentHeartR = 75;
                    break;
                case 4:
                    CurrentHeartR = 86;
                    break;
                case 5:
                    CurrentHeartR = 100;
                    break;
            }
            step++;
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentHeartR).ToString();
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentHeartR).ToString();
            }

            HeartRVector.Add(new Vector2(24, 3));

            HeartRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(HeartRVector);

            for (int i = 0; i < ExternalLineRenderers.Length; i++)
            {
                HeartRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(HeartRVector);
            }

            textObject.GetComponent<TextMeshProUGUI>().text = ("\n"+CurrentHeartR).ToString();

            for (int i = 0; i < ExternalTextObjects.Length; i++)
            {
                ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentHeartR).ToString();
            }
        }
        else
        {
            switch (CurrentHeartR)                                 //what heart rate is selected?
            {
                case 60:
                    switch (step)                                  //in which step of the drawing of a heart rate curve are we?
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                            HeartRVector.Add(new Vector2(24, 3));
                            break;
                        case 7:
                        case 9:
                            HeartRVector.Add(new Vector2(24, 3.5f));
                            break;
                        case 8:
                            HeartRVector.Add(new Vector2(24, 2.7f));
                            break;
                        case 10:
                            HeartRVector.Add(new Vector2(24, 5));
                            HeartMonitor.GetComponent<AudioSource>().Play();        //play the sound of the device
                            break;
                        case 11:
                            HeartRVector.Add(new Vector2(24, 2.5f));
                            break;
                        case 12:
                            HeartRVector.Add(new Vector2(24, 3.8f));
                            break;
                        case 20:
                            HeartRVector.Add(new Vector2(24, 3));
                            step = -1;
                            break;
                    }
                    break;

                case 67:
                    switch (step)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                            HeartRVector.Add(new Vector2(24, 3));
                            break;
                        case 5:
                        case 7:
                            HeartRVector.Add(new Vector2(24, 3.5f));
                            break;
                        case 6:
                            HeartRVector.Add(new Vector2(24, 2.7f));
                            break;
                        case 8:
                            HeartRVector.Add(new Vector2(24, 5));
                            HeartMonitor.GetComponent<AudioSource>().Play();
                            break;
                        case 9:
                            HeartRVector.Add(new Vector2(24, 2.5f));
                            break;
                        case 10:
                            HeartRVector.Add(new Vector2(24, 3.8f));
                            break;
                        case 18:
                            HeartRVector.Add(new Vector2(24, 3));
                            step = -1;
                            break;
                    }
                    break;
                case 75:
                    switch (step)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                            HeartRVector.Add(new Vector2(24, 3));
                            break;
                        case 5:
                        case 7:
                            HeartRVector.Add(new Vector2(24, 3.5f));
                            break;
                        case 6:
                            HeartRVector.Add(new Vector2(24, 2.7f));
                            break;
                        case 8:
                            HeartRVector.Add(new Vector2(24, 5));
                            HeartMonitor.GetComponent<AudioSource>().Play();
                            break;
                        case 9:
                            HeartRVector.Add(new Vector2(24, 2.5f));
                            break;
                        case 10:
                            HeartRVector.Add(new Vector2(24, 3.8f));
                            break;
                        case 16:
                            HeartRVector.Add(new Vector2(24, 3));
                            step = -1;
                            break;
                    }
                    break;
                case 86:
                    switch (step)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                            HeartRVector.Add(new Vector2(24, 3));
                            break;
                        case 3:
                        case 5:
                            HeartRVector.Add(new Vector2(24, 3.5f));
                            break;
                        case 4:
                            HeartRVector.Add(new Vector2(24, 2.7f));
                            break;
                        case 6:
                            HeartRVector.Add(new Vector2(24, 5));
                            HeartMonitor.GetComponent<AudioSource>().Play();
                            break;
                        case 7:
                            HeartRVector.Add(new Vector2(24, 2.5f));
                            break;
                        case 8:
                            HeartRVector.Add(new Vector2(24, 3.8f));
                            break;
                        case 14:
                            HeartRVector.Add(new Vector2(24, 3));
                            step = -1;
                            break;
                    }
                    break;
                case 100:
                    switch (step)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 9:
                        case 10:
                        case 11:
                            HeartRVector.Add(new Vector2(24, 3));
                            break;
                        case 3:
                        case 5:
                            HeartRVector.Add(new Vector2(24, 3.5f));
                            break;
                        case 4:
                            HeartRVector.Add(new Vector2(24, 2.7f));
                            break;
                        case 6:
                            HeartRVector.Add(new Vector2(24, 5));
                            HeartMonitor.GetComponent<AudioSource>().Play();
                            break;
                        case 7:
                            HeartRVector.Add(new Vector2(24, 2.5f));
                            break;
                        case 8:
                            HeartRVector.Add(new Vector2(24, 3.8f));
                            break;
                        case 12:
                            HeartRVector.Add(new Vector2(24, 3));
                            step = -1;
                            break;
                    }
                    break;
            }
            step++;     //progress the drawing of the heart rate graph by 1
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentHeartR).ToString();
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentHeartR).ToString();
            }
            HeartRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(HeartRVector);

            for (int i = 0; i < ExternalLineRenderers.Length; i++)
            {
                HeartRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(HeartRVector);
            }
            textObject.GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentHeartR).ToString();

            for (int i = 0; i < ExternalTextObjects.Length; i++)
            {
                ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentHeartR).ToString();
            }
        }
    }

    IEnumerator RefreshRate() //Refresh every graph / display according to custom refresh rate
    {
        if (OxySatTurnedOff == false)       //is this vital sign measurment turned on?
        {
            
            if (OxySatNormal == true)       //should the vital sign be normal?
            {
                OxySatNormalSim();
            }
            if (OxySatCritical > 0)         //should the vital sign be abnormal and if yes, which issue is in place?
            {
                OxySatCriticalSim(OxySatCritical);
            }
        }
        if (RespRTurnedOff == false)
        {
            if (RespRNormal == true)
            {
                RespRNormalSim();
            }
            if (RespRCritical > 0)
            {
                RespRCriticalSim(RespRCritical);
            }
        }
        if (BloodPrTurnedOff == false)
        {
            if (BloodPrNormal == true)
            {
                bloodPrSim();
            }
            if (BloodPrCritical > 0)
            {
                bloodPrCriticalSim(BloodPrCritical);
            }
        }
        if(HeartRTurnedOff == false)
        { 
            if (HeartRNormal == true)
            {
                HeartRNormalSim();
            }
            if (HeartRCritical > 0)
            {
                HeartRCriticalSim(HeartRCritical);
            }
        }
        
        
            if (HeartRTurnedOff == true)            //the vital sign measurement is turned off
            {
                HeartROff();
            }
            if (RespRTurnedOff == true)
            {
                RespROff();
            }
            if (OxySatTurnedOff == true)
            {
                OxySatOff();
            }
            if (BloodPrTurnedOff == true)
            {
                BloodPrOff();
            }
        

        yield return new WaitForSeconds(refreshRate); 
        
        refreshNeccessary = true;
    }


    public void bloodPrSim() //Simulating normal blood pressure
    {
        if (CurrentBloodPr1 == -1 || CurrentBloodPr2 == -1)
        {
            CurrentBloodPr1 = Random.Range(90, 120);
            CurrentBloodPr2 = Random.Range(60, 80);
            

        }
        else
        {
            int randomNumber = Random.Range(0, 100);

            if(randomNumber < chanceToChangeBloodPr) //First Blood Pressure Value changes
            {
                randomNumber = Random.Range(1, 6);
                int coinFlip = Random.Range(0, 2);

                if (coinFlip == 0)
                {
                    CurrentBloodPr1 += randomNumber;
                    if(CurrentBloodPr1 > 119)
                    {
                        CurrentBloodPr1 = 119;
                    }
                }
                else
                {
                    CurrentBloodPr1 -= randomNumber;
                    if (CurrentBloodPr1 < 90)
                    {
                        CurrentBloodPr1 = 90;
                    }
                }
            }

            randomNumber = Random.Range(0, 100);

            if (randomNumber < chanceToChangeBloodPr) //Second Blood Pressure Value changes
            {
                randomNumber = Random.Range(1, 4);
                int coinFlip = Random.Range(0, 2);

                if (coinFlip == 0)
                {
                    CurrentBloodPr2 += randomNumber;
                    if (CurrentBloodPr2 > 79)
                    {
                        CurrentBloodPr2 = 79;
                    }
                }
                else
                {
                    CurrentBloodPr2 -= randomNumber;
                    if (CurrentBloodPr2 < 60)
                    {
                        CurrentBloodPr2 = 60;
                    }
                }
            }
            
        }

        textObject.GetComponent<TextMeshProUGUI>().text = (CurrentBloodPr1 + " \n " + CurrentBloodPr2).ToString();

        for(int i = 0; i<ExternalTextObjects.Length; i++)
        {
            ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = ("\n"+ CurrentBloodPr1 + " \n " + CurrentBloodPr2).ToString();
        }
        for (int i = 0; i < ExternalLargeNums.Length; i++)
        {
            ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentBloodPr1 + " \n " + CurrentBloodPr2).ToString();
        }
    }

    public void bloodPrCriticalSim(int mode) //Simulating critical blood pressure
    {
        if (CurrentBloodPr1 == -1 || CurrentBloodPr2 == -1)
        {
            
            switch (mode)
            {
                case 1:
                    CurrentBloodPr1 = Random.Range(120, 140);   //too high
                    CurrentBloodPr2 = Random.Range(80, 90);
                    break;
                case 2:
                    CurrentBloodPr1 = Random.Range(80, 100); //slightly lower
                    CurrentBloodPr2 = Random.Range(55, 70);
                   
                    break;
            }
            

        }
        else
        {
            int randomNumber = Random.Range(0, 100);

            if (randomNumber < chanceToChangeBloodPr) //First Blood Pressure Value changes
            {
                randomNumber = Random.Range(1, 6);
                int coinFlip = Random.Range(0, 2);

                if (coinFlip == 0)
                {
                    CurrentBloodPr1 += randomNumber;
                    
                    switch (mode)
                    {
                        case 1:
                            if (CurrentBloodPr1 > 139)
                            {
                                CurrentBloodPr1 = 139;
                            }
                            break;
                        case 2:
                            if (CurrentBloodPr1 > 99)
                            {
                                CurrentBloodPr1 = 99;
                            }
                            break;
                    }
                }
                else
                {
                    CurrentBloodPr1 -= randomNumber;
                    switch (mode)
                    {
                        case 1:
                            if (CurrentBloodPr1 < 120)
                            {
                                CurrentBloodPr1 = 120;
                            }
                            break;
                        case 2:
                            if (CurrentBloodPr1 < 80)
                            {
                                CurrentBloodPr1 = 80;
                            }
                            break;
                    }
                    
                }

            }

            randomNumber = Random.Range(0, 100);

            if (randomNumber < chanceToChangeBloodPr) //Second Blood Pressure Value changes
            {
                randomNumber = Random.Range(1, 4);
                int coinFlip = Random.Range(0, 2);

                if (coinFlip == 0)
                {
                    CurrentBloodPr2 += randomNumber;
                    switch (mode)
                    {
                        case 1:
                            if (CurrentBloodPr2 > 90)
                            {
                                CurrentBloodPr2 = 90;
                            }
                            break;
                        case 2:
                            if (CurrentBloodPr2 > 70)
                            {
                                CurrentBloodPr2 = 70;
                            }
                            break;
                    }
                }
                else
                {
                    CurrentBloodPr2 -= randomNumber;
                    switch (mode)
                    {
                        case 1:
                            if (CurrentBloodPr2 < 80)
                            {
                                CurrentBloodPr2 = 80;
                            }
                            break;
                        case 2:
                            if (CurrentBloodPr2 < 55)
                            {
                                CurrentBloodPr2 = 55;
                            }
                            break;
                    }
                }
            }
            
        }
       
        textObject.GetComponent<TextMeshProUGUI>().text = (CurrentBloodPr1 + " \n " + CurrentBloodPr2).ToString();

        for (int i = 0; i < ExternalTextObjects.Length; i++)
        {
            ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentBloodPr1 + " \n " + CurrentBloodPr2).ToString();
        }
        for (int i = 0; i < ExternalLargeNums.Length; i++)
        {
            ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentBloodPr1 + " \n " + CurrentBloodPr2).ToString();
        }
    }

    public void HeartRCriticalSim(int mode)
    {
        if (CurrentHeartR == -1)
        {
            CurrentHeartR = 120;
            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentHeartR).ToString();

            step++;

            HeartRVector.Add(new Vector2(24, 3));

            HeartRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(HeartRVector);

            for (int i = 0; i < ExternalLineRenderers.Length; i++)
            {
                HeartRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(HeartRVector);
            }

            textObject.GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentHeartR).ToString();

            for (int i = 0; i < ExternalTextObjects.Length; i++)
            {
                ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentHeartR).ToString();
            }
        }
        else
        {
            if (mode == 2 && step == 0)
            {
                int randomNumber = Random.Range(0, 100);
                if (randomNumber < 30)
                {
                    step = 1;
                }
                if (randomNumber > 80)
                {
                    step = 2;
                }
            }
            if (mode == 2 && step == 10)
            {
                int randomNumber = Random.Range(0, 100);
                if(randomNumber < 30)
                {
                    step = 0;
                }
                if (randomNumber > 80)
                {
                    step = 1;
                }
            }
                   switch (step)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 9:
                            HeartRVector.Add(new Vector2(24, 3));
                            break;
                        case 3:
                        case 5:
                            HeartRVector.Add(new Vector2(24, 3.5f));
                            break;
                        case 4:
                            HeartRVector.Add(new Vector2(24, 2.7f));
                            break;
                        case 6:
                            HeartRVector.Add(new Vector2(24, 5));
                            HeartMonitor.GetComponent<AudioSource>().Play();
                            break;
                        case 7:
                            HeartRVector.Add(new Vector2(24, 2.5f));
                            break;
                        case 8:
                            HeartRVector.Add(new Vector2(24, 3.8f));
                            break;
                        case 10:
                            HeartRVector.Add(new Vector2(24, 3));
                            step = -1;
                            break;
                    }

            largeNum.GetComponent<TextMeshProUGUI>().text = (CurrentHeartR).ToString();
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (CurrentHeartR).ToString();
            }
            step++;
            HeartRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(HeartRVector);

            for (int i = 0; i < ExternalLineRenderers.Length; i++)
            {
                HeartRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(HeartRVector);
            }
            textObject.GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentHeartR).ToString();

            for (int i = 0; i < ExternalTextObjects.Length; i++)
            {
                ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = ("\n" + CurrentHeartR).ToString();
            }
        }
    }

    public void addOneToRange(int m)        //increase a boundary
    {
        if (justPressed == true)            //to not double press buttons
        {
            return;
        }
        else
        {
            justPressed = true;
        }
        if (OxySatNormal == true || OxySatCritical > 0)
        {
            oxySatLower++;                  //increased by 1
            if(oxySatLower > CurrentOxySat) //now outside of range
            {
                SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(0);
            }
        }
        if (RespRNormal == true || RespRCritical > 0)
        {
            if (m == 1)
            {
                respRLower++;
                if(respRLower > CurrentRespR) //now outside of range
                {
                    SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(1);
                }
            }
            if (m == 2)
            {
                respRUpper++;
            }
        }
        if (BloodPrNormal == true || BloodPrCritical > 0)
        {
            switch (m)
            {
                case 1:
                    bloodPr1Lower++;
                    if (bloodPr1Lower > CurrentBloodPr1) //now outside of range
                    {
                        SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(2);
                    }
                    break;
                case 2:
                    bloodPr1Upper++;
                    break;
                case 3:
                    bloodPr2Lower++;
                    if (bloodPr2Lower > CurrentBloodPr2) //now outside of range
                    {
                        SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(2);
                    }
                    break;
                case 4:
                    bloodPr2Upper++;
                    break;
            }
        }
        if (HeartRNormal == true || HeartRCritical > 0)
        {
            if (m == 1)
            {
                heartRLower++;
                if (heartRLower > CurrentHeartR) //now outside of range
                {
                    SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(3);
                }
            }
            if (m == 2)
            {
                heartRUpper++;
            }
        }
        StartCoroutine(ButtonDelay());          //prevent double press of buttons
    }                                           //analogue for decreasing of the boundary

    public void subtractOneFromRange(int m)     //decrease boundary
    {
        if(justPressed == true)
        {
            return;
        }
        else
        {
            justPressed = true;
        }
        if (OxySatNormal == true || OxySatCritical > 0)
        {
            oxySatLower--;
        }
        if (RespRNormal == true || RespRCritical > 0)
        {
            if (m == 1)
            {
                respRLower--;
            }
            if (m == 2)
            {
                respRUpper--;
                if (respRUpper < CurrentRespR) //now outside of range
                {
                    SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(1);
                }
            }
        }
        if (BloodPrNormal == true || BloodPrCritical > 0)
        {
            switch (m)
            {
                case 1:
                    bloodPr1Lower--;
                    break;
                case 2:
                    bloodPr1Upper--;
                    if (bloodPr1Upper < CurrentBloodPr1) //now outside of range
                    {
                        SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(2);
                    }
                    break;
                case 3:
                    bloodPr2Lower--;
                    break;
                case 4:
                    bloodPr2Upper--;
                    if (bloodPr2Upper < CurrentBloodPr2) //now outside of range
                    {
                        SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(2);
                    }
                    break;
            }
        }
        if (HeartRNormal == true || HeartRCritical > 0)
        {
            if (m == 1)
            {
                heartRLower--;
            }
            if (m == 2)
            {
                heartRUpper--;
                if (heartRUpper < CurrentHeartR) //now outside of range
                {
                    SimulationController.gameObject.GetComponent<SimulationSetup>().newGlobalAlarm(3);
                }
            }
        }
        StartCoroutine(ButtonDelay());
    }

    public void HeartROff()     //Heart Rate is currently not measured
    {
        HeartRVector.Add(new Vector2(24, 0));

        HeartRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(HeartRVector);           //all new added points are y=0
        for (int i = 0; i < ExternalLineRenderers.Length; i++)
        {
            HeartRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(HeartRVector);
        }

        textObject.GetComponent<TextMeshProUGUI>().text = ("\n" + "Aus");                               //replace all mentioning of the Pulse with "Aus"
        for (int i = 0; i < ExternalTextObjects.Length; i++)
        {
            ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = ("\n" + "Aus");
        }
        largeNum.GetComponent<TextMeshProUGUI>().text = ("Aus");                                        //replace all mentioning of the Heart Rate with "Aus"
        for (int i = 0; i < ExternalLargeNums.Length; i++)
        {
            ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = ("Aus");
        }
    }                                                                                                   //analogue for the other vital signs

    public void RespROff()      //Respiration Rate is currently not measured
    {
        RespRVector.Add(new Vector2(12, 0));

        RespRVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(RespRVector);

        for (int i = 0; i < ExternalLineRenderers.Length; i++)
        {
            RespRVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(RespRVector);
        }
        largeNum.GetComponent<TextMeshProUGUI>().text = ("Aus");
        for (int i = 0; i < ExternalLargeNums.Length; i++)
        {
            ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = ("Aus");
        }
    }

    public void OxySatOff()         //Oxygen Saturation is currently not measured
    {
        OxySatVector.Add(new Vector2(12, 0));

        OxySatVector = LineRenderer.GetComponent<UILineRenderer>().addNewPoint(OxySatVector);

        for (int i = 0; i<ExternalLineRenderers.Length; i++)
        {
            OxySatVector = ExternalLineRenderers[i].GetComponent<UILineRenderer>().addNewPoint(OxySatVector);
        }
        if (OxySatUnknown == false)
        {
            
            largeNum.GetComponent<TextMeshProUGUI>().text = ("Aus");
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = ("Aus");
            }
        }
        else //Scenario 4: Measurement Device fell off
        {
            
            largeNum.GetComponent<TextMeshProUGUI>().text = (" ?");
            for (int i = 0; i < ExternalLargeNums.Length; i++)
            {
                ExternalLargeNums[i].GetComponent<TextMeshProUGUI>().text = (" ?");
            }
            
            
        }

    }

    public void BloodPrOff()        //Blood Pressure is currently not measured
    {
        if (BloodPrUnknown == false)
        {
            textObject.GetComponent<TextMeshProUGUI>().text = "Aus";

            for (int i = 0; i < ExternalTextObjects.Length; i++)
            {
                ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = "\n" + "Aus";
            }
        }
        else
        {
            textObject.GetComponent<TextMeshProUGUI>().text = " ? ";
            for (int i = 0; i < ExternalTextObjects.Length; i++)
            {
                ExternalTextObjects[i].GetComponent<TextMeshProUGUI>().text = (" ?");
            }
        }
    }

    public void turnOffMeasurement(int sign)            //measurement of value deactivated on screen
    {
        switch (sign)
        {
            case 1:
                HeartRTurnedOff = true;
                break;
            case 2:
                OxySatTurnedOff = true;
                OxySatTurnedOffhard = true;             //removed by a button press specifically
                //StationaryDevice.gameObject.GetComponent<MoveDeviceAnim>().handle.SetActive(true); //Animation of Device not deemed necessary anymore
                FingertipDeviceWindow.gameObject.SetActive(true);
                break;
            case 3:
                RespRTurnedOff = true;
                break;
            case 4:
                BloodPrTurnedOff = true;
                BloodPrTurnedOffhard = true;
                CuffWindow.gameObject.SetActive(true);
                break;
            case 5: //Scenario 4 where Fingertip device fell off
                OxySatTurnedOff = true;
                OxySatUnknown = true;
                break;

        }
    }
    public void turnOnMeasurement(int sign)         //measurement of value activated on screen
    {
        switch (sign)
        {
            case 1:
                HeartRTurnedOff = false;
                break;
            case 2:
                OxySatTurnedOff = false;
                OxySatTurnedOffhard = false;
                OxySatUnknown = false;
                //StationaryDevice.gameObject.GetComponent<MoveDeviceAnim>().handle.SetActive(false); //Animation of Device not deemed necessary anymore
                break;
            case 3:
                RespRTurnedOff = false;
                break;
            case 4:
                BloodPrTurnedOff = false;
                BloodPrTurnedOffhard = false;
                BloodPrUnknown = false;
                break;
            case 5: //OxySat but only device added
                OxySatUnknown = false;
                break;
            case 6: //BloodPr but only device added
                BloodPrUnknown = false;
                break;
        }
    }

    public void deviceRemoved(int sign)         
    {
        switch (sign)                                   //which device is removed 
        {
            case 1:
                
                break;
            case 2:
                OxySatTurnedOff = true;
                if(OxySatTurnedOffhard == false)
                {
                    OxySatUnknown = true;
                }
                break;
            case 3:
                
                break;
            case 4:
                BloodPrTurnedOff = true;
                if (BloodPrTurnedOffhard == false)
                {
                    BloodPrUnknown = true;
                }
                break;
            
        }
    }

    public bool confirmFlicker(int sign)            //check if flicker is correct
    {
        switch (sign)
        {
            case 0:
                if(CurrentOxySat < oxySatLower)
                {
                    return true;
                }
                break;
            case 1:
                if(CurrentRespR < respRLower || CurrentRespR > respRUpper)
                {
                    return true;
                }
                break;
            case 2:
                if(CurrentBloodPr1 < bloodPr1Lower || CurrentBloodPr1 > bloodPr1Upper || CurrentBloodPr2 < bloodPr2Lower || CurrentBloodPr2 > bloodPr2Upper)
                {
                    return true;
                }
                break;
            case 3:
                if(CurrentHeartR < heartRLower || CurrentHeartR > heartRUpper)
                {
                    return true;
                }
                break;
        }
       
        return false;
    }

    IEnumerator ButtonDelay() //buttons otherwise do multiple actions with one press
    {
        yield return new WaitForSeconds(0.3f);
        justPressed = false;
    }
}
