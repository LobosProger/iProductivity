using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class WindowController : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] private int currentActiveWindow;
	[SerializeField] private float transitionAnimationSeconds = 0.2f;
	[Space]
	[Header("Window components")]
	[SerializeField] private List<Window> windowses = new List<Window>();

	[System.Serializable]
	private class Window
	{
		public string windowName;
		public CanvasGroup canvasGroup;
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (!Application.isPlaying)
		{
			ShowWindowByIndex(currentActiveWindow);
		}
	}
#endif

	public void ShowWindowByIndex(int index)
	{
		if (index >= windowses.Count || index < 0)
			return;

		for (int i = 0; i < windowses.Count; i++)
		{
			if (i == index)
			{
				if (windowses[i].canvasGroup != null)
				{
					if (Application.isPlaying)
						windowses[i].canvasGroup.DOFade(1, transitionAnimationSeconds);
					else
						windowses[i].canvasGroup.alpha = 1;
					
					windowses[i].canvasGroup.blocksRaycasts = true;
				}
			}
			else
			{
				if (windowses[i].canvasGroup != null)
				{
					if (Application.isPlaying)
						windowses[i].canvasGroup.DOFade(0, transitionAnimationSeconds);
					else
						windowses[i].canvasGroup.alpha = 0;
					
					windowses[i].canvasGroup.blocksRaycasts = false;
				}
			}
		}
	}

	public void ShowWindowByName(string nameOfWindow)
	{
		try
		{
			Window retrievingWindow = windowses.Where(window => window.windowName == nameOfWindow).First();
			int retrievingWindowIndex = windowses.IndexOf(retrievingWindow);

			if (retrievingWindow != null)
			{
				for (int i = 0; i < windowses.Count; i++)
				{
					if (retrievingWindowIndex == i)
					{
						windowses[i].canvasGroup.DOFade(1, transitionAnimationSeconds);
						windowses[i].canvasGroup.blocksRaycasts = true;
					}
					else
					{
						if (windowses[i].canvasGroup != null)
						{
							windowses[i].canvasGroup.DOFade(0, transitionAnimationSeconds);
							windowses[i].canvasGroup.blocksRaycasts = false;
						}
					}
				}
			}
		} catch
		{
			Debug.LogWarning($"Invalid window name! Name: {nameOfWindow}");
		}
	}
}
