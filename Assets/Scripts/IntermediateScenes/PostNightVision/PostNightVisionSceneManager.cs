using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Utilities.Extensions;

public class PostNightVisionSceneManager : MonoBehaviour
{
    [Header("Player Reset")]
    public Vector3 playerStartPosition  = new Vector3(1f, 0f, 1f);
    public Vector3 playerStartRotation  = new Vector3(1f, 0f, 1f);

    [Header("Subtitles & Audio")]
    public TextMeshProUGUI subtitleText;
    [TextArea(2,5)] public string[] nurseOutroSentences;
    public AudioClip[] nurseOutroClips; 

    [Header("Nurse Portrait (UI)")]
    public CanvasGroup nursePortraitGroup;
    [Header("Simulation Finished UI")]
    public CanvasGroup    simulationFinishedUI;
    public Button  quitButton;

    private AudioSource audioSource;

    void Start()
    {
       
        var player = RobotHandReference.Instance.gameObject;
        player.transform.position = playerStartPosition;
        player.transform.rotation = Quaternion.Euler(playerStartRotation);

        audioSource = GetComponent<AudioSource>();
        
        simulationFinishedUI.alpha = 0f;
        simulationFinishedUI.interactable = false;
        simulationFinishedUI.blocksRaycasts = false;
        quitButton.gameObject.SetActive(false);
        
        StartCoroutine(PlayNurseOutro());
    }

    IEnumerator PlayNurseOutro()
    {
        for (int i = 0; i < nurseOutroSentences.Length; i++)
        {
            subtitleText.text = nurseOutroSentences[i];

            if (nurseOutroClips != null &&
                i < nurseOutroClips.Length &&
                nurseOutroClips[i] != null)
            {
                audioSource.clip = nurseOutroClips[i];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length + 0.1f);
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }
        
        if (nursePortraitGroup != null)
            nursePortraitGroup.DOFade(0f, 0.5f)
                .OnComplete(()=>subtitleText.gameObject.transform.parent.SetActive(false));

        gameObject.GetComponent<AudioManager>().PlayLevelCompleted();
        
        simulationFinishedUI.DOFade(1f, 0.5f).OnStart(() =>
        {
            simulationFinishedUI.interactable = true;
            simulationFinishedUI.blocksRaycasts = true;
        });
        
        quitButton.gameObject.SetActive(true);
        quitButton.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        });
    }
}

