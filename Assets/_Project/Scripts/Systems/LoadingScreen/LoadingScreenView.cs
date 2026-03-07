using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;
using VContainer;

namespace Marmalade.Systems
{
    /// <summary>
    /// Manages the visual state of the loading screen.
    /// Exposes simple methods for the presenter to call.
    /// </summary>
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _progressBarFillImage;
        [SerializeField] private TextMeshProUGUI _progressText;

        [Inject] private LoadingScreenConfig _config;

        private Tween _progressTween;

        public void Start()
        {
            _canvasGroup.alpha = 0f;
            _progressBarFillImage.fillAmount = 0f;
        }
        
        /// <summary>
        /// Fades the loading screen in and resets the progress bar to empty.
        /// </summary>
        public void Show()
        {
            _progressBarFillImage.fillAmount = 0f;
            var tween = Tween.Alpha(_canvasGroup, endValue: 1f, duration: _config.FadeInDuration);
            SetProgressText(0f);
        }

        /// <summary>
        /// Fades the loading screen out.
        /// </summary>
        public void Hide()
        {
            Tween.Alpha(_canvasGroup, endValue: 0f, duration: _config.FadeOutDuration);
            SetProgressText(1f);
        }

        /// <summary>
        /// Smoothly tweens the progress bar toward the given value.
        /// Value should be between 0 and 1.
        /// </summary>
        public void SetProgress(float progress)
        {
            // Avoid triggering a tween when the value hasn't changed.
            // PrimeTween warns and skips tweens where start and end values are the same.
            if (_progressBarFillImage.fillAmount == progress) return;
            _progressTween.Stop();
            Tween.UIFillAmount(_progressBarFillImage, endValue: progress, duration:_config.ProgressSmoothing);
            SetProgressText(progress);
        }

        private void SetProgressText(float progress)
        {
            float progressPercentage = progress * 100;
            _progressText.text = $"{progressPercentage:0}%";
        }
    }
}