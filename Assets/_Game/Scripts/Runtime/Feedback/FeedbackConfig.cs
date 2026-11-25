using UnityEngine;

namespace Game.Feedback
{
    /// <summary>Hit feedback values (GDD ch. 4, Hit feedback).</summary>
    [CreateAssetMenu(menuName = "Game/Feedback Config", fileName = "FeedbackConfig")]
    public sealed class FeedbackConfig : ScriptableObject
    {
        [Tooltip("Material shown for a moment on whatever gets hit (plain white).")]
        [SerializeField] private Material _hitFlashMaterial;

        [SerializeField, Min(0f)] private float _hitFlashDuration = 0.08f;

        [Tooltip("Seconds between on/off toggles while blinking during i-frames.")]
        [SerializeField, Min(0.01f)] private float _invulnerabilityBlinkInterval = 0.1f;

        public Material HitFlashMaterial => _hitFlashMaterial;
        public float HitFlashDuration => _hitFlashDuration;
        public float InvulnerabilityBlinkInterval => _invulnerabilityBlinkInterval;
    }
}
