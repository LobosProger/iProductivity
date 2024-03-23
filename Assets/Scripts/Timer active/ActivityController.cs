using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityController : MonoBehaviour
{
	private int _currentCompletingActivityIndex = 0;
	private bool _isChosenPomodoroMode;

	private TimerController _timerController;
	private List<CompletingActivity> _completingActivities = new List<CompletingActivity>();
	
	private TimeSpan _timeActivityInPomodoroMode = TimeSpan.FromMinutes(1);
	private TimeSpan _timeBreakInPomodoroMode = TimeSpan.FromMinutes(2);

	private void Start()
	{
		_timerController = GetComponent<TimerController>();
		TimerController.OnTimerCompleted += OnTimerCompletedActivity;

		InitAndStartCompletingByPomodoroMode("Programming", TimeSpan.FromMinutes(2), TimeSpan.MinValue);
	}

	private void OnDestroy()
	{
		TimerController.OnTimerCompleted -= OnTimerCompletedActivity;
	}

	public void InitAndStartCompletingByPomodoroMode(string activityName, TimeSpan timeOfCompleting, TimeSpan breakTime)
	{
		_isChosenPomodoroMode = true;
		InitCompletingByPomodoroMode(activityName, timeOfCompleting, breakTime);

		SetTimeOnTimerAndLauchItForActivity();
	}

	public void InitCompletingByPomodoroMode(string activityName, TimeSpan timeOfCompleting, TimeSpan breakTime)
	{
		TimeSpan remainedAllocationTimeOfCompletion = timeOfCompleting;
		while(remainedAllocationTimeOfCompletion.TotalMinutes < _timeActivityInPomodoroMode.TotalMinutes)
		{
			CompletingActivity completingActivity = new CompletingActivity();
			completingActivity.SetActivityData(activityName, _timeActivityInPomodoroMode);
			_completingActivities.Add(completingActivity);

			remainedAllocationTimeOfCompletion = remainedAllocationTimeOfCompletion.Subtract(_timeActivityInPomodoroMode);
			CompletingActivity breakActivity = new CompletingActivity();
			completingActivity.SetActivityData("Break", _timeBreakInPomodoroMode);
			_completingActivities.Add(breakActivity);
		}

		if(remainedAllocationTimeOfCompletion.TotalMinutes > 0)
		{
			CompletingActivity completingActivity = new CompletingActivity();
			completingActivity.SetActivityData(activityName, remainedAllocationTimeOfCompletion);
			_completingActivities.Add(completingActivity);
		}

		_currentCompletingActivityIndex = 0;
	}

	private void OnTimerCompletedActivity()
	{
		if(_isChosenPomodoroMode)
		{
			if(IsLastCompletingActivity())
			{
				Debug.Log("Session completed!");
			} else
			{
				_currentCompletingActivityIndex++;
				SetTimeOnTimerAndLauchItForActivity();
			}
		}
	}

	private void SetTimeOnTimerAndLauchItForActivity()
	{
		TimeSpan currentTime = _completingActivities[_currentCompletingActivityIndex].GetRemainedTimeForCompletion();
		_timerController.SetTimeOfTimer(currentTime);
		_timerController.StartTimer();
	}

	private bool IsLastCompletingActivity()
	{
		return _currentCompletingActivityIndex >= _completingActivities.Count;
	}
}
