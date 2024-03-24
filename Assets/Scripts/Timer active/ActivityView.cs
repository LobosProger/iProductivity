using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivityView : MonoBehaviour
{
    [SerializeField] private TMP_Text inProgressActivity;

    public void ShowOnTimerCurrentInProgressActivity(string activityName)
    {
		inProgressActivity.text = activityName;
	}
}
