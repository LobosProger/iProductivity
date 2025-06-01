using System;
using UnityEngine;

public class TopDownPanel : MonoBehaviour
{
    [SerializeField] private ExtendedToggle _activitiesToggle;
    [SerializeField] private ExtendedToggle _timerToggle;
    [SerializeField] private ExtendedToggle _analyticsToggle;
    
    public static TopDownPanel instance { get; private set; }
    
    private void Awake()
    {
        instance = this;
    }

    public void ShowActivitiesWindow()
    {
        _activitiesToggle.SetValueOfToggle(true);
    }
}
