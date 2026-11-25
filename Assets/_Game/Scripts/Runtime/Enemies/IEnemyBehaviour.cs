using UnityEngine;

namespace Game.Enemies
{
    /// <summary>An enemy's behaviour component. The room that activates the enemy gives it its target.</summary>
    public interface IEnemyBehaviour
    {
        void SetTarget(Transform target);
    }
}
