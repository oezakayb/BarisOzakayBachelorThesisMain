using TMPro;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("SFX Clips")]
    public AudioClip buttonClickClip;
    public AudioClip armStartClip;
    public AudioClip pillDropClip;
    public AudioClip levelCompletedClip;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    public GameObject muteButton;

    private bool isMuted = false;

    /// <summary>Play a one‑shot, unless muted.</summary>
    private void Play(AudioClip clip)
    {
        if (isMuted || clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()  => Play(buttonClickClip);

    public void PlayArmStart()
    {
        Play(armStartClip);
    }
    public void PlayArmEnd()
    {
        sfxSource.Stop();
    }
    public void PlayPillDrop()     => Play(pillDropClip);
    public void PlayLevelCompleted()     => Play(levelCompletedClip);

    /// <summary>Toggle mute/unmute.</summary>
    public void ToggleMute()
    {
        var buttonText = muteButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText.text == "Ton aus") buttonText.text = "Ton ein";
        else buttonText.text = "Ton aus";
        isMuted = !isMuted;
    }
}

