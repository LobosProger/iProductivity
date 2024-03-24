using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenController : MonoBehaviour
{
	[SerializeField] private string chosenActivity;
	[Space]
	[SerializeField] private ActivityController activityController;
	[Space]
    [SerializeField] private CircularSlider circularSlider;
	[SerializeField] private Toggle regularMode;
	[SerializeField] private Toggle pomodoroMode;
    [SerializeField] private ButtonManager startButton;
	
	private void Start()
	{
		startButton.onClick.AddListener(StartActivity);
	}

	private void StartActivity()
	{
		TimeSpan currentTimeSetted = circularSlider.GetCurrentSettedTime();
		if(currentTimeSetted.TotalMinutes > 0)
		{
			if(regularMode.isOn)
			{
				Debug.Log("Regular mode");
			} 
			else if(pomodoroMode.isOn)
			{
				ScreenManager.Singleton.ShowTimerCompletingActivityScreen();
				activityController.InitAndStartCompletingByPomodoroMode(chosenActivity, currentTimeSetted);
			} else
			{
				Debug.LogWarning("Error with setupping toggles!");
			}
		}
	}
}
