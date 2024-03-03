using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenController : MonoBehaviour
{
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
		int currentMinutesSetted = circularSlider.GetCurrentSettedMinutes();
		if(currentMinutesSetted > 0)
		{
			if(regularMode.isOn)
			{
				Debug.Log("Regular mode");
			} 
			else if(pomodoroMode.isOn)
			{
				Debug.Log("Pomodoro mode");
			} else
			{
				Debug.LogWarning("Error with setupping toggles!");
			}
		}
	}
}
