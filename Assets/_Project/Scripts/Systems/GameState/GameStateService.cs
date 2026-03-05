using MessagePipe;
using Marmalade.Core;

namespace Marmalade.Systems
{
    /// <summary>
    /// Manages the application-wide game state and notifies subscribers of state transitions.
    /// Publishes a GameStateChangedMessage via MessagePipe whenever the state changes,
    /// allowing any system to react to transitions without direct coupling to this service.
    /// </summary>
    public class GameStateService : IGameStateService
    {
        private readonly IPublisher<GameStateChangedMessage> _publisher;
        private GameState _currentState;

        /// <summary>
        /// The current active state of the game.
        /// </summary>
        public GameState CurrentState => _currentState;

        /// <summary>
        /// Initialises the service with the Bootstrap state as the default.
        /// </summary>
        public GameStateService(IPublisher<GameStateChangedMessage> publisher)
        {
            _currentState = GameState.Bootstrap;
            _publisher = publisher;
        }

        /// <summary>
        /// Transitions the application to the given state and publishes a GameStateChangedMessage.
        /// Has no effect if the requested state is the same as the current state.
        /// </summary>
        public void SetState(GameState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            _publisher.Publish(new GameStateChangedMessage(_currentState));
        }
    }
}