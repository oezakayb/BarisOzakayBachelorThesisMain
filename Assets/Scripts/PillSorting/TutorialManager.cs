using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Content")]
    public Sprite[] tutorialImages;

    [Header("UI References")]
    public GameObject tutorialPanel;
    public Image tutorialImage;
    public AspectRatioFitter aspectRatioFitter;

    [Header("Buttons")]
    public Button nextButton;
    public Button prevButton;

    private int currentPage = 0;

    void Awake()
    {
        tutorialPanel.SetActive(true);
        currentPage = 0;
        ShowPage(currentPage);
        UpdateNavigationButtons();
    }

    void Start()
    {
        UpdateNavigationButtons();
    }

    public void OpenTutorial()
    {
        currentPage = 0;
        tutorialPanel.SetActive(true);
        ShowPage(currentPage);
        UpdateNavigationButtons();
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void NextPage()
    {
        if (currentPage < tutorialImages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
            UpdateNavigationButtons();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
            UpdateNavigationButtons();
        }
    }

    void ShowPage(int index)
    {
        if (index >= 0 && index < tutorialImages.Length)
        {
            var sprite = tutorialImages[index];
            tutorialImage.sprite = sprite;
            aspectRatioFitter.aspectRatio = sprite.rect.width / sprite.rect.height;
        }
    }

    void UpdateNavigationButtons()
    {
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < tutorialImages.Length - 1;
    }
}

