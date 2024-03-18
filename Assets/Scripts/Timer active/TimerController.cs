using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	public int seconds;

	private int _maxSettedSeconds;
	private int _currentRemainedSeconds;
	
	private TimerView _timerView;
	private CancellationTokenSource _timerToken;

	private async UniTask Start()
	{
		_timerView = GetComponent<TimerView>();
		Application.runInBackground = true;

		await UniTask.WaitUntil(() => AlarmSetter.Instance.IsInit);

		TimeSpan time = TimeSpan.FromSeconds(seconds);
		AlarmSetter.Instance.SetNewAlarmNotification("Stop!", "Activity stop!", (int)time.TotalSeconds);
		SetTimeOfTimer(time);
		StartTimer();
	}

	private async UniTask StartTimerCompletion(CancellationToken ct)
	{
		_timerView.ShowRemainedTime(_currentRemainedSeconds, _maxSettedSeconds);

		while (true)
		{
			await UniTask.Delay(1000, cancellationToken: ct);

			_currentRemainedSeconds--;
			_timerView.ShowRemainedTime(_currentRemainedSeconds, _maxSettedSeconds);

			if (_currentRemainedSeconds <= 0)
			{
				OnTimerCompleted();
				break;
			}

			if (ct.IsCancellationRequested)
			{
				break;
			}
		}
	}

	private void OnTimerCompleted()
	{
		GetComponent<AudioSource>().Play();
		Debug.Log("Completed!");
	}

	public void StartTimer()
	{
		_timerToken = new CancellationTokenSource();
		if (_currentRemainedSeconds > 0)
		{
			StartTimerCompletion(_timerToken.Token);
		}
	}

	public void PauseTimer()
	{
		_timerToken.Cancel();
	}

	public void SetTimeOfTimer(TimeSpan settedTime)
	{
		_maxSettedSeconds = (int)settedTime.TotalSeconds;
		_currentRemainedSeconds = _maxSettedSeconds;
	}
}