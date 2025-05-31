using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class CircularSlider : MonoBehaviour
{
	[SerializeField] private int minutesInterval = 20;
	[SerializeField] private TMP_Text sliderValueInMinutes;
	[SerializeField] private Image fillAmountLineOfSlider;
    [SerializeField] private UI_Knob _radialSlider;
	
    private int _currentMinutesSet = 0;

    private const float k_epsilonValue = 0.01f;

	private void Start()
	{
		_radialSlider.OnValueChanged.AddListener(OnSlidedHandlerOnSlider);

		var currentSliderValue = CheckOnMaxValueAndSetKnobToZeroValue();
		fillAmountLineOfSlider.fillAmount = currentSliderValue;
		sliderValueInMinutes.text = ((int)(Mathf.Floor(currentSliderValue * _radialSlider.SnapStepsPerLoop) * minutesInterval)).ToString();
	}

	private void OnSlidedHandlerOnSlider(float value)
	{
		var knobValue = CheckOnMaxValueAndSetKnobToZeroValue();
		
		_currentMinutesSet = (int)(Mathf.Floor(knobValue * _radialSlider.SnapStepsPerLoop) * minutesInterval);
		fillAmountLineOfSlider.fillAmount = knobValue;
		sliderValueInMinutes.text = _currentMinutesSet.ToString();
	}

	private float CheckOnMaxValueAndSetKnobToZeroValue()
	{
		// var isKnobValueLikeInOneInterval = 1 - _radialSlider.KnobValue < k_epsilonValue;
		// _radialSlider.KnobValue = isKnobValueLikeInOneInterval ? 0 : _radialSlider.KnobValue;
		
		return _radialSlider.KnobValue;
	}

	public TimeSpan GetCurrentSettedTime()
	{
		return TimeSpan.FromMinutes(_currentMinutesSet);
	}
}
