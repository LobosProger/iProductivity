using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private WindowController timerWindowsesController;

	public static ScreenManager Singleton { get; private set; }

	// Main screen windowses
	private const string _timerSettingScreenName = "Setting timer screen";
	private const string _timerScreenName = "Timer screen";

	private void Awake()
	{
		Singleton = this;
	}

	public void ShowSettingTimerScreen()
	{
		timerWindowsesController.ShowWindowByName(_timerSettingScreenName);
	}

	public void ShowTimerCompletingActivityScreen()
	{
		timerWindowsesController.ShowWindowByName(_timerScreenName);
	}
}
