using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SoundMeter : MonoBehaviour
{
    [Tooltip("Current sound meter value.")]
    public float currentValue = 0f;
    [Tooltip("Maximum sound meter value.")]
    public float maxValue = 100f;
    [Tooltip("UI Slider representing the sound meter.")]
    public Slider soundSlider;
    [Tooltip("First UI Image whose G and B channels will fade out.")]
    public Image imageA;
    [Tooltip("Second UI Image whose G and B channels will fade out.")]
    public Image imageB;
    [Tooltip("Duration of the slider animation in seconds.")]
    public float animationDuration = 0.5f;

    private Color originalColorA;
    private Color originalColorB;

    void Start()
    {
        if (soundSlider != null)
        {
            soundSlider.maxValue = maxValue;
            soundSlider.value = currentValue;
        }
        if (imageA != null)
            originalColorA = imageA.color;
        if (imageB != null)
            originalColorB = imageB.color;
    }
    
    public void Increase(float amount)
    {
        SetValue(currentValue + amount);
    }
    
    public void Decrease(float amount)
    {
        SetValue(currentValue - amount);
    }
    
    private void SetValue(float newValue)
    {
        currentValue = Mathf.Clamp(newValue, 0, maxValue);
        if (soundSlider != null)
        {
            soundSlider.DOKill();
            soundSlider.DOValue(currentValue, animationDuration)
                .OnUpdate(() => UpdateImages(soundSlider.normalizedValue));
        }
        else
        {
            UpdateImages(Mathf.InverseLerp(0f, maxValue, currentValue));
        }
    }

    private void UpdateImages(float t)
    {
        float fade = 1f - t;
        if (imageA != null)
        {
            Color c = originalColorA;
            c.g = c.g * fade;
            c.b = c.b * fade;
            imageA.color = c;
        }
        if (imageB != null)
        {
            Color c = originalColorB;
            c.g = c.g * fade;
            c.b = c.b * fade;
            c.r = Mathf.Lerp(originalColorB.r, 1f, t);
            imageB.color = c;
        }
    }
}

