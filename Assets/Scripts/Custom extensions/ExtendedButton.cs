using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ExtendedButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
	[Header("Properties")]
	[SerializeField] private ButtonTypeTransition buttonTypeTransition;
	[SerializeField] private float animationPeriodSeconds = 0.2f;
	[SerializeField] private bool isInteractable = true;
	[Space]
	[Header("Color properties")]
	[SerializeField] private Color normalColor;
	[SerializeField] private Color clickedColor;
	[SerializeField] private Color disabledColor;
	[Space]
	[Header("Canvas group components")]
	[SerializeField] private CanvasGroup normalCG;
	[SerializeField] private CanvasGroup clickedCG;
	[SerializeField] private CanvasGroup disabledCG;
	[Space]
	[Header("Image graphic component")]
	[SerializeField] private Image targetGraphic;
	[Space]
	[Header("Button transition check")]
	[SerializeField] ButtonTransitionCheck buttonTransitionCheck = ButtonTransitionCheck.None;
	[Space]
	[Header("Events")]
	public UnityEvent OnClickedButton;

	private enum ButtonTypeTransition { Color, CanvasGroup }
	private enum ButtonTransitionCheck { None, Normal, Clicked, Disabled }

	private bool previousInteractableValue;

#if UNITY_EDITOR
	private void Start()
	{
		previousInteractableValue = isInteractable;
	}
#endif

#if UNITY_EDITOR
	private void Update()
	{
		if (!Application.isPlaying)
		{
			if (buttonTransitionCheck != ButtonTransitionCheck.None)
			{
				switch (buttonTypeTransition)
				{
					case ButtonTypeTransition.Color: UpdateColorGraphicDueTransitionCheckInEditor(); break;
					case ButtonTypeTransition.CanvasGroup: UpdateCanvasGroupGraphicDueTransitionCheckInEditor(); break;
				}
			}
			else
			{
				UpdateGraphicsForInteractableModeInEditor();
			}
		}
		else
		{
			if (previousInteractableValue != isInteractable)
			{
				previousInteractableValue = isInteractable;
				SwitchInteractableState(isInteractable);
			}
		}
	}
#endif

	private void UpdateCanvasGroupGraphicDueTransitionCheckInEditor()
	{
		if (normalCG == null || clickedCG == null)
			return;

		switch (buttonTransitionCheck)
		{
			case ButtonTransitionCheck.Normal:
				normalCG.alpha = 1;
				clickedCG.alpha = 0;
				if (disabledCG != null)
					disabledCG.alpha = 0;
				break;

			case ButtonTransitionCheck.Clicked:
				normalCG.alpha = 0;
				clickedCG.alpha = 1;
				if (disabledCG != null)
					disabledCG.alpha = 0;
				break;

			case ButtonTransitionCheck.Disabled:
				normalCG.alpha = 0;
				clickedCG.alpha = 0;
				if (disabledCG != null)
					disabledCG.alpha = 1;
				break;
		}
	}

	private void UpdateColorGraphicDueTransitionCheckInEditor()
	{
		if (targetGraphic == null)
			return;

		targetGraphic.color = buttonTransitionCheck switch
		{
			ButtonTransitionCheck.Normal => normalColor,
			ButtonTransitionCheck.Clicked => clickedColor,
			ButtonTransitionCheck.Disabled => disabledColor,
			_ => throw new System.NotImplementedException()
		};
	}

	private void UpdateGraphicsForInteractableModeInEditor()
	{
		switch (buttonTypeTransition)
		{
			case ButtonTypeTransition.Color:
				if (targetGraphic == null)
					return;

				if (isInteractable)
				{
					targetGraphic.color = normalColor;
				}
				else
				{
					targetGraphic.color = disabledColor;
				}
				break;

			case ButtonTypeTransition.CanvasGroup:
				if (normalCG == null || clickedCG == null)
					return;

				if (isInteractable)
				{
					normalCG.alpha = 1;
					clickedCG.alpha = 0;
					if (disabledCG != null)
						disabledCG.alpha = 0;
				}
				else
				{
					normalCG.alpha = 0;
					clickedCG.alpha = 0;
					if (disabledCG != null)
						disabledCG.alpha = 1;
				}
				break;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		switch (buttonTypeTransition)
		{
			case ButtonTypeTransition.Color:
				if (isInteractable)
				{
					targetGraphic?.DOColor(clickedColor, animationPeriodSeconds);
				}

				break;

			case ButtonTypeTransition.CanvasGroup:
				if (isInteractable)
				{
					normalCG?.DOFade(0, animationPeriodSeconds);
					clickedCG?.DOFade(1, animationPeriodSeconds);
				}

				break;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		switch (buttonTypeTransition)
		{
			case ButtonTypeTransition.Color:
				if (isInteractable)
				{
					targetGraphic?.DOColor(normalColor, animationPeriodSeconds);
				}
					
				break;

			case ButtonTypeTransition.CanvasGroup:
				if (isInteractable)
				{
					clickedCG?.DOFade(0, animationPeriodSeconds);
					normalCG?.DOFade(1, animationPeriodSeconds);
				}

				break;
		}

		OnClickedButton?.Invoke();
	}

	public void SwitchInteractableState(bool isInteractable)
	{
		this.isInteractable = isInteractable;

		if (this.isInteractable)
		{
			switch (buttonTypeTransition)
			{
				case ButtonTypeTransition.Color:
					targetGraphic?.DOColor(normalColor, animationPeriodSeconds);
					break;

				case ButtonTypeTransition.CanvasGroup:
					disabledCG?.DOFade(0, animationPeriodSeconds);
					normalCG?.DOFade(1, animationPeriodSeconds);
					break;
			}
		}
		else
		{
			switch (buttonTypeTransition)
			{
				case ButtonTypeTransition.Color:
					targetGraphic?.DOColor(disabledColor, animationPeriodSeconds);
					break;

				case ButtonTypeTransition.CanvasGroup:
					disabledCG?.DOFade(1, animationPeriodSeconds);
					normalCG?.DOFade(0, animationPeriodSeconds);
					break;
			}
		}
	}
}