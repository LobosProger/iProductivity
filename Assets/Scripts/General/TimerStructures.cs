using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletingActivity
{
	private string titleAcitvity;
	private TimeSpan overallTimeForActivity;
	private DateTime notificationTime;

	public void SetActivityData(string titleAcitvity, TimeSpan overallTimeForActivity)
	{
		this.titleAcitvity = titleAcitvity;
		this.overallTimeForActivity = overallTimeForActivity;
		this.notificationTime = DateTime.Now.AddSeconds(this.overallTimeForActivity.TotalSeconds);
	}

	public TimeSpan GetRemainedTimeForCompletion()
	{
		return overallTimeForActivity;
	}

	public string GetNameOfActivity()
	{
		return titleAcitvity;
	}
}
