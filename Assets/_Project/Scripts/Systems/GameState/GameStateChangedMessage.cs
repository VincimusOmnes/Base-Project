using Marmalade.Core;

namespace Marmalade.Systems
{
    /// <summary>
    /// Published by GameStateService when the application transitions to a new GameState.
    /// Subscribe to this message via ISubscriber&lt;GameStateChangedMessage&gt; to react to
    /// state transitions without direct coupling to GameStateService.
    /// </summary>
    public readonly struct GameStateChangedMessage
    {
        /// <summary>
        /// The new state the application has transitioned to.
        /// </summary>
        public readonly GameState State;

        public GameStateChangedMessage(GameState state)
        {
            State = state;
        }
    }
}