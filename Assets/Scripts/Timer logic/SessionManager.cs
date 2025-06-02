using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ActivityType
{
    Work,
    Break
}

public class SessionManager : MonoBehaviour
{
    [Header("Windows")] 
    [SerializeField] private WindowView _timerWindow;
    [SerializeField] private WindowView _pomodoroSessionWindow;
    [Space]
    [SerializeField] private TimerCircularSlider _timerCircularSlider;
    [SerializeField] private TimerController _timerController;
    [SerializeField] private TMP_Text _currentActivityNameText;
    [Space] 
    [Header("Main timer window components")]
    [SerializeField] private Button _selectedActivityButton;
    [SerializeField] private TMP_Text _selectedActivityText;
    [SerializeField] private Button _startActivityButton;

    private string _chosenActivityName;
    private string _chosenActivityHash;
    private string _currentServerActivityHash; // Track current activity on server
    
    private ActivityType _currentActivityType = ActivityType.Work;
    private int _totalSessionMinutes;
    private int _completedCycles;
    private int _totalCycles;

    private void Start()
    {
        ActivitiesSetter.onSelectedActivity += SelectActivityAndRefreshView;
        
        _timerController.onSessionOfTimerStarted += CaptureAtServerLaunchedActivity;
        _timerController.onSessionOfTimerEnded += CaptureAtServerCompletedActivityAndSwitchToNext;
        _timerController.onTimerStopped += OnTimerStopped;
        _timerController.onTimerPaused += CaptureAtServerCompletedActivity;
        _timerController.onTimerResumed += CaptureAtServerLaunchedActivity;
        
        _startActivityButton.onClick.AddListener(StartSessionActivity);
        _selectedActivityButton.onClick.AddListener(ReselectActivityInActivitiesWindow);
    }

    private void OnDestroy()
    {
        ActivitiesSetter.onSelectedActivity -= SelectActivityAndRefreshView;
        
        _timerController.onSessionOfTimerStarted -= CaptureAtServerLaunchedActivity;
        _timerController.onSessionOfTimerEnded -= CaptureAtServerCompletedActivityAndSwitchToNext;
        _timerController.onTimerStopped -= OnTimerStopped;
        _timerController.onTimerPaused -= CaptureAtServerCompletedActivity;
        _timerController.onTimerResumed -= CaptureAtServerLaunchedActivity;
    }

    private void ReselectActivityInActivitiesWindow()
    {
        TopDownPanel.instance.ShowActivitiesWindow();
    }

    private void SelectActivityAndRefreshView(string activityName)
    {
        _chosenActivityName = activityName;
        _selectedActivityText.text = activityName;
        
        // Find the activity hash
        var activity = ActivityManager.instance.FindActivityByName(activityName);
        if (activity != null)
        {
            _chosenActivityHash = activity.hash;
        }
    }
    
    private async void CaptureAtServerLaunchedActivity()
    {
        if (_currentActivityType == ActivityType.Work && !string.IsNullOrEmpty(_chosenActivityHash))
        {
            var success = await ActivityManager.instance.StartActivityAsync(_chosenActivityHash);
            if (success)
            {
                _currentServerActivityHash = _chosenActivityHash;
                Debug.Log($"Session started on server: {_chosenActivityName}");
            }
        }
        else if (_currentActivityType == ActivityType.Break)
        {
            Debug.Log("Break session started (not tracked on server)");
        }
    }
    
    private async void CaptureAtServerCompletedActivityAndSwitchToNext()
    {
        await CaptureAtServerCompletedActivityAsync();
        
        // Switch to next activity or complete session
        SwitchToNextActivityOrCompleteSession();
    }

    private void CaptureAtServerCompletedActivity()
    {
        CaptureAtServerCompletedActivityAsync().Forget();
    }

    private async UniTask CaptureAtServerCompletedActivityAsync()
    {
        if (_currentActivityType == ActivityType.Work && !string.IsNullOrEmpty(_currentServerActivityHash))
        {
            await ActivityManager.instance.EndActivityAsync(_currentServerActivityHash);
            Debug.Log($"Session ended on server: {_chosenActivityName}");
            _currentServerActivityHash = null;
        }
        else if (_currentActivityType == ActivityType.Break)
        {
            Debug.Log("Break session ended (not tracked on server)");
        }
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
        _pomodoroSessionWindow.ClosePreviousAndShowThisWindow();
        
        _currentActivityNameText.text = _currentActivityType == ActivityType.Work ? _chosenActivityName : "Rest";
        Debug.Log($"Started {_currentActivityType} activity for {activityDurationMinutes} minutes");
    }
    
    private async void OnTimerStopped()
    {
        Debug.Log("Timer stopped by user");
        CloseTimerWindowAndBackToTheMainMenu();

        await CaptureAtServerCompletedActivityAsync();
        ResetActivitiesStats();
    }

    private void ResetActivitiesStats()
    {
        // Reset session state
        _currentActivityType = ActivityType.Work;
        _completedCycles = 0;
        _totalCycles = 0;
        _currentServerActivityHash = null;
    }

    private void StartSessionActivity()
    {
        if (string.IsNullOrEmpty(_chosenActivityName))
        {
            Debug.LogWarning("Please select an activity first");
            return;
        }
        
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
        
        TopDownPanel.instance.HidePanel();
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
        TopDownPanel.instance.ShowPanel();
        _timerWindow.ClosePreviousAndShowThisWindow();
    }
}