using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class GameFlowManager: MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Fade Settings")]
    [Tooltip("CanvasGroup used for fade. Should cover whole screen.")]
    public CanvasGroup fadeCanvasGroup;
    [Tooltip("Time in seconds for fade in/out.")]
    public float fadeDuration = 1f;

    [Header("Loading Screen (Optional)")]
    [Tooltip("Optional loading screen GameObject to show during scene load.")]
    public GameObject loadingScreen;
    [Tooltip("Optional progress bar slider on the loading screen.")]
    public Slider progressBar;

    private int currentSceneIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        Debug.Log($"Loaded scene index: {currentSceneIndex}");
    }
    public void LoadNextScene()
    {
        int nextIndex = currentSceneIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(Transition(nextIndex));
        }
        else
        {
            Debug.LogWarning("No next scene in build settings.");
        }
    }
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(Transition(sceneIndex));
        }
        else
        {
            Debug.LogWarning($"Scene index {sceneIndex} is out of range.");
        }
    }

    public void LoadSameScene()
    {
        StartCoroutine(Transition(currentSceneIndex));
    }

    private IEnumerator Transition(int sceneIndex)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            if (progressBar != null)
                progressBar.value = Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }
        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeCanvasGroup == null)
            yield break;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = endAlpha;
    }
}
