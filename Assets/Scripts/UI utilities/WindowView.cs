using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[DefaultExecutionOrder(-50)]
public class WindowView : IWindowView
{
	[HideInInspector] public CanvasGroup canvasGroup;
	
	public Action onStartOpeningWindow;
	public Action onFinishedClosingWindow;
	
	private static Stack<WindowView> _previousOpenedWindows = new();
	
	private const float k_transitionDuration = 0.4f;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();

		onStartOpeningWindow += OnStartOpeningWindowHandler;
		onFinishedClosingWindow += OnClosedFullyWindowHandler;

		if (IsWindowOpened())
		{
			_previousOpenedWindows.Push(this);
		}
	}

	private void OnDestroy()
	{
		onStartOpeningWindow -= OnStartOpeningWindowHandler;
		onFinishedClosingWindow -= OnClosedFullyWindowHandler;
	}

#if UNITY_EDITOR
	public static void CloseAllWindowsEditorMethod()
	{
		var windows = FindObjectsByType<WindowView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		foreach (var eachWindow in windows)
		{
			eachWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
			eachWindow.GetComponent<CanvasGroup>().alpha = 0;
		}
	}
	
	[ContextMenu("CloseAllWindows")]
	private void CloseAllWindows()
	{
		var windows = FindObjectsByType<WindowView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		foreach (var eachWindow in windows)
		{
			eachWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
			eachWindow.GetComponent<CanvasGroup>().alpha = 0;
		}
	}
	
	[ContextMenu("ShowOnlyThisWindow")]
	private void ShowOnlyThisWindow()
	{
		CloseAllWindows();

		var currentObjectForTurningWindow = transform;
		
		while (true)
		{
			if (currentObjectForTurningWindow == null)
				break;
			
			ShowWindow(currentObjectForTurningWindow.gameObject);
			
			currentObjectForTurningWindow = currentObjectForTurningWindow.parent;
		}

		void ShowWindow(GameObject obj)
		{
			var window = obj.GetComponent<CanvasGroup>();

			if (window != null)
			{
				window.blocksRaycasts = true;
				window.alpha = 1;
			}
		}
	}
	
	[ContextMenu("CloseThisWindow")]
	private void CloseThisWindow()
	{
		var window = gameObject.GetComponent<CanvasGroup>();
		window.blocksRaycasts = false;
		window.alpha = 0;
	}
#endif

	public static WindowView GetLastOpenedWindow()
	{
		return _previousOpenedWindows.Peek();
	}
	
	public static void ClosePreviousWindow()
	{
		if (_previousOpenedWindows.Count > 0)
		{
			var retrievedPreviousWindow = _previousOpenedWindows.Pop();
			
			retrievedPreviousWindow.canvasGroup.blocksRaycasts = false;
			retrievedPreviousWindow.canvasGroup.DOFade(0, k_transitionDuration).
				OnComplete(() => retrievedPreviousWindow.onFinishedClosingWindow?.Invoke());
		}
	}

	public static void ClosePreviousAndShowThisWindow(WindowView windowForShow, Action onShowingCompleted = null)
	{
		ClosePreviousWindow();
		
		windowForShow.onStartOpeningWindow?.Invoke();
		windowForShow.canvasGroup.blocksRaycasts = true;
		windowForShow.canvasGroup.DOFade(1, k_transitionDuration).OnComplete(() =>
		{
			onShowingCompleted?.Invoke();
		});
		
		_previousOpenedWindows.Push(windowForShow);
	}

	public bool IsWindowOpened()
	{
		return canvasGroup.alpha > 0.01f;
	}

	// Useful for view buttons on UI to set on Add listener in Unity Editor
	public void ClosePreviousAndShowThisWindow() => ClosePreviousAndShowThisWindow(null);

	public void ClosePreviousAndShowThisWindow(Action onShowingCompleted = null)
	{
		ClosePreviousWindow();
		
		onStartOpeningWindow?.Invoke();
		canvasGroup.blocksRaycasts = true;
		canvasGroup.DOFade(1, k_transitionDuration).OnComplete(() =>
		{
			onShowingCompleted?.Invoke();
		});
		
		_previousOpenedWindows.Push(this);
	}

	// Useful for view buttons on UI to set on Add listener in Unity Editor
	public void ShowWindowUponAnotherWindows(bool show) => ShowWindowUponAnotherWindows(show, null);
	
	public void ShowWindowUponAnotherWindows(bool show, Action onShowingCompleted = null, bool pushIntoStackOfOpenedWindows = true)
	{
		if(show)
			onStartOpeningWindow?.Invoke();
		
		canvasGroup.blocksRaycasts = show;
		canvasGroup.DOFade(show ? 1 : 0, k_transitionDuration).OnComplete(() =>
		{
			onShowingCompleted?.Invoke();
			
			if(!show)
				onFinishedClosingWindow?.Invoke();
		});
		
		if(!pushIntoStackOfOpenedWindows)
			return;

		if (show)
		{
			_previousOpenedWindows.Push(this);
		}
		else
		{
			var listOfWindows = _previousOpenedWindows.ToList();
			listOfWindows.Remove(this);
			_previousOpenedWindows = new Stack<WindowView>(listOfWindows);
		}
	}

	protected override CanvasGroup GetCanvasGroup()
	{
		return canvasGroup;
	}
}
