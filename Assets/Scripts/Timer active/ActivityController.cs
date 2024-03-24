using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityController : MonoBehaviour
{
	private int _currentCompletingActivityIndex = 0;
	private bool _isChosenPomodoroMode;

	private ActivityView _activityView;
	private TimerController _timerController;
	private List<CompletingActivity> _completingActivities = new List<CompletingActivity>();
	
	private TimeSpan _timeActivityInPomodoroMode = TimeSpan.FromMinutes(1);
	private TimeSpan _timeBreakInPomodoroMode = TimeSpan.FromSeconds(10);

	private void Start()
	{
		_timerController = GetComponent<TimerController>();
		_activityView = GetComponent<ActivityView>();
		TimerController.OnTimerCompleted += OnTimerCompletedActivity;
	}

	private void OnDestroy()
	{
		TimerController.OnTimerCompleted -= OnTimerCompletedActivity;
	}

	public void InitAndStartCompletingByPomodoroMode(string activityName, TimeSpan timeOfCompleting)
	{
		_isChosenPomodoroMode = true;

		TimeSpan remainedAllocationTimeOfCompletion = timeOfCompleting;
		while (remainedAllocationTimeOfCompletion.TotalMinutes > _timeActivityInPomodoroMode.TotalMinutes)
		{
			InsertIntoListOfActivitiesCompletingActitvity(activityName, _timeActivityInPomodoroMode);
			InsertIntoListOfActivitiesCompletingActitvity("Break", _timeBreakInPomodoroMode);

			remainedAllocationTimeOfCompletion = remainedAllocationTimeOfCompletion.Subtract(_timeActivityInPomodoroMode);
		}

		if (remainedAllocationTimeOfCompletion.TotalMinutes > 0)
		{
			InsertIntoListOfActivitiesCompletingActitvity(activityName, remainedAllocationTimeOfCompletion);
		}

		_currentCompletingActivityIndex = 0;
		SetTimeOnTimerAndLauchItForActivity();
	}

	private void SetTimeOnTimerAndLauchItForActivity()
	{
		TimeSpan currentTime = _completingActivities[_currentCompletingActivityIndex].GetRemainedTimeForCompletion();
		string currentActivity = _completingActivities[_currentCompletingActivityIndex].GetNameOfActivity();
		_activityView.ShowOnTimerCurrentInProgressActivity(currentActivity);
		_timerController.SetTimeOfTimer(currentTime);
		_timerController.StartTimer();
	}

	private void InsertIntoListOfActivitiesCompletingActitvity(string nameOfActivity, TimeSpan timeOnActivity)
	{
		CompletingActivity completingActivity = new CompletingActivity();
		completingActivity.SetActivityData(nameOfActivity, timeOnActivity);
		_completingActivities.Add(completingActivity);
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

	private bool IsLastCompletingActivity()
	{
		return _currentCompletingActivityIndex >= _completingActivities.Count - 1;
	}
}
