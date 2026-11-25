using UnityEngine;

namespace Game.Feedback
{
    /// <summary>Hit feedback values (GDD ch. 4, Hit feedback).</summary>
    [CreateAssetMenu(menuName = "Game/Feedback Config", fileName = "FeedbackConfig")]
    public sealed class FeedbackConfig : ScriptableObject
    {
        [Header("Flashes")]
        [Tooltip("Material shown for a moment on whatever gets hit (plain white). Also used for telegraph blinking.")]
        [SerializeField] private Material _hitFlashMaterial;

        [SerializeField, Min(0f)] private float _hitFlashDuration = 0.08f;

        [Tooltip("Seconds between on/off toggles while blinking during i-frames.")]
        [SerializeField, Min(0.01f)] private float _invulnerabilityBlinkInterval = 0.1f;

        [Tooltip("Seconds between on/off toggles while an enemy telegraphs an attack.")]
        [SerializeField, Min(0.01f)] private float _telegraphBlinkInterval = 0.08f;

        [Header("Melee")]
        [Tooltip("How long the swing arc stays visible, in real seconds.")]
        [SerializeField, Min(0f)] private float _meleeSwingDuration = 0.1f;

        [Header("Hit stop and screen shake (melee hits)")]
        [Tooltip("Game pause on a melee hit, in real seconds (a few frames).")]
        [SerializeField, Min(0f)] private float _hitStopDuration = 0.05f;

        [Tooltip("Largest camera offset during a shake, in m.")]
        [SerializeField, Min(0f)] private float _screenShakeStrength = 0.12f;

        [SerializeField, Min(0f)] private float _screenShakeDuration = 0.15f;

        [Header("Denied action (e.g. potion at full HP)")]
        [SerializeField, Min(0f)] private float _deniedShakeDuration = 0.25f;

        [Tooltip("Side-to-side wiggle of the character's visual, in m.")]
        [SerializeField, Min(0f)] private float _deniedShakeAmplitude = 0.08f;

        public Material HitFlashMaterial => _hitFlashMaterial;
        public float HitFlashDuration => _hitFlashDuration;
        public float InvulnerabilityBlinkInterval => _invulnerabilityBlinkInterval;
        public float TelegraphBlinkInterval => _telegraphBlinkInterval;
        public float MeleeSwingDuration => _meleeSwingDuration;
        public float HitStopDuration => _hitStopDuration;
        public float ScreenShakeStrength => _screenShakeStrength;
        public float ScreenShakeDuration => _screenShakeDuration;
        public float DeniedShakeDuration => _deniedShakeDuration;
        public float DeniedShakeAmplitude => _deniedShakeAmplitude;
    }
}
