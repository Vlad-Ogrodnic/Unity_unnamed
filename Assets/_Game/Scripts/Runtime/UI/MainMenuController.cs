
using UnityEngine;

namespace Game.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject loadGamePanel;
        [SerializeField] private GameObject settingsPanel;

        private void Start()
        {
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            mainMenuPanel.SetActive(true);
            loadGamePanel.SetActive(false);
            settingsPanel.SetActive(false);
        }

        public void ShowLoadGame()
        {
            mainMenuPanel.SetActive(false);
            loadGamePanel.SetActive(true);
            settingsPanel.SetActive(false);
        }

        public void ShowSettings()
        {
            mainMenuPanel.SetActive(false);
            loadGamePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            Application.Quit();
        }
    }
}
