using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PostPillSortingSceneManager : MonoBehaviour
{
    [Header("Robot & Camera")]
    public Transform robotTransform; 
    public Transform cameraTransform;
    public Transform robotHand;
    public Animator robotAnimator;
    
    [Header("Tray & Points")]
    public Transform pillTrayPoint;     
    public Transform cameraChangePoint1;
    public Transform cameraChangePoint2;
    public GameObject pillTray;        
    public Transform  bedsideTablePoint; 
    public Transform nurseOutroPoint;
    public Transform cuvetDropPoint;

    [Header("UI & Subtitles")]
    public TextMeshProUGUI subtitleText;
    public Button bringTrayButton;
    public Button returnToNurseButton;
    public AudioClip buttonClickClip;

    [Header("Nurse Intro")]
    public AudioClip[] nurseClips1;
    public string[] nurseSentences1;

    [Header("Patient Speech")]
    public AudioClip[] patientClips;
    public string[] patientSentences;

    [Header("Nurse Outro")] 
    public Transform nurse;
    public Animator door;
    public AudioClip[] nurseClips2;
    public string[] nurseSentences2;

    [Header("Timings")]
    public float speechGap = 0.1f;
    public float arriveThreshold = 0.1f;
    public float nightDelay = 2f;

    private AudioSource audioSource;
    private NavMeshAgent robotAgent;
    private int idx;

    void Awake()
    {
        if (robotTransform == null)
        {
            var go = RobotHandReference.Instance;
            if (go != null) robotTransform = go.transform;
        }
        robotTransform.gameObject.SetActive(true);
        robotAgent = robotTransform.GetComponent<NavMeshAgent>();
        robotAnimator = robotTransform.GetComponent<Animator>();
        robotHand = robotTransform.GetComponent<RobotHandReference>().hand;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        robotTransform.position = new Vector3(-10f, 0.01f, 0.4f);
        robotTransform.localRotation = Quaternion.Euler(0, -58, 0);

        bringTrayButton.gameObject.SetActive(false);
        returnToNurseButton.gameObject.SetActive(false);

        StartCoroutine(NurseIntro());
    }

    IEnumerator NurseIntro()
    {
        door.Play("OpeningDoor");
        for (idx = 0; idx < nurseClips1.Length; idx++)
        {
            subtitleText.text = nurseSentences1[idx];
            audioSource.clip = nurseClips1[idx];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + speechGap);
        }
        bringTrayButton.gameObject.SetActive(true);
        bringTrayButton.onClick.AddListener(OnBringTrayClicked);
    }

    void OnBringTrayClicked()
    {
        audioSource.PlayOneShot(buttonClickClip);
        bringTrayButton.onClick.RemoveAllListeners();
        bringTrayButton.gameObject.SetActive(false);
        StartCoroutine(HandleTrayPickup());
    }

    IEnumerator HandleTrayPickup()
    {

        robotAgent.destination = cameraChangePoint1.position + new Vector3(0, robotTransform.position.y, 0);
        
        while (robotAgent.pathPending ||
               robotAgent.remainingDistance > arriveThreshold)
            yield return null;

        cameraTransform.position = new Vector3(-7.72f, 1.3f, -7.78f);
        cameraTransform.rotation = Quaternion.Euler(0, 42, 0);
        
        robotAgent.destination = pillTrayPoint.position + new Vector3(0, robotTransform.position.y, 0);

        yield return new WaitForSeconds(0.5f);

        while (robotAgent.pathPending ||
               robotAgent.remainingDistance > arriveThreshold)
            yield return null;

        robotTransform.DORotate(new Vector3(0, 90, 0), 1.5f);
        yield return new WaitForSeconds(1.5f);
        
        robotAnimator.Play("robodyPickUpCuvet");

        yield return new WaitForSeconds(0.7f);
        
        pillTray.transform.SetParent(robotHand);
        
        robotAnimator.Play("robodyPickUpCuvetFinish");
        
        yield return new WaitForSeconds(0.6f);
        
        robotAgent.destination = cameraChangePoint2.position + new Vector3(0, robotTransform.position.y, 0);
        
        while (robotAgent.pathPending ||
               robotAgent.remainingDistance > arriveThreshold)
            yield return null;
        
        cameraTransform.position = new Vector3(2.8f, 1.95f, -3.84f);
        cameraTransform.rotation = Quaternion.Euler(10, -65, 0);
        
        robotAgent.destination = bedsideTablePoint.position + new Vector3(0, robotTransform.position.y, 0);

        
        while (robotAgent.pathPending ||
               robotAgent.remainingDistance > arriveThreshold)
            yield return null;
        
        cameraTransform.position = new Vector3(-1.17f, 1.82f, -2.47f);
        cameraTransform.rotation = Quaternion.Euler(10, 65, 0);

        pillTray.transform.SetParent(null);
        pillTray.transform.position = cuvetDropPoint.position;
        robotAnimator.Play("robodyDropCuvet");
        
        yield return new WaitForSeconds(0.3f);

        robotTransform.DORotate(new Vector3(0, -116, 0), 1.5f);

        yield return new WaitForSeconds(1.75f);
        
        for (idx = 0; idx < patientClips.Length; idx++)
        {
            subtitleText.text = patientSentences[idx];
            audioSource.clip = patientClips[idx];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + speechGap);
        }

        returnToNurseButton.gameObject.SetActive(true);
        returnToNurseButton.onClick.AddListener(OnReturnToNurse);
    }

    void OnReturnToNurse()
    {
        audioSource.PlayOneShot(buttonClickClip);
        returnToNurseButton.onClick.RemoveAllListeners();
        returnToNurseButton.gameObject.SetActive(false);
        StartCoroutine(NurseOutro());
    }

    IEnumerator NurseOutro()
    {
        cameraTransform.position = new Vector3(2.8f, 1.95f, -3.84f);
        cameraTransform.rotation = Quaternion.Euler(10, -65, 0);
        nurse.position = new Vector3(-6.29f, 0, -3.88f);
        nurse.rotation = Quaternion.Euler(0, 217.6f, 0);
        robotAgent.destination = cameraChangePoint2.position + new Vector3(0, robotTransform.position.y, 0);
        
        while (robotAgent.pathPending ||
               robotAgent.remainingDistance > arriveThreshold)
            yield return null;

        cameraTransform.position = new Vector3(-8, 2, -4.7f);
        cameraTransform.rotation = Quaternion.Euler(20,90,0);
        
        robotAgent.destination = nurseOutroPoint.position + new Vector3(0, robotTransform.position.y, 0);
        
        while (robotAgent.pathPending ||
               robotAgent.remainingDistance > arriveThreshold)
            yield return null;
        
        door.Play("ClosingDoor");
        robotTransform.DORotate(new Vector3(0, -58, 0), 1.5f);
        
        yield return new WaitForSeconds(1.75f);
        
        for (idx = 0; idx < nurseClips2.Length; idx++)
        {
            subtitleText.text = nurseSentences2[idx];
            audioSource.clip = nurseClips2[idx];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + speechGap);
        }
        robotAgent.enabled = false;
        Destroy(pillTray);
        
        yield return new WaitForSeconds(nightDelay);
        GameFlowManager.Instance.LoadNextScene();
    }
}
