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

	private void Start()
	{
		_radialSlider = GetComponent<RadialSlider>();
		_radialSlider.onValueChanged.AddListener(OnSlidedHandlerOnSlider);
	}

	private void OnSlidedHandlerOnSlider(float value)
	{
		sliderValue.text = (Mathf.Floor(value) * minutesInterval).ToString();
	}
}
