using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARController : MonoBehaviour
{
    public GameObject robody;
    public AudioClip welcomeClip;
    public TTS tts;

    public CanvasGroup buttonsUI;
    
    private ARPlaneManager planeManager;
    private bool robodyPlaced = false;
    public AudioSource lipSyncAudioSource;

    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        buttonsUI.alpha = 0;
        buttonsUI.interactable = false;
    }

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (robodyPlaced)
            return;
        
        List<ARPlane> horizontalPlanes = new List<ARPlane>();
        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
                horizontalPlanes.Add(plane);
        }

        if (horizontalPlanes.Count > 0)
        {
            horizontalPlanes.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
            ARPlane lowestPlane = horizontalPlanes[0];
            
            Vector3 placementPosition = lowestPlane.transform.position;
            placementPosition.y += 0.05f;

            robody.SetActive(true);
            robody.transform.position = placementPosition;
            robodyPlaced = true;
            StartCoroutine(Animate());
        }
    }

    IEnumerator Animate()
    {
        yield return new WaitForSeconds(1.5f);
        robody.GetComponent<Animator>().Play("Waving");
        tts.GetComponent<AudioSource>().PlayOneShot(welcomeClip);
        lipSyncAudioSource.PlayOneShot(welcomeClip);
        /*Hallo und herzlich willkommen!
        Ich freue mich sehr, dich kennenzulernen.
        Mein Name ist Robody und ich wurde von der Firma Devanthro entwickelt.
        Ich bin dafür gemacht, Pflegekräfte bei alltäglichen Aufgaben zu unterstützen – 
        zum Beispiel beim Bringen von Gegenständen oder bei der nächtlichen Überwachung von Patienten. 
        Ich bin keine künstliche Intelligenz. 
        Hinter mir steht ein echter Mensch, der mich mit einem Virtual-Reality-Headset steuert. 
        Auch autorisierte Personen wie Familienmitglieder können sich nach einer kurzen Einweisung über 
        ein Virtual-Reality-Headset mit mir verbinden und mich fernsteuern. 
        Mein Gesicht ist ein digitaler Bildschirm, auf dem das Gesicht der steuernden Person projiziert wird. 
        Ich kann alles sehen, hören und sprechen – fast genauso wie ein echter Mensch.
        In dieser Simulation wirst du ein Auszubildender an deinem ersten Tag sein. 
        Du wirst die Person sein, die mich steuert, und einige Aufgaben in der Klinik erledigen. Viel Spaß dabei!
        Diese Simulation soll dir zeigen, wer ich bin und was ich kann. 
        Du kannst jetzt noch ein wenig mit mir sprechen oder direkt in die Simulation starten. 
        Drücke auf 'Sprechen', wenn du mir etwas sagen möchtest – ich brauche dann einen Moment, 
        um über eine Antwort nachzudenken und mit dir zu sprechen. 
        Wenn du bereit bist, drücke auf 'Starten', um die Simulation zu beginnen.
        */
        yield return new WaitForSeconds(welcomeClip.length);
        buttonsUI.alpha = 1;
        buttonsUI.interactable = true;

    }
}
