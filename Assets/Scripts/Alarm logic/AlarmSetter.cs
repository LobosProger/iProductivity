using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications;
using UnityEngine;

public class AlarmSetter : MonoBehaviour
{
	public bool IsInit { get; private set; }

	public static AlarmSetter Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private IEnumerator Start()
	{
		var args = NotificationCenterArgs.Default;
		args.AndroidChannelId = "defadevdfgfgvdsvsdvult";
		args.AndroidChannelName = "Notificvfggfdssvdsvdations";
		args.AndroidChannelDescription = "Maifggfn nosdvsvdsvdsvdsvdtifications";
		args.PresentationOptions = NotificationPresentation.Alert;
		NotificationCenter.Initialize(args);

		var request = NotificationCenter.RequestPermission();
		yield return new WaitUntil(() => request.Status != NotificationsPermissionStatus.RequestPending);
		if (request.Status == NotificationsPermissionStatus.RequestPending)
		{
			yield return request;
		}
			
		if(request.Status == NotificationsPermissionStatus.Denied)
		{
			Debug.LogWarning("Denied init of notification system!");
			yield return null;
		} else
		{
			IsInit = true;
		}
	}

	public void SetNewAlarmNotification(string notificationTitle, string notificationText, int secondsAppearing)
	{
		var notification = new Notification()
		{
			Title = notificationTitle,
			Text = notificationText,
		};

		var dateTime = DateTime.Now.AddSeconds(secondsAppearing);
		NotificationCenter.ScheduleNotification(notification, new NotificationDateTimeSchedule(dateTime));
		NotificationCenter.ScheduleNotification(notification, new NotificationDateTimeSchedule(dateTime.AddSeconds(6)));
		NotificationCenter.ScheduleNotification(notification, new NotificationDateTimeSchedule(dateTime.AddSeconds(12)));
		NotificationCenter.ScheduleNotification(notification, new NotificationDateTimeSchedule(dateTime.AddSeconds(18)));
		NotificationCenter.ScheduleNotification(notification, new NotificationDateTimeSchedule(dateTime.AddSeconds(24)));
		NotificationCenter.ScheduleNotification(notification, new NotificationDateTimeSchedule(dateTime.AddSeconds(31)));
	}

	public void ClearAllAlarmNotifications()
	{
		NotificationCenter.CancelAllScheduledNotifications();
		NotificationCenter.CancelAllDeliveredNotifications();
	}
}
