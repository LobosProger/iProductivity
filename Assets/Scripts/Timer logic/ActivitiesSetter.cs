using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivitiesSetter : MonoBehaviour
{
    [SerializeField] private ActivityBoxView _activityBoxViewTemplate;
    [SerializeField] private Transform _activityBoxesContainer;
    [Space]
    [SerializeField] private ActivityAdderView _activityAdderView;

    public static event Action<string> onSelectedActivity; 

    private Dictionary<string, ActivityBoxView> _activities = new();
    
    private void Start()
    {
        // By default, disabling it
        _activityBoxViewTemplate.gameObject.SetActive(false);

        _activityAdderView.onActivityAdded += AddNewActivityToServer;
        ActivityBoxView.onSelectedActivity += SelectActivity;
        ActivityBoxView.onDeleteActivity += DeleteActivityFromServer;
        
        // Subscribe to activity manager events
        ActivityManager.instance.onActivitiesLoaded += LoadActivitiesFromServer;
        ActivityManager.instance.onActivityCreated += OnActivityCreatedOnServer;
        ActivityManager.instance.onActivityDeleted += OnActivityDeletedFromServer;
    }

    private void OnDestroy()
    {
        _activityAdderView.onActivityAdded -= AddNewActivityToServer;
        ActivityBoxView.onSelectedActivity -= SelectActivity;
        ActivityBoxView.onDeleteActivity -= DeleteActivityFromServer;
        
        if (ActivityManager.instance != null)
        {
            ActivityManager.instance.onActivitiesLoaded -= LoadActivitiesFromServer;
            ActivityManager.instance.onActivityCreated -= OnActivityCreatedOnServer;
            ActivityManager.instance.onActivityDeleted -= OnActivityDeletedFromServer;
        }
    }

    private async void AddNewActivityToServer(string activityName)
    {
        if (_activities.ContainsKey(activityName))
        {
            Debug.LogWarning($"Activity with name '{activityName}' already exists");
            return;
        }
        
        var success = await ActivityManager.instance.CreateActivityAsync(activityName);
        if (!success)
        {
            Debug.LogError($"Failed to create activity: {activityName}");
        }
    }

    private void OnActivityCreatedOnServer(ActivityResponse activity)
    {
        CreateActivityBoxInUI(activity);
    }

    private void CreateActivityBoxInUI(ActivityResponse activity)
    {
        var creatingNewActivityBlock = Instantiate(_activityBoxViewTemplate, _activityBoxesContainer);
        creatingNewActivityBlock.InitActivityBox(activity.name, activity.hash);

        // Make activity block topper than add activity block
        var reorderingSiblingIndex = creatingNewActivityBlock.transform.GetSiblingIndex() - 1;
        creatingNewActivityBlock.transform.SetSiblingIndex(reorderingSiblingIndex);
        creatingNewActivityBlock.gameObject.SetActive(true);
        
        _activities.Add(activity.name, creatingNewActivityBlock);
    }

    private void LoadActivitiesFromServer(List<ActivityResponse> activities)
    {
        // Clear existing activities
        ClearAllActivities();
        
        // Create UI for each activity from server
        foreach (var activity in activities)
        {
            CreateActivityBoxInUI(activity);
        }
        
        Debug.Log($"Loaded {activities.Count} activities from server");
    }

    private void ClearAllActivities()
    {
        foreach (var activityKvp in _activities)
        {
            if (activityKvp.Value != null && activityKvp.Value.gameObject != null)
            {
                Destroy(activityKvp.Value.gameObject);
            }
        }
        _activities.Clear();
    }

    private void SelectActivity(string activityName)
    {
        Debug.Log($"Selected activity: {activityName}");
        onSelectedActivity?.Invoke(activityName);
    }

    private async void DeleteActivityFromServer(string activityName)
    {
        // Find the activity hash by name
        var activity = ActivityManager.instance.FindActivityByName(activityName);
        if (activity == null)
        {
            Debug.LogError($"Activity not found: {activityName}");
            return;
        }

        var success = await ActivityManager.instance.DeleteActivityAsync(activity.hash);
        if (!success)
        {
            Debug.LogError($"Failed to delete activity: {activityName}");
        }
    }

    private void OnActivityDeletedFromServer(string activityHash)
    {
        // Find and remove activity from UI by hash
        var activityToRemove = _activities.FirstOrDefault(kvp => 
        {
            var activity = ActivityManager.instance.FindActivityByName(kvp.Key);
            return activity?.hash == activityHash;
        });

        if (!activityToRemove.Equals(default(KeyValuePair<string, ActivityBoxView>)))
        {
            if (activityToRemove.Value != null)
            {
                Destroy(activityToRemove.Value.gameObject);
            }
            _activities.Remove(activityToRemove.Key);
        }
    }
}