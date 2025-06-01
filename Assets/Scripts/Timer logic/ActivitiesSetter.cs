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

    private Dictionary<string, ActivityBoxView> _activities = new(); // Key: activity name
    private Dictionary<string, ActivityBoxView> _activitiesByHash = new(); // Key: activity hash
    
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
            Debug.LogWarning($"Activity with name '{activityName}' already exists in UI");
            return;
        }
        
        Debug.Log($"Creating new activity: {activityName}");
        
        var success = await ActivityManager.instance.CreateActivityAsync(activityName);
        if (!success)
        {
            Debug.LogError($"Failed to create activity on server: {activityName}");
        }
        else
        {
            Debug.Log($"Successfully created activity on server: {activityName}");
        }
    }

    private void OnActivityCreatedOnServer(ActivityResponse activity)
    {
        Debug.Log($"Activity created on server, adding to UI: {activity.name} (Hash: {activity.hash})");
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
        _activitiesByHash.Add(activity.hash, creatingNewActivityBlock);
    }

    private void LoadActivitiesFromServer(List<ActivityResponse> activities)
    {
        Debug.Log($"Loading {activities.Count} activities from server");
        
        // Clear existing activities
        ClearAllActivities();
        
        // Create UI for each activity from server
        foreach (var activity in activities)
        {
            Debug.Log($"Loading activity: {activity.name} (Hash: {activity.hash})");
            CreateActivityBoxInUI(activity);
        }
        
        Debug.Log($"Successfully loaded {activities.Count} activities from server");
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
        _activitiesByHash.Clear();
    }

    private void SelectActivity(string activityName)
    {
        Debug.Log($"Selected activity: {activityName}");
        onSelectedActivity?.Invoke(activityName);
    }

    private async void DeleteActivityFromServer(string activityName)
    {
        Debug.Log($"Deleting activity: {activityName}");
        
        // Find the activity hash by name
        var activity = ActivityManager.instance.FindActivityByName(activityName);
        if (activity == null)
        {
            Debug.LogError($"Activity not found in ActivityManager: {activityName}");
            return;
        }

        Debug.Log($"Found activity to delete: Name={activity.name}, Hash={activity.hash}");
        
        var success = await ActivityManager.instance.DeleteActivityAsync(activity.hash);
        if (!success)
        {
            Debug.LogError($"Failed to delete activity from server: {activityName}");
        }
        else
        {
            Debug.Log($"Successfully deleted activity from server: {activityName}");
        }
    }

    private void OnActivityDeletedFromServer(string activityHash)
    {
        // Find activity by hash directly
        if (_activitiesByHash.ContainsKey(activityHash))
        {
            var activityBoxView = _activitiesByHash[activityHash];
            var activityName = activityBoxView.GetActivityName();
            
            // Remove from UI
            Destroy(activityBoxView.gameObject);
            
            // Remove from both dictionaries
            _activities.Remove(activityName);
            _activitiesByHash.Remove(activityHash);
            
            Debug.Log($"Activity removed from UI: {activityName}");
        }
        else
        {
            Debug.LogWarning($"Activity with hash {activityHash} not found in UI");
        }
    }
}