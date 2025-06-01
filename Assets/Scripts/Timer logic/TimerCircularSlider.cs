using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TimerCircularSlider : MonoBehaviour
{
	[SerializeField] private int minutesSteps = 30;
	[SerializeField] private TMP_Text sliderValueInMinutes;
	[SerializeField] private Image fillAmountLineOfSlider;
    [SerializeField] private UI_Knob _radialSlider;
	
    private int _currentMinutesSet = 0;

	private void Start()
	{
		_radialSlider.OnValueChanged.AddListener(OnSlidedHandlerOnSlider);

		ProcessSliderValuesAndShowToView();
	}
	
	private void OnSlidedHandlerOnSlider(float value)
	{
		ProcessSliderValuesAndShowToView();
	}

	private void ProcessSliderValuesAndShowToView()
	{
		var knobValue = _radialSlider.KnobValue;
		fillAmountLineOfSlider.fillAmount = knobValue;
		
		_currentMinutesSet = (int)(Mathf.Floor(knobValue * _radialSlider.SnapStepsPerLoop) * minutesSteps);
		sliderValueInMinutes.text = _currentMinutesSet.ToString();
	}

	public int GetCurrentMinutesSet()
	{
		return _currentMinutesSet;
	}
}
