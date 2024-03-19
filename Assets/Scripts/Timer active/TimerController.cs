using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	public int seconds;

	private TimeSpan _maxSettedSeconds;
	private TimeSpan _currentRemainedSeconds;
	
	private TimerView _timerView;
	private CancellationTokenSource _timerToken;

	private async UniTask Start()
	{
		_timerView = GetComponent<TimerView>();
		Application.runInBackground = true;

		await UniTask.WaitUntil(() => AlarmSetter.Instance.IsInit);

		TimeSpan time = TimeSpan.FromSeconds(seconds);
		SetTimeOfTimer(time);
		StartTimer();
	}

	private async UniTask StartTimerCompletion(CancellationToken ct)
	{
		_timerView.ShowRemainedTime((int)_currentRemainedSeconds.TotalSeconds, (int)_maxSettedSeconds.TotalSeconds);

		while (true)
		{
			await UniTask.Delay(1000, cancellationToken: ct);

			_currentRemainedSeconds.Subtract(TimeSpan.FromSeconds(1));
			_timerView.ShowRemainedTime((int)_currentRemainedSeconds.TotalSeconds, (int)_maxSettedSeconds.TotalSeconds);

			if ((int)_currentRemainedSeconds.TotalSeconds <= 0)
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
		if (_currentRemainedSeconds.TotalSeconds > 0)
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
		_maxSettedSeconds = settedTime;
		_currentRemainedSeconds = _maxSettedSeconds;
	}
}