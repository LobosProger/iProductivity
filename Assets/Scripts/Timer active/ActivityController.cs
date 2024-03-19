using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityController : MonoBehaviour
{
	[SerializeField] private int minutesOnActivity;
	[SerializeField] private string activityName;

	private int _currentCompletingActivityIndex = 0;
    private List<CompletingActivity> _completingActivities = new List<CompletingActivity>();

	private TimeSpan _timeActivityInPomodoroMode = TimeSpan.FromMinutes(25);
	private TimeSpan _timeBreakInPomodoroMode = TimeSpan.FromMinutes(5);

	private void Start()
	{
		
	}

	public void StartCompletingByRegularMode(string activityName, TimeSpan timeOfCompleting)
	{

	}

	public void StartCompletingByPomodoroMode(string activityName, TimeSpan timeOfCompleting, int breakTimeInSeconds)
	{
		TimeSpan remainedAllocationTimeOfCompletion = timeOfCompleting;
		while(remainedAllocationTimeOfCompletion.TotalMinutes < _timeActivityInPomodoroMode.TotalMinutes)
		{
			CompletingActivity completingActivity = new CompletingActivity();
			completingActivity.SetActivityData(activityName, _timeActivityInPomodoroMode);
			_completingActivities.Add(completingActivity);

			remainedAllocationTimeOfCompletion.Subtract(_timeActivityInPomodoroMode);
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
}
