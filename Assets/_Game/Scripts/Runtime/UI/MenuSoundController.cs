using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class MenuSoundController : MonoBehaviour
    {
        public static MenuSoundController Instance { get; private set; }

        [Header("Sound Effects")]
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip clickSound;

        [Header("Volume Settings")]
        [SerializeField, Range(0f, 1f)]
        private float hoverVolume = 0.25f;

        [SerializeField, Range(0f, 1f)]
        private float clickVolume = 0.4f;

        private AudioSource audioSource;

        private void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayHover()
        {
            if (hoverSound != null)
                audioSource.PlayOneShot(hoverSound, hoverVolume);
        }

        public void PlayClick()
        {
            if (clickSound != null)
                audioSource.PlayOneShot(clickSound, clickVolume);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
