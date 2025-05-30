using TMPro;
using UnityEngine;

public class AudioManagerVision : MonoBehaviour
{
    [Header("SFX Clips")]
    public AudioClip buttonClickClip;
    public AudioClip soundWarningClip;
    public AudioClip gameWonClip;
    public AudioClip gameLostClip;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    public GameObject muteButton;

    private bool isMuted = false;

    public AudioSource catAudio;
    public AudioSource patientAudio;

    /// <summary>Play a one‑shot, unless muted.</summary>
    private void Play(AudioClip clip)
    {
        if (isMuted || clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()  => Play(buttonClickClip);

    public void PlayArmEnd()
    {
        sfxSource.Stop();
    }
    public void PlayGameWon()     => Play(gameWonClip);
    public void PlayGameLost()     => Play(gameLostClip);
    public void PlaySoundWarning() => Play(soundWarningClip);

    /// <summary>Toggle mute/unmute.</summary>
    public void ToggleMute()
    {
        var buttonText = muteButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText.text == "Ton aus") buttonText.text = "Ton ein";
        else buttonText.text = "Ton aus";
        isMuted = !isMuted;
        catAudio.mute = !catAudio.mute;
        patientAudio.mute = !patientAudio.mute;
    }
}

