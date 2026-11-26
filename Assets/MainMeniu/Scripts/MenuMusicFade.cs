using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 3f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        float targetVolume = audioSource.volume;

        audioSource.volume = 0f;
        audioSource.Play();

        StartCoroutine(FadeIn(targetVolume));
    }

    private IEnumerator FadeIn(float targetVolume)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            audioSource.volume = Mathf.Lerp(
                0f,
                targetVolume,
                elapsedTime / fadeDuration
            );

            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}