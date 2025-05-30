using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DG.Tweening;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Models;
using OpenAI.Threads;
using UnityEngine;
using UnityEngine.UI;

public class TTS : MonoBehaviour, ISpeechToTextListener
{
    [Header("OpenAI Settings")]
    private static string key = ""; //TODO: api key
    private static string proj_id = ""; //TODO: project id
    private static string org_id = ""; //TODO: organization id
    private static string ass_id = ""; //TODO: assistant id

    private OpenAIClient api;
    private AssistantResponse assistant;
    private ThreadResponse thread;
    private bool assistantReady = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioSource lipSyncAudioSource;

    [Header("UI")]
    public CanvasGroup retry;
    public CanvasGroup loadingPanel;

    private Queue<AudioClip> speechQueue    = new Queue<AudioClip>();
    private bool isPlayingQueue = false;
    private bool generationComplete = false;

    void Awake()
    {
        api = new OpenAIClient(new OpenAIAuthentication(key, org_id, proj_id));
        ConnectToAssistant();
        
        SpeechToText.Initialize("de-DE");
        if (SpeechToText.CheckPermission())
            SpeechToText.RequestPermissionAsync();
        
        if (retry != null)
            retry.alpha = 0;
        
        HideLoadingPanel();
    }

    private async void ConnectToAssistant()
    {
        try
        {
            assistant = await api.AssistantsEndpoint
                                 .RetrieveAssistantAsync(ass_id);
            Debug.Log($"Assistant loaded: {assistant.Id}");
            
            thread = await api.ThreadsEndpoint
                              .CreateThreadAsync();
            Debug.Log($"Thread created: {thread.Id}");

            assistantReady = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to assistant: {e}");
        }
    }

    public async void SendMessage(string message)
    {
        if (!assistantReady || string.IsNullOrWhiteSpace(message))
            return;

        if (loadingPanel != null)
        {
            loadingPanel.alpha = 1;
            loadingPanel.interactable = true;
            loadingPanel.blocksRaycasts = true;
        }

        try
        {
            await thread.CreateMessageAsync(message);
            var run = await thread.CreateRunAsync(assistant);

            while (run.Status is RunStatus.Queued or RunStatus.InProgress or RunStatus.Cancelling)
                run = await run.WaitForStatusChangeAsync();

            if (run.Status != RunStatus.Completed)
            {
                Debug.LogError($"Run did not complete (Status: {run.Status})");
                HideLoadingPanel();
                return;
            }

            var messages = await thread.ListMessagesAsync();
            var reply = messages.Items[0].Content[0].ToString();

            if (string.IsNullOrEmpty(reply))
            {
                Debug.LogError("No assistant reply found.");
                HideLoadingPanel();
                return;
            }

            TextToSpeech(reply);
        }
        catch (Exception e)
        {
            Debug.LogError($"GPT request failed: {e}");
            HideLoadingPanel();
        }
    }

    public void TextToSpeech(string message)
    {
        List<string> speechPackages = CreateSpeechPackages(message, 2);
        
        speechQueue.Clear();
        generationComplete = false;
        if (!isPlayingQueue)
            StartCoroutine(PlaySpeechQueue());
        
        _ = GenerateAndEnqueueClips(speechPackages);
    }

    private async Task GenerateAndEnqueueClips(List<string> packages)
    {
        foreach (var pkg in packages)
        {
            try
            {
                var request = new SpeechRequest(pkg, Model.TTS_1HD, Voice.Nova, SpeechResponseFormat.WAV);
                var clip    = await api.AudioEndpoint.GetSpeechAsync(request);
                speechQueue.Enqueue(clip);
            }
            catch (Exception ex)
            {
                Debug.LogError("TTS Error: " + ex.Message);
                break;
            }
        }

        generationComplete = true;
    }

    private IEnumerator PlaySpeechQueue()
    {
        isPlayingQueue = true;
        
        while (!generationComplete || speechQueue.Count > 0)
        {
            if (speechQueue.Count == 0)
            {
                yield return null;
                continue;
            }
            
            var clip = speechQueue.Dequeue();
            audioSource.PlayOneShot(clip);
            lipSyncAudioSource.PlayOneShot(clip);
            
            HideLoadingPanel();

            yield return new WaitForSeconds(clip.length);
        }

        isPlayingQueue = false;
    }
    
    private List<string> CreateSpeechPackages(string message, int sentencesPerPackage)
    {
        var packages = new List<string>();
        string pattern = @"(?<=[\.!\?])\s+";
        string[] sentences = Regex.Split(message, pattern);
        for (int i = 0; i < sentences.Length; i += sentencesPerPackage)
        {
            int count = Math.Min(sentencesPerPackage, sentences.Length - i);
            packages.Add(string.Join(" ", sentences, i, count).Trim());
        }
        return packages;
    }
    
    public void StartSpeechRecognition()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
        if (!SpeechToText.IsBusy())
        {
            audioSource.Stop();
            speechQueue.Clear();
            SpeechToText.Start(this);
        }
    #else
        Debug.Log("Speech recognition is nur auf Android verfügbar.");
    #endif
    }

    // Speech‐to‐text callbacks
    void ISpeechToTextListener.OnReadyForSpeech()    => Debug.Log("OnReadyForSpeech");
    void ISpeechToTextListener.OnBeginningOfSpeech() => Debug.Log("OnBeginningOfSpeech");
    void ISpeechToTextListener.OnVoiceLevelChanged(float lvl) { }
    void ISpeechToTextListener.OnPartialResultReceived(string spokenText)
        => Debug.Log("OnPartialResultReceived: " + spokenText);

    void ISpeechToTextListener.OnResultReceived(string spokenText, int? errorCode)
    {
        Debug.Log("OnResultReceived: " + spokenText +
                  (errorCode.HasValue ? " --- Error: " + errorCode : ""));
        if (errorCode != null)
        {
            if (retry != null)
                retry.alpha = 1;
                retry.DOFade(0, 2);
            return;
        }
        SendMessage(spokenText);
    }
    
    private void HideLoadingPanel()
    {
        if (loadingPanel != null)
        {
            loadingPanel.alpha = 0;
            loadingPanel.interactable = false;
            loadingPanel.blocksRaycasts = false;
        }
    }

}
