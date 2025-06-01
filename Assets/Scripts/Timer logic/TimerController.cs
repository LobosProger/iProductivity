using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TMP_Text _remainingTimeMinutesText;
    [SerializeField] private TMP_Text _remainingTimeSecondsText;
    [SerializeField] private Image _timerProgressBar;
    [Space]
    [Header("Timer buttons")]
    [SerializeField] private Button _stopTimerButton;
    [SerializeField] private ExtendedToggle _pauseAndResumeTimerToggle;
    
    public event Action onSessionOfTimerStarted;
    public event Action onSessionOfTimerEnded;
    public event Action onTimerStopped;
    public event Action onTimerPaused;
    public event Action onTimerResumed;

    private TimeSpan _currentTimeOfSession;
    private TimeSpan _initiallySetTimeOfSession;
    private CancellationTokenSource _timerToken;
    private AudioSource _beepSound;

    private const float k_timerDurationRefreshInSeconds = 1;

    private void Start()
    {
        _beepSound = GetComponent<AudioSource>();
        
        _stopTimerButton.onClick.AddListener(StopTimer);
        _pauseAndResumeTimerToggle.onSwitchedToggle.AddListener(SwitchTimerState);
    }

    private void OnDestroy()
    {
        _timerToken?.Cancel();
        _timerToken?.Dispose();
    }

    private void StopTimer()
    {
        if (!IsRemainedTimeAtTheTimer())
            return;
        
        _currentTimeOfSession = TimeSpan.Zero;
        UpdateActualViewAtTheTimer();
        _pauseAndResumeTimerToggle.SetIsOnWithoutNotify(false);
        
        _timerToken?.Cancel();
        onTimerStopped?.Invoke();
    }

    private bool IsRemainedTimeAtTheTimer()
    {
        return _currentTimeOfSession.Subtract(TimeSpan.FromSeconds(1)).TotalSeconds > 0;
    }
    
    private void UpdateActualViewAtTheTimer()
    {
        _remainingTimeMinutesText.text = _currentTimeOfSession.ToString("mm");
        _remainingTimeSecondsText.text = _currentTimeOfSession.ToString("ss");
        
        var fillAmount = (float)_currentTimeOfSession.TotalSeconds / (float)_initiallySetTimeOfSession.TotalSeconds;
        _timerProgressBar.fillAmount = fillAmount;
    }

    private void SwitchTimerState(bool pauseTimer)
    {
        if (!IsRemainedTimeAtTheTimer())
            return;
        
        if (pauseTimer)
        {
            onTimerPaused?.Invoke();
            _timerToken?.Cancel();
        }
        else
        {
            onTimerResumed?.Invoke();
            LaunchTimerAsync().Forget();
        }
    }
    
    public void LaunchSessionOfTimer()
    {
        onSessionOfTimerStarted?.Invoke();
        _pauseAndResumeTimerToggle.SetIsOnWithoutNotify(false);
        
        LaunchTimerAsync().Forget();
    }

    private async UniTask LaunchTimerAsync()
    {
        _timerToken = new();
        while (true)
        {
            UpdateActualViewAtTheTimer();

            await UniTask.WaitForSeconds(k_timerDurationRefreshInSeconds, cancellationToken: _timerToken.Token);
            _currentTimeOfSession = _currentTimeOfSession.Subtract(TimeSpan.FromSeconds(k_timerDurationRefreshInSeconds));
            
            UpdateActualViewAtTheTimer();

            var isSessionOfTimerCompleted = _currentTimeOfSession.TotalSeconds <= 0;
            if (isSessionOfTimerCompleted)
            {
                _beepSound.Play();
                onSessionOfTimerEnded?.Invoke();
                break;
            }
        }
    }

    public void SetTimeOfTimer(TimeSpan time)
    {
        _initiallySetTimeOfSession = time;
        _currentTimeOfSession = time;
        _pauseAndResumeTimerToggle.SetIsOnWithoutNotify(false);
    }
}