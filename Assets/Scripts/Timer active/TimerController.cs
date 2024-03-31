using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	public TimeSpan _maxSettedSeconds;
	public TimeSpan _currentRemainedSeconds;

	private bool _isTimerCompleting;
	private bool _isTimerPaused;
	private TimerView _timerView;
	private CancellationTokenSource _timerToken;

	public static Action OnTimerPaused;
	public static Action OnTimerResumed;
	public static Action OnTimerCompleted;

	private void Awake()
	{
		_timerView = GetComponent<TimerView>();
		Application.runInBackground = true;
	}

	private async UniTask StartTimerCompletion(CancellationToken ct)
	{
		_timerView.ShowRemainedTime((int)_currentRemainedSeconds.TotalSeconds, (int)_maxSettedSeconds.TotalSeconds);

		while (true)
		{
			await UniTask.Delay(1000, cancellationToken: ct);

			_currentRemainedSeconds = _currentRemainedSeconds.Subtract(TimeSpan.FromSeconds(1));
			_timerView.ShowRemainedTime((int)_currentRemainedSeconds.TotalSeconds, (int)_maxSettedSeconds.TotalSeconds);

			if ((int)_currentRemainedSeconds.TotalSeconds <= 0)
			{
				CompleteTimer();
				break;
			}

			if (ct.IsCancellationRequested)
			{
				break;
			}
		}
	}

	private void CompleteTimer()
	{
		GetComponent<AudioSource>().Play();
		OnTimerCompleted?.Invoke();
	}

	public void StartTimer(string notificationTitle, string notificationDescription)
	{
		_timerToken = new CancellationTokenSource();
		if (_currentRemainedSeconds.TotalSeconds > 0)
		{
			_isTimerCompleting = true;
			StartTimerCompletion(_timerToken.Token);
			
			if(_isTimerPaused)
			{
				OnTimerResumed?.Invoke();
				_isTimerPaused = false;
			}
		}
	}

	public void PauseTimer()
	{
		_isTimerPaused = true;
		_timerToken.Cancel();
		OnTimerPaused?.Invoke();
		AlarmSetter.Instance.ClearAllAlarmNotifications();
	}

	public void SetTimeOfTimer(TimeSpan settedTime)
	{
		_maxSettedSeconds = settedTime;
		_currentRemainedSeconds = _maxSettedSeconds;
	}

	public TimeSpan GetRemainedTimeOnTimer()
	{
		return _currentRemainedSeconds;
	}
}