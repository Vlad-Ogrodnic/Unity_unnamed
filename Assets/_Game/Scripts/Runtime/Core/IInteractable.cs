namespace Game.Core
{
    /// <summary>Objects the player uses with the Interact key (choice room items, Forge).</summary>
    public interface IInteractable
    {
        /// <summary>Text shown above the object while the player is close, e.g. "Buy (10 coins)".</summary>
        string Prompt { get; }

        bool CanInteract(PlayerContext player);

        void Interact(PlayerContext player);
    }
}
