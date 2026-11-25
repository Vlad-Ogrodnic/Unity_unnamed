using Game.Core;
using Game.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    /// <summary>
    /// PROVISIONAL (M2-02), replaced by the real end screen in M4-07: when the player dies the game
    /// stops and "Ai murit — R pentru restart" appears; R reloads the scene. Drawn with IMGUI on
    /// purpose, so it needs no UI assets.
    /// </summary>
    public sealed class ProvisionalDeathScreen : MonoBehaviour
    {
        private const string Message = "Ai murit — R pentru restart";

        [SerializeField] private Health _playerHealth;
        [SerializeField] private PlayerInputReader _input;

        private GUIStyle _style;
        private bool _shown;

        private void OnEnable()
        {
            _playerHealth.Died += OnPlayerDied;
            _input.RestartPressed += OnRestartPressed;
        }

        private void OnDisable()
        {
            _playerHealth.Died -= OnPlayerDied;
            _input.RestartPressed -= OnRestartPressed;
        }

        private void LateUpdate()
        {
            // Hit stop restores the time scale when it ends; keep the game stopped while dead.
            if (_shown)
                Time.timeScale = 0f;
        }

        private void OnPlayerDied()
        {
            _shown = true;
            Time.timeScale = 0f;
        }

        private void OnRestartPressed()
        {
            if (!_shown)
                return;

            Time.timeScale = 1f;
            Scene scene = SceneManager.GetActiveScene();
#if UNITY_EDITOR
            // Sandbox scenes aren't in the build list, so load by path in the editor.
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scene.path, new LoadSceneParameters(LoadSceneMode.Single));
#else
            SceneManager.LoadScene(scene.buildIndex);
#endif
        }

        private void OnGUI()
        {
            if (!_shown)
                return;

            _style ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.RoundToInt(Screen.height * 0.06f),
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), Message, _style);
        }
    }
}
