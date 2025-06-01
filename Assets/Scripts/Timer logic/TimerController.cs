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
    [Space]
    [Header("Timer buttons")]
    [SerializeField] private Button _stopTimerButton;
    [SerializeField] private ExtendedToggle _pauseAndResumeTimerToggle;
    
    public event Action onSessionOfTimerStarted;
    public event Action onSessionOfTimerEnded;
    public event Action onTimerStopped;

    private bool _isPauseButtonPressed;
    private TimeSpan _timeOfSession;
    private CancellationTokenSource _timerToken;

    private const float k_timerDurationRefreshInSeconds = 1;

    private void Start()
    {
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
        
        _timeOfSession = TimeSpan.Zero;
        UpdateActualViewAtTheTimer();
        _pauseAndResumeTimerToggle.SetIsOnWithoutNotify(false);
        
        _timerToken?.Cancel();

        // If already clicked on the pause button, we don't need to second time invoke onSessionOfTimerEnded action, on which subscribed SessionManager
        // because it's need for handle logic of start and end of the session activity (for example, programming, reading, etc.) and it will be sent
        // into backend as event
        if (!_isPauseButtonPressed)
            onSessionOfTimerEnded?.Invoke();
        
        onTimerStopped?.Invoke();
    }

    private bool IsRemainedTimeAtTheTimer()
    {
        return _timeOfSession.Subtract(TimeSpan.FromSeconds(1)).TotalSeconds > 0;
    }
    
    private void UpdateActualViewAtTheTimer()
    {
        _remainingTimeMinutesText.text = _timeOfSession.ToString("mm");
        _remainingTimeSecondsText.text = _timeOfSession.ToString("ss");
    }

    private void SwitchTimerState(bool pauseTimer)
    {
        if (!IsRemainedTimeAtTheTimer())
            return;
        
        if (pauseTimer)
        {
            onSessionOfTimerEnded?.Invoke();
            _timerToken?.Cancel();
        }
        else
        {
            LaunchSessionOfTimer();
        }
    }
    
    public void LaunchSessionOfTimer()
    {
        onSessionOfTimerStarted?.Invoke();
        
        _pauseAndResumeTimerToggle.SetIsOnWithoutNotify(false);
        
        _isPauseButtonPressed = false;
        _timerToken = new();
        LaunchTimerAsync().Forget();
    }

    private async UniTask LaunchTimerAsync()
    {
        while (true)
        {
            UpdateActualViewAtTheTimer();

            await UniTask.WaitForSeconds(k_timerDurationRefreshInSeconds, cancellationToken: _timerToken.Token);
            _timeOfSession = _timeOfSession.Subtract(TimeSpan.FromSeconds(k_timerDurationRefreshInSeconds));
            
            UpdateActualViewAtTheTimer();

            var isSessionOfTimerCompleted = _timeOfSession.TotalSeconds <= 0;
            if (isSessionOfTimerCompleted)
            {
                onSessionOfTimerEnded?.Invoke();
                break;
            }
        }
    }

    public void SetTimeOfTimer(TimeSpan time)
    {
        _timeOfSession = time;
        _pauseAndResumeTimerToggle.SetIsOnWithoutNotify(false);
    }
}