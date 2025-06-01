using System;
using UnityEngine;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    [SerializeField] private TimerCircularSlider _timerCircularSlider;
    [SerializeField] private TimerController _timerController;
    [Space] 
    [SerializeField] private Button _startActivityButton;

    private void Start()
    {
        _timerController = GetComponent<TimerController>();
        
        _timerController.onSessionOfTimerStarted += StartSessionAndCaptureAtServerActivity;
        _timerController.onSessionOfTimerEnded += EndSessionAndCaptureAtServerActivity;
        
        _timerController.onTimerStopped += EndSessionAndCaptureAtServerActivity;
    }

    private void OnDestroy()
    {
        _timerController.onSessionOfTimerStarted -= StartSessionAndCaptureAtServerActivity;
        _timerController.onSessionOfTimerEnded -= EndSessionAndCaptureAtServerActivity;
        
        _timerController.onTimerStopped -= EndSessionAndCaptureAtServerActivity;
    }

    private void StartSessionActivity()
    {
        var setMinutesOnTheSlider = _timerCircularSlider.GetCurrentMinutesSet();
        
        //_timerController.SetTimeOfTimer();
    }

    private void StartSessionAndCaptureAtServerActivity()
    {
        
    }
    
    private void EndSessionAndCaptureAtServerActivity()
    {
        
    }

    private void CloseTimerWindowAndBackToTheMainMenu()
    {
        
    }
}
