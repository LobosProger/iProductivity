using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

[ExecuteInEditMode]
public class ExtendedToggle : MonoBehaviour
{
	[Header("Properties")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private float fadePeriodSeconds = 0.2f;
	[Space]
	[Header("Components")]
	[SerializeField] private CanvasGroup turnedOnCG;
    [SerializeField] private CanvasGroup turnedOffCG;
	[Space]
	[Header("Events")]
	[SerializeField] private UnityEvent onTurnedOn;
	[SerializeField] private UnityEvent onTurnedOff;
	[SerializeField] private UnityEvent<bool> onSwitchedToggle;

#if UNITY_EDITOR
	private void Update()
	{
		if(!Application.isPlaying)
		{
			UpdateGraphicOfToggle();
		}
	}
#endif

	private void UpdateGraphicOfToggle()
    {
		if (turnedOnCG == null || turnedOffCG == null)
			return;

		if (isOn)
		{
            if(!Application.isPlaying)
            {
				turnedOnCG.alpha = 1f;
				turnedOffCG.alpha = 0f;
			} else
            {
                turnedOnCG.DOFade(1, fadePeriodSeconds);
                turnedOffCG.DOFade(0, fadePeriodSeconds);
			}
		}
		else
		{
			if (!Application.isPlaying)
			{
				turnedOffCG.alpha = 1;
				turnedOnCG.alpha = 0;
			}
			else
			{
				turnedOffCG.DOFade(1, fadePeriodSeconds);
				turnedOnCG.DOFade(0, fadePeriodSeconds);
			}
		}
	}

	private void InvokeEvents()
	{
		if(isOn)
		{
			onTurnedOn?.Invoke();
		} else
		{
			onTurnedOff?.Invoke();
		}

		onSwitchedToggle?.Invoke(isOn);
	}

	public void SetToggleValue(bool value)
	{
		if (isOn == value)
			return;

		isOn = value;
		InvokeEvents();
		UpdateGraphicOfToggle();
	}

	public void SwitchToggleValue()
	{
		isOn = !isOn;
		InvokeEvents();
		UpdateGraphicOfToggle();
	}

	public bool IsOn => isOn;
}