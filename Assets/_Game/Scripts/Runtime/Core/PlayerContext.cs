using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// What an interactable may access on the player. Starts minimal; inventory and stats
    /// are added here when those systems exist (M2-03, M4-03).
    /// </summary>
    public sealed class PlayerContext
    {
        public GameObject Player { get; }

        public PlayerContext(GameObject player)
        {
            Player = player;
        }
    }
}
