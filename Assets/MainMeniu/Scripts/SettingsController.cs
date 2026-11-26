using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Category Panels")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Category Buttons")]
    [SerializeField] private Button audioButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button controlsButton;

    [Header("Category Colors")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor =
        new Color(0.65f, 0.65f, 0.65f, 1f);

    private void OnEnable()
    {
        ShowAudio();
    }

    public void ShowAudio()
    {
        ShowCategory(audioPanel, audioButton);
    }

    public void ShowGraphics()
    {
        ShowCategory(graphicsPanel, graphicsButton);
    }

    public void ShowControls()
    {
        ShowCategory(controlsPanel, controlsButton);
    }

    private void ShowCategory(GameObject activePanel, Button activeButton)
    {
        audioPanel.SetActive(activePanel == audioPanel);
        graphicsPanel.SetActive(activePanel == graphicsPanel);
        controlsPanel.SetActive(activePanel == controlsPanel);

        audioButton.image.color =
            activeButton == audioButton ? selectedColor : unselectedColor;

        graphicsButton.image.color =
            activeButton == graphicsButton ? selectedColor : unselectedColor;

        controlsButton.image.color =
            activeButton == controlsButton ? selectedColor : unselectedColor;
    }
}