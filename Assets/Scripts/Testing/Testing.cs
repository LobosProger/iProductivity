using System;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private bool _test;
    [SerializeField] private int minutes;
    [SerializeField] private int seconds;
    
    private void Update()
    {
        if (!_test)
        {
            return;
        }
        
        _test = false;

        var comp = GetComponent<TimerController>();
        comp.SetTimeOfTimer(new TimeSpan(minutes: minutes, seconds: seconds, hours: 0));
        comp.LaunchSessionOfTimer();
    }
}
