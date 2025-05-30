using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using Utilities.Extensions;

public class PreNightVisionSceneManager : MonoBehaviour
{
    [Header("Subtitles & Speech")]
    public TextMeshProUGUI subtitleText;
    public string[] caregiverSentences;
    public AudioClip[] caregiverClips; 
    public RawImage nurse;

    [Header("UI Button")]
    public Button goToPatientButton;

    public AudioManager AudioManager;

    [Header("Navigation")]
    public Transform doorPoint; 
    public Transform cameraChangePoint;
    public Transform robotTransform;
    private NavMeshAgent agent;

    [Header("Timings")]
    public float speechGap = 0.1f;
    public float postClickDelay = 2f;

    private AudioSource audioSource;

    public Animator glassDoor, door;
    public Transform camera;

    void Start()
    {
        if (robotTransform == null)
        {
            var player = RobotHandReference.Instance;
            if (player != null) robotTransform = player.transform;
        }
        agent = robotTransform.GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        goToPatientButton.gameObject.SetActive(false);

        robotTransform.position = new Vector3(-12.5f, 0, -0.15f);
        robotTransform.rotation = Quaternion.Euler(0,90,0);
        
        glassDoor.Play("OpeningDoor");
        door.Play("OpeningDoor");
        
        StartCoroutine(RunCaregiverIntro());
    }

    IEnumerator RunCaregiverIntro()
    {
        for (int i = 0; i < caregiverSentences.Length; i++)
        {
            subtitleText.text = caregiverSentences[i];

            if (caregiverClips != null && i < caregiverClips.Length && caregiverClips[i] != null)
            {
                audioSource.clip = caregiverClips[i];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length + speechGap);
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }
        
        goToPatientButton.gameObject.SetActive(true);
        goToPatientButton.onClick.AddListener(OnGoToPatientClicked);
    }

    void OnGoToPatientClicked()
    {
        AudioManager.PlayButtonClick();
        goToPatientButton.onClick.RemoveAllListeners();
        goToPatientButton.SetActive(false);
        
        StartCoroutine(WaitThenLoadNext());
    }

    IEnumerator WaitThenLoadNext()
    {
        nurse.DOFade(0, 1);
        yield return new WaitForSeconds(1);
        Vector3 target = new Vector3(
            cameraChangePoint.position.x,
            robotTransform.position.y,
            cameraChangePoint.position.z
        );
        agent.enabled = true;
        agent.destination = target;
        while (agent.pathPending || agent.remainingDistance > 0.1)
        {
            yield return null;
        }

        camera.position = new Vector3(-7.58f, 1.3f, -0.25f);
        camera.rotation = Quaternion.Euler(10, 180, 0);


        var temp = robotTransform.rotation;
        robotTransform.LookAt(doorPoint);
        var targetRot = robotTransform.rotation;
        robotTransform.rotation = temp;
        robotTransform.DORotate(targetRot.eulerAngles, 1.5f);
        yield return new WaitForSeconds(1.5f);
        
        
        target = new Vector3(
            doorPoint.position.x,
            robotTransform.position.y,
            doorPoint.position.z
        );
        
        agent.destination = target;
        while (agent.pathPending || agent.remainingDistance > 0.1)
        {
            yield return null;
        }

        agent.enabled = false;
        
        temp = robotTransform.rotation;
        robotTransform.LookAt(new Vector3(door.gameObject.transform.position.x, 
            robotTransform.position.y,
            door.gameObject.transform.position.z));
        targetRot = robotTransform.rotation;
        robotTransform.rotation = temp;
        robotTransform.DORotate(targetRot.eulerAngles, 1.5f);
        yield return new WaitForSeconds(1.5f);

        

        GameFlowManager.Instance.LoadNextScene();
    }
}
