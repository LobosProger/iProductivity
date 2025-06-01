using System;
using UnityEngine;

public class TopDownPanel : MonoBehaviour
{
    [SerializeField] private ExtendedToggle _activitiesToggle;
    [SerializeField] private ExtendedToggle _timerToggle;
    [SerializeField] private ExtendedToggle _analyticsToggle;
    
    public static TopDownPanel instance { get; private set; }
    
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        instance = this;

        _canvasGroup = GetComponent<CanvasGroup>();
        HidePanel();
    }

    public void ShowActivitiesWindow()
    {
        _activitiesToggle.SetValueOfToggle(true);
    }

    public void ShowPanel()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }
    
    public void HidePanel()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
}
