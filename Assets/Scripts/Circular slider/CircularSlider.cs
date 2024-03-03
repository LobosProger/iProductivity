using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircularSlider : MonoBehaviour
{
	[SerializeField] private int minutesInterval = 20;
	[SerializeField] private TMP_Text sliderValue;

    private RadialSlider _radialSlider;
	private int _currentMinutesSetted = 0;

	private void Start()
	{
		_radialSlider = GetComponent<RadialSlider>();
		_radialSlider.onValueChanged.AddListener(OnSlidedHandlerOnSlider);

		float currentSliderValue = _radialSlider.currentValue;
		sliderValue.text = (Mathf.Floor(currentSliderValue) * minutesInterval).ToString();
	}

	private void OnSlidedHandlerOnSlider(float value)
	{
		_currentMinutesSetted = (int)(Mathf.Floor(value) * minutesInterval);
		sliderValue.text = _currentMinutesSetted.ToString();
	}

	public int GetCurrentSettedMinutes()
	{
		return _currentMinutesSetted;
	}
}
