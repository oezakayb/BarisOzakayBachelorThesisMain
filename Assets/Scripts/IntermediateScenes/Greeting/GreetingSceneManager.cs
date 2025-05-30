using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.AI;
using Utilities.Extensions;

public class GreetingSceneManager : MonoBehaviour
{
    [Header("UI Groups")]
    public CanvasGroup detectionUI;

    [Header("Subtitles & Buttons")]
    public TextMeshProUGUI subtitleText;
    public Button continueButton;
    public Button goToNurseButton;

    [Header("Audio Clips")]
    public AudioClip[] greetingClips;  
    public string[] germanSentences; 
    public AudioClip lookClip;      
    public AudioClip wellDoneClip;    
    public AudioClip nurseHelpClip;  

    [Header("Face Detection")]
    public AR.FaceDetectorScene faceDetector;

    [Header("Camera Tween")]
    public Transform cameraTransform;
    public float tweenDuration = 1f;

    private AudioSource audioSource;
    private int sentenceIndex = 0;

    public Transform robotTransform;
    public Transform nurseRoomEntrance;
    public Transform nurseRoomExit;
    public DoorAnim door;
    
    [Header("— Pre-Pill Sorting Talk —")]
    public AudioClip[] prePillClips;         
    public string[] prePillSentences;     
    public Button goToPillSortingButton;
    public Transform pillSortingStation;   

    void Awake()
    {
        DontDestroyOnLoad(robotTransform.gameObject);
        detectionUI.alpha = 0;
        detectionUI.interactable = false;
        detectionUI.blocksRaycasts = false;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        continueButton.gameObject.SetActive(false);
        goToNurseButton.gameObject.SetActive(false);

        PlayGreeting();
    }

    void PlayGreeting()
    {
        sentenceIndex = 0;
        PlayNextGreetingLine();
    }

    void PlayNextGreetingLine()
    {
        subtitleText.text = germanSentences[sentenceIndex];
        audioSource.clip = greetingClips[sentenceIndex];
        audioSource.Play();
        StartCoroutine(WaitForGreetingLine());
    }

    IEnumerator WaitForGreetingLine()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        sentenceIndex++;
        if (sentenceIndex < germanSentences.Length)
        {
            PlayNextGreetingLine();
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    void OnContinueClicked()
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.gameObject.SetActive(false);
        StartDetectionPhase();
    }

    void StartDetectionPhase()
    {
        detectionUI.DOFade(1, 0.5f).OnComplete(() =>
        {
            cameraTransform.position = new Vector3(-7.29f, 1.44f, 5.7f);
            cameraTransform.rotation = Quaternion.Euler(10, 180, 0);
            detectionUI.interactable = true;
            detectionUI.blocksRaycasts = true;
            
            subtitleText.text = "Schau jetzt direkt in die Kamera, ohne deinen Kopf zu neigen.";
            audioSource.clip = lookClip;
            audioSource.Play();
            
            StartCoroutine(PollForAlignment());
        });
    }

    IEnumerator PollForAlignment()
    {
        yield return new WaitForSeconds(lookClip.length);
        
        while (!faceDetector.faceAligned)
            yield return null;

        OnFaceAligned();
    }

    void OnFaceAligned()
    {
        detectionUI.DOFade(0, 0.5f).OnComplete(() => {
            detectionUI.blocksRaycasts = false;
        });
        
        subtitleText.text = "Gut gemacht! Du bist jetzt bereit für das Training.";
        audioSource.clip = wellDoneClip;
        audioSource.Play();

        StartCoroutine(AfterWellDone());
    }

    IEnumerator AfterWellDone()
    {
        yield return new WaitForSeconds(wellDoneClip.length);
        
        cameraTransform.DOMove(new Vector3(-8.8f, 1.7f, 5.8f), tweenDuration);
        cameraTransform.DORotate(new Vector3(10, 90, 0), tweenDuration);
        

        yield return new WaitForSeconds(tweenDuration);
        
        subtitleText.text = "Eine andere Pflegekraft benötigt deine Hilfe im Pflegerraum. Bitte gehe dorthin.";
        audioSource.clip = nurseHelpClip;
        audioSource.Play();

        yield return new WaitForSeconds(nurseHelpClip.length);

        goToNurseButton.gameObject.SetActive(true);
        goToNurseButton.onClick.AddListener(() => {
            goToNurseButton.onClick.RemoveAllListeners();
            goToNurseButton.gameObject.SetActive(false);
            robotTransform.DORotate(new Vector3(0, 180, 0), 2)
                .OnComplete(() =>
                {
                    StartCoroutine(GoToNurseRoom());
                });
            
        });
    }

    IEnumerator GoToNurseRoom()
    {

        cameraTransform.DOMove(new Vector3(-7.43f, 2.27f, 6.215f), 1)
            .OnComplete(() =>
            {
                cameraTransform.SetParent(robotTransform);
                cameraTransform.DOLocalRotate(new Vector3(30, 0, 0), 1);
            });
        
        yield return new WaitForSeconds(1);
        
        var nav = robotTransform.GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            nav.destination = nurseRoomEntrance.position + new Vector3(0, robotTransform.position.y, 0);
            yield return new WaitForSeconds(1f);
            door.setInactiveAndOpen();
            yield return new WaitForSeconds(1.2f);
            robotTransform.DORotate(new Vector3(0f, -90f, 0f), 2)
                .OnComplete(() =>
                {
                    nav.destination = nurseRoomExit.position + new Vector3(0, robotTransform.position.y, 0);
                    StartCoroutine(PrePillSortingSetUp());
                });
        }
        else
        {
            Debug.LogError("NavMeshAgent missing on Robody!");
        }
    }

    IEnumerator PrePillSortingSetUp()
    {
        yield return new WaitForSeconds(1.5f);
        cameraTransform.SetParent(gameObject.transform);
        cameraTransform.SetParent(null);
        
        cameraTransform.DOMove(new Vector3(-12.378f, 1.53f, -0.388f), 1)
            .OnComplete(() =>
            {
                cameraTransform.DORotate(new Vector3(20, 0, 0), 1);
                robotTransform.DORotate(new Vector3(0, -127, 0), 1f);
            });
        
        
        yield return new WaitForSeconds(2f);
        StartCoroutine(PrePillSortingTalk());
    }

    IEnumerator PrePillSortingTalk()
    {
        for (int i = 0; i < prePillClips.Length; i++)
        {
            subtitleText.text = prePillSentences[i];
            audioSource.clip = prePillClips[i];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        goToPillSortingButton.gameObject.SetActive(true);
        goToPillSortingButton.onClick.AddListener(OnGoToPillSortingClicked);
    }

    void OnGoToPillSortingClicked()
    {
        goToPillSortingButton.onClick.RemoveAllListeners();
        goToPillSortingButton.gameObject.SetActive(false);
        StartCoroutine(GoToPillSortingStation());
    }

    IEnumerator GoToPillSortingStation()
    {
        Vector3 camPos = cameraTransform.position;
        Vector3 targetPos = new Vector3(camPos.x, camPos.y, -1.06f);
        cameraTransform.DOMove(targetPos, 1)
            .OnComplete(() =>
            {
                float curX = cameraTransform.rotation.eulerAngles.x;
                float curZ = cameraTransform.rotation.eulerAngles.z;
                Vector3 targetRot = new Vector3(curX, 50, curZ);
                cameraTransform.DORotate(targetRot, 1)
                    .OnComplete(() =>
                    {
                        robotTransform.DORotate(new Vector3(0f, 90f, 0f), 2)
                            .OnComplete(() =>
                            {
                                var nav = robotTransform.GetComponent<NavMeshAgent>();
                                nav.destination = pillSortingStation.position + new Vector3(0, robotTransform.position.y, 0);
                            });
                        
                    });
            });
        
        yield return new WaitForSeconds(5.5f);
        robotTransform.gameObject.SetActive(false);
        GameFlowManager.Instance.LoadNextScene();
    }
}
