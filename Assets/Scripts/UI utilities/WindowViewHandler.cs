using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class WindowViewHandler : MonoBehaviour
{
    [SerializeField] private WindowView linkedWindow;
    
    [SerializeField] private UnityEvent onStartOpeningWindow;
    [SerializeField] private UnityEvent onFinishedClosingWindow;

    private void Start()
    {
        var handlingAction = linkedWindow.IsWindowOpened() ? onStartOpeningWindow : onFinishedClosingWindow;
        handlingAction?.Invoke();

        linkedWindow.onStartOpeningWindow += OnStartOpeningWindow;
        linkedWindow.onFinishedClosingWindow += OnFinishedClosingWindow;
    }

    private void OnDestroy()
    {
        linkedWindow.onStartOpeningWindow -= OnStartOpeningWindow;
        linkedWindow.onFinishedClosingWindow -= OnFinishedClosingWindow;
    }

    [Button]
    private void OnStartOpeningWindow()
    {
        onStartOpeningWindow?.Invoke();
    }
    
    [Button]
    private void OnFinishedClosingWindow()
    {
        onFinishedClosingWindow?.Invoke();
    }
}
