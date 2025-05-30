using System;
using System.Collections;
using System.Collections.Generic;
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

public class TTSBackup: MonoBehaviour, ISpeechToTextListener
{
    private static string key = ""; //TODO: api key
    private static string proj_id = ""; //TODO: project id
    private static string org_id = ""; //TODO: organization id
    private static string ass_id = ""; //TODO: assistant id

    private OpenAIClient api;
    private AssistantResponse assistant;
    private ThreadResponse thread;

    private bool assistantReady = false;

    public AudioSource audioSource;
    public AudioSource lipSyncAudioSource;
    public CanvasGroup retry;

    private Queue<AudioClip> speechQueue   = new Queue<AudioClip>();
    private bool isPlayingQueue = false;

    void Awake()
    {
        api = new OpenAIClient(new OpenAIAuthentication(key, org_id, proj_id));
        ConnectToAssistant();

        SpeechToText.Initialize("de-DE");
        if (SpeechToText.CheckPermission())
            SpeechToText.RequestPermissionAsync();
    }

    private async void ConnectToAssistant()
    {
        try
        {
            assistant = await api.AssistantsEndpoint.RetrieveAssistantAsync(ass_id);
            Debug.Log($"Assistant loaded: {assistant.Id} @ {assistant.CreatedAt}");

            var run = await assistant.CreateThreadAndRunAsync(new CreateThreadRequest());
            thread = await run.GetThreadAsync();
            Debug.Log($"Thread created: {thread.Id} @ {thread.CreatedAt}");
            
            assistantReady = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to assistant: {e}");
        }
    }

    public async void SendMessage(string message)
    {
        if (!assistantReady)
        {
            Debug.LogWarning("Assistant not ready yet. Bitte kurz warten…");
            return;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            Debug.LogWarning("Leere Nachricht wird nicht gesendet.");
            return;
        }

        // Try up to 2 times for transient bad‐request errors
        for (int attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                var m = await thread.CreateMessageAsync(message);
                var runResponse = await thread.CreateRunAsync(assistant);
                var messages = await thread.ListMessagesAsync();
                
                var answer = messages.Items[1].Content[0].ToString();

                for (int i = 0; i < messages.Items.Count; i++)
                {
                    Debug.Log(i + ": " + messages.Items[i].Content[0]);
                }
                
                Debug.Log($"GPT antwortet: {answer}");

                TextToSpeech(answer);
                return;
            }
            catch (Exception e) when (attempt == 1)
            {
                Debug.LogWarning($"GPT BadRequest (Versuch {attempt}): {e.Message}. Erneuter Versuch…");
                await Task.Delay(500);
                continue;
            }
            catch (Exception e)
            {
                Debug.LogError($"GPT Anfrage fehlgeschlagen nach Retry: {e.Message}");
                return;
            }
        }
    }

    public async void TextToSpeech(string message)
    {
        List<string> speechPackages = CreateSpeechPackages(message, 2);
        foreach (string pkg in speechPackages)
        {
            try
            {
                var request = new SpeechRequest(pkg, Model.TTS_1HD, Voice.Nova, SpeechResponseFormat.WAV);
                AudioClip clip = await api.AudioEndpoint.GetSpeechAsync(request);
                speechQueue.Enqueue(clip);
            }
            catch (Exception ex)
            {
                Debug.LogError("TTS Error: " + ex.Message);
            }
        }
        if (!isPlayingQueue)
            StartCoroutine(PlaySpeechQueue());
    }

    private List<string> CreateSpeechPackages(string message, int sentencesPerPackage)
    {
        List<string> packages = new List<string>();
        string pattern = @"(?<=[\.!\?])\s+";
        string[] sentences = Regex.Split(message, pattern);
        for (int i = 0; i < sentences.Length; i += sentencesPerPackage)
        {
            int count = Math.Min(sentencesPerPackage, sentences.Length - i);
            string pkg = string.Join(" ", sentences, i, count).Trim();
            packages.Add(pkg);
        }
        return packages;
    }

    private IEnumerator PlaySpeechQueue()
    {
        isPlayingQueue = true;
        while (speechQueue.Count > 0)
        {
            AudioClip clip = speechQueue.Dequeue();
            audioSource.PlayOneShot(clip);
            lipSyncAudioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
        isPlayingQueue = false;
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

    void ISpeechToTextListener.OnReadyForSpeech()    => Debug.Log("OnReadyForSpeech");
    void ISpeechToTextListener.OnBeginningOfSpeech() => Debug.Log("OnBeginningOfSpeech");
    void ISpeechToTextListener.OnVoiceLevelChanged(float lvl) { /* optional */ }
    void ISpeechToTextListener.OnPartialResultReceived(string spokenText)
        => Debug.Log("OnPartialResultReceived: " + spokenText);

    void ISpeechToTextListener.OnResultReceived(string spokenText, int? errorCode)
    {
        Debug.Log("OnResultReceived: " + spokenText + (errorCode.HasValue ? " --- Error: " + errorCode : ""));
        if (errorCode != null)
        {
            retry.alpha = 1;
            retry.DOFade(0, 2);
            return;
        }
        SendMessage(spokenText);
    }
}
