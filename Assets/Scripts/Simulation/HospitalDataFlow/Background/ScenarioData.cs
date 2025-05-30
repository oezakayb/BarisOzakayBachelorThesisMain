using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioData : MonoBehaviour
{
    //Changed to only shown in inspector!
    //The headers for the scenario's guide menu are listed here. Each scenario has its own line. Up to ten steps per scenario. 

    public string[] scenarioHeader = new string[50]; /*{ "Sie hören den Alarm auf dem Flur." , "Auf Uhr und Monitor kann der Patient identifiziert werden.", "Betreten Sie den Raum links und sehen Sie nach der Patientin.", "Überprüfen Sie die Vitalparameter der Patientin.", "Fertig mit dem Szenario?", "","", "", "", "",
                                       "Sie hören wieder den Alarm auf dem Flur.",
    };*/

    //The descriptions for the scenario's guide menu are listed here. Each scenario has its own line. Up to ten steps per scenario. 

    public string[] scenarioDescription = new string[50];/*{ "Folgen Sie ihren gewöhnlichen Arbeitsschritten zum Ursprung des Alarms und deaktivieren Sie ihn." , "Finden Sie den Ursprung des Alarms auf der Uhr oder dem Monitor.", "Einige Geräte dabei sind interaktiv und können benutzt werden.", "Durch eine Interaktion mit der Patientin schließen Sie das Szenario ab.", "Klicken Sie auf dieses Fenster um zum Hauptmenü zurückzukehren.", "","", "", "", "",
                            "Auf der Station liegt eine Covid-19 Patientin in einem schlechten Zustand."
    };*/

    //The positions for the scenario's guide menu are listed here. Each scenario has its own line. Up to 3 * 10 steps per scenario. For a single floor, y is always 0.
    public float[] scenarioPositions = new float[150];/*{ -11, 1.4, 0, -7, 1.4, 2.5, -7, 1.4, -7.6, 0.4, 1.4, -4.7, 1.5, 1.4, -2.9, 0,0,0, 0,0,0, 0,0,0, 0,0,0, 0,0,0

    };*/

    //The Y-rotations for the scenario's guide menu are listed here. Each scenario has its own line. Up to 10 steps per scenario. Only Y-rotations are used.

    public float[] scenarioRotations = new float[50];/*{ 0, -90, 90, 0, 0, 0,0,0,0,0

    };*/
}
