using System;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Marmalade.Systems
{
    /// <summary>
    /// Drives the LoadingScreenView in response to scene load events published via MessagePipe.
    /// Subscribes to SceneLoadStartedMessage, SceneLoadProgressMessage, and SceneLoadCompletedMessage
    /// and calls the corresponding View methods.
    /// </summary>
    public class LoadingScreenPresenter : IInitializable, IDisposable
    {
        private readonly LoadingScreenView _view;
        private readonly ISubscriber<SceneLoadStartedMessage> _loadStarted;
        private readonly ISubscriber<SceneLoadProgressMessage> _loadProgress;
        private readonly ISubscriber<SceneLoadCompletedMessage> _loadCompleted;

        private IDisposable _subscriptions;

        public LoadingScreenPresenter(
            LoadingScreenView view,
            ISubscriber<SceneLoadStartedMessage> loadStarted,
            ISubscriber<SceneLoadProgressMessage> loadProgress,
            ISubscriber<SceneLoadCompletedMessage> loadCompleted)
        {
            _view = view;
            _loadStarted = loadStarted;
            _loadProgress = loadProgress;
            _loadCompleted = loadCompleted;
        }

        /// <summary>
        /// Subscribes to scene load messages and begins driving the view.
        /// Called automatically by VContainer after all dependencies are injected.
        /// </summary>
        public void Initialize()
        {
            IDisposable loadStartedSubscription = _loadStarted.Subscribe(_ =>
            {
                Debug.Log("[LoadingScreenPresenter] Load started");
                _view.Show();
            });
            IDisposable loadProgressSubscription = _loadProgress.Subscribe(msg =>
            {
                Debug.Log($"[LoadingScreenPresenter] Progress: {msg.Progress}");
                _view.SetProgress(msg.Progress);
            });
            IDisposable loadCompletedSubscription = _loadCompleted.Subscribe(_ =>
            {
                Debug.Log("[LoadingScreenPresenter] Load completed");
                _view.Hide();
            });

            _subscriptions = DisposableBag.Create(loadStartedSubscription, loadProgressSubscription, loadCompletedSubscription);
        }

        /// <summary>
        /// Disposes all MessagePipe subscriptions.
        /// Called automatically by VContainer when the scope is destroyed.
        /// </summary>
        public void Dispose()
        {
            _subscriptions?.Dispose();
        }
    }
}