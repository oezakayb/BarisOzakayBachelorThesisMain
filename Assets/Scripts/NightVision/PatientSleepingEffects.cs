using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class PatientSleepEffects : MonoBehaviour
{
    [Header("Audio Snoring")]
    [Tooltip("Snoring audio clip (looped)")]
    public AudioClip snoreClip;
    [Tooltip("Snore volume (0..1)")]
    public float snoreVolume = 0.5f;

    [Header("Zzz Animation")]
    [Tooltip("Spawn point above patient head for Zzz UI elements.")]
    public Transform zzzSpawnPoint;
    [Tooltip("Prefab of the Zzz UI element (should be a Text/Image under a world-space Canvas)")]
    public GameObject zzzPrefab;
    [Tooltip("World-space Canvas to parent Zzz elements into.")]
    public Canvas zzzCanvas;
    [Tooltip("Seconds between each Zzz spawn.")]
    public float spawnInterval = 1f;
    [Tooltip("Duration of each Zzz drift & fade.")]
    public float driftDuration = 2f;
    [Tooltip("Vertical drift distance in world units.")]
    public float driftDistance = 1f;

    private AudioSource audioSource;
    private Coroutine zzzRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = snoreClip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = snoreVolume;
    }

    void Start()
    {
        if (snoreClip != null)
            audioSource.Play();
        if (zzzPrefab != null && zzzCanvas != null && zzzSpawnPoint != null)
            zzzRoutine = StartCoroutine(SpawnZzzLoop());
    }

    void OnDestroy()
    {
        if (zzzRoutine != null)
            StopCoroutine(zzzRoutine);
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    IEnumerator SpawnZzzLoop()
    {
        while (true)
        {
            SpawnSingleZzz();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSingleZzz()
    {
        GameObject zzz = Instantiate(zzzPrefab, zzzCanvas.transform);
        zzz.transform.position = zzzSpawnPoint.position;
        
        Graphic gfx = zzz.GetComponent<Graphic>();
        Vector3 targetPos = zzzSpawnPoint.position + Vector3.up * driftDistance;
        if (gfx != null)
        {
            Color start = gfx.color;
            start.a = 1f;
            gfx.color = start;
            
            zzz.transform.DOMove(targetPos, driftDuration).SetEase(Ease.OutQuad);
            gfx.DOFade(0f, driftDuration)
               .OnComplete(() => Destroy(zzz));
        }
        else
        {
            zzz.transform.DOMove(targetPos, driftDuration).SetEase(Ease.OutQuad);
            Destroy(zzz, driftDuration);
        }
    }
}

