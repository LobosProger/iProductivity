using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerView : MonoBehaviour
{
	[SerializeField] private Image progressBarTimer;
	[SerializeField] private TMP_Text minutesText;
	[SerializeField] private TMP_Text secondsText;

	public int testSeconds;
	public int maxTestSeconds;

	private void Update()
	{
		ShowRemainedTime(testSeconds, maxTestSeconds);
	}

	public void ShowRemainedTime(int currentRemainedSeconds, int maxSettedSeconds)
	{
		float fillAmountForProgressBarTimer = ((float)currentRemainedSeconds / maxSettedSeconds);
		progressBarTimer.fillAmount = fillAmountForProgressBarTimer;

		TimeSpan formattedTime = TimeSpan.FromSeconds(currentRemainedSeconds);
		if(formattedTime.TotalMinutes > 59)
		{
			minutesText.text = Mathf.Floor((float)formattedTime.TotalMinutes).ToString();
		} else
		{
			minutesText.text = formattedTime.ToString("mm");
		}
		secondsText.text = formattedTime.ToString("ss");
	}
}
