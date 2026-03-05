using MessagePipe;

namespace Marmalade.Core
{
    /// <summary>
    /// Defines the high-level states of the game.
    /// Used by IGameStateService to track and communicate state transitions across systems.
    /// </summary>
    public enum GameState
    {
        Bootstrap,
        MainMenu,
        Gameplay,
        Paused,
        Settings
    }

    /// <summary>
    /// Contract for the application-wide game state management service.
    /// Tracks the current GameState and publishes a GameStateChangedMessage via MessagePipe
    /// whenever the state transitions.
    /// </summary>
    public interface IGameStateService
    {
        
        GameState CurrentState { get; }
        void SetState(GameState newState);
    }
}