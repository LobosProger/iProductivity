using System;
using UnityEngine;
using UnityEngine.UI;

public enum ActivityType
{
    Work,
    Break
}

public class SessionManager : MonoBehaviour
{
    [SerializeField] private TimerCircularSlider _timerCircularSlider;
    [SerializeField] private TimerController _timerController;
    [Space] 
    [SerializeField] private Button _startActivityButton;

    private ActivityType _currentActivityType = ActivityType.Work;
    private int _totalSessionMinutes;
    private int _completedCycles;
    private int _totalCycles;

    public event Action<ActivityType> onActivityTypeChanged;
    public event Action<int, int> onCycleProgressChanged; // current cycle, total cycles

    private void Start()
    {
        _timerController = GetComponent<TimerController>();
        
        _timerController.onSessionOfTimerStarted += CaptureAtServerLaunchedActivity;
        _timerController.onSessionOfTimerEnded += CaptureAtServerCompletedActivityAndSwitchToNext;
        
        _timerController.onTimerStopped += OnTimerStopped;
        _timerController.onTimerPaused += CaptureAtServerCompletedActivity;
        _timerController.onTimerResumed += CaptureAtServerLaunchedActivity;
        
        _startActivityButton.onClick.AddListener(StartSessionActivity);
    }

    private void OnDestroy()
    {
        _timerController.onSessionOfTimerStarted -= CaptureAtServerLaunchedActivity;
        _timerController.onSessionOfTimerEnded -= CaptureAtServerCompletedActivityAndSwitchToNext;
        
        _timerController.onTimerStopped -= OnTimerStopped;
        _timerController.onTimerPaused -= CaptureAtServerCompletedActivity;
        _timerController.onTimerResumed -= CaptureAtServerLaunchedActivity;
    }
    
    private void CaptureAtServerLaunchedActivity()
    {
        var activityName = _currentActivityType == ActivityType.Work ? "Programming" : "Rest";
        Debug.Log($"Session started: {activityName}");
    }
    
    private void CaptureAtServerCompletedActivityAndSwitchToNext()
    {
        CaptureAtServerCompletedActivity();
        
        // Here you can send analytics events to server
        // AnalyticsManager.SendEvent("end_activity", activityName);
        
        // Switch to next activity or complete session
        SwitchToNextActivityOrCompleteSession();
    }

    private void CaptureAtServerCompletedActivity()
    {
        var activityName = _currentActivityType == ActivityType.Work ? "Programming" : "Rest";
        Debug.Log($"Session ended: {activityName}");
    }
    
    private void SwitchToNextActivityOrCompleteSession()
    {
        if (_currentActivityType == ActivityType.Work)
        {
            // Switch to break
            _currentActivityType = ActivityType.Break;
            
            // Check if we have time for break
            var breakDuration = GetCurrentActivityDuration();
            if (breakDuration > 0)
            {
                StartCurrentActivity();
                return;
            }
        }
        else
        {
            // Completed one full cycle (work + break)
            _completedCycles++;
            _currentActivityType = ActivityType.Work;
        }
        
        // Check if session is completed
        if (_completedCycles >= _totalCycles)
        {
            CompleteSession();
            return;
        }
        
        // Continue with next work interval
        StartCurrentActivity();
    }
    
    private int GetCurrentActivityDuration()
    {
        if (_currentActivityType == ActivityType.Work)
        {
            return SessionConfig.k_workIntervalMinutes;
        }
        
        // For break, check if this is the last cycle
        var isLastCycle = (_completedCycles + 1) >= _totalCycles;
        if (isLastCycle)
        {
            // Calculate remaining time for the last break
            var totalUsedMinutes = _completedCycles * SessionConfig.k_cycleDurationMinutes + SessionConfig.k_workIntervalMinutes;
            var remainingMinutes = _totalSessionMinutes - totalUsedMinutes;
            return Mathf.Max(0, remainingMinutes);
        }
        
        return SessionConfig.k_breakIntervalMinutes;
    }
    
    private void StartCurrentActivity()
    {
        var activityDurationMinutes = GetCurrentActivityDuration();
        var timeSpan = new TimeSpan(hours: 0, minutes: activityDurationMinutes, seconds: 0);
        
        _timerController.SetTimeOfTimer(timeSpan);
        _timerController.LaunchSessionOfTimer();
        
        onActivityTypeChanged?.Invoke(_currentActivityType);
        onCycleProgressChanged?.Invoke(_completedCycles + 1, _totalCycles);
        
        Debug.Log($"Started {_currentActivityType} activity for {activityDurationMinutes} minutes");
    }
    
    private void OnTimerStopped()
    {
        Debug.Log("Timer stopped by user");

        CaptureAtServerCompletedActivity();
        ResetActivitiesStats();
        CloseTimerWindowAndBackToTheMainMenu();
    }

    private void ResetActivitiesStats()
    {
        // Reset session state
        _currentActivityType = ActivityType.Work;
        _completedCycles = 0;
        _totalCycles = 0;
    }

    private void StartSessionActivity()
    {
        _totalSessionMinutes = _timerCircularSlider.GetCurrentMinutesSet();
        
        if (_totalSessionMinutes <= 0)
        {
            Debug.LogWarning("Session time should be greater than 0");
            return;
        }
        
        // Calculate total cycles based on selected time
        _totalCycles = Mathf.CeilToInt((float)_totalSessionMinutes / SessionConfig.k_cycleDurationMinutes);
        _completedCycles = 0;
        _currentActivityType = ActivityType.Work;
        
        Debug.Log($"Starting session: {_totalSessionMinutes} minutes, {_totalCycles} cycles");
        
        StartCurrentActivity();
    }

    private void CompleteSession()
    {
        Debug.Log("Session completed successfully!");
        
        ResetActivitiesStats();
        
        // Here you can navigate back to main menu or show completion screen
        CloseTimerWindowAndBackToTheMainMenu();
    }

    private void CloseTimerWindowAndBackToTheMainMenu()
    {
        // Implementation depends on your window system
        // For example:
        // MainMenuWindow.Show();
        // TimerWindow.Hide();
    }
}