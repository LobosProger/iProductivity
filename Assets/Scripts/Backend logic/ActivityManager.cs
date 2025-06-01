using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActivityManager : MonoBehaviour
{
    public static ActivityManager instance { get; private set; }
    
    public event Action<List<ActivityResponse>> onActivitiesLoaded;
    public event Action<ActivityResponse> onActivityCreated;
    public event Action<string> onActivityDeleted;
    public event Action<ActivityResponse> onActivityStarted;
    public event Action<ActivityResponse> onActivityEnded;

    private List<ActivityResponse> _userActivities = new();
    private ActivityResponse _currentRunningActivity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async UniTask LoadUserActivitiesAsync()
    {
        var responseText = await ApiClient.instance.GetRequestAsync("/api/v1/activities/");

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                // Parse array response manually since Unity doesn't handle arrays well
                var activitiesArrayWrapper = JsonUtility.FromJson<ActivityArrayWrapper>("{\"activities\":" + responseText + "}");
                _userActivities = new List<ActivityResponse>(activitiesArrayWrapper.activities);
                onActivitiesLoaded?.Invoke(_userActivities);
                Debug.Log($"Loaded {_userActivities.Count} user activities");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse activities: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError("Failed to load activities");
        }
    }

    public async UniTask<bool> CreateActivityAsync(string activityName)
    {
        var activityData = new ActivityCreate
        {
            name = activityName
        };

        var jsonData = JsonUtility.ToJson(activityData);
        var responseText = await ApiClient.instance.PostRequestAsync("/api/v1/activities/", jsonData);

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                var newActivity = JsonUtility.FromJson<ActivityResponse>(responseText);
                _userActivities.Add(newActivity);
                onActivityCreated?.Invoke(newActivity);
                Debug.Log($"Activity created: {newActivity.name}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse create activity response: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError("Failed to create activity");
            return false;
        }
    }

    public async UniTask<bool> DeleteActivityAsync(string activityHash)
    {
        var success = await ApiClient.instance.DeleteRequestAsync($"/api/v1/activities/{activityHash}");

        if (success)
        {
            _userActivities.RemoveAll(activity => activity.hash == activityHash);
            onActivityDeleted?.Invoke(activityHash);
            Debug.Log($"Activity deleted: {activityHash}");
            return true;
        }
        else
        {
            Debug.LogError("Failed to delete activity");
            return false;
        }
    }

    public async UniTask<bool> StartActivityAsync(string activityHash)
    {
        var startData = new ActivityStart
        {
            activity_hash = activityHash
        };

        var jsonData = JsonUtility.ToJson(startData);
        var responseText = await ApiClient.instance.PostRequestAsync("/api/v1/activities/start", jsonData);

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                _currentRunningActivity = JsonUtility.FromJson<ActivityResponse>(responseText);
                onActivityStarted?.Invoke(_currentRunningActivity);
                Debug.Log($"Activity started: {_currentRunningActivity.name}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse start activity response: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError("Failed to start activity");
            return false;
        }
    }

    public async UniTask<bool> EndActivityAsync(string activityHash)
    {
        var endData = new ActivityEnd
        {
            activity_hash = activityHash
        };

        var jsonData = JsonUtility.ToJson(endData);
        var responseText = await ApiClient.instance.PostRequestAsync("/api/v1/activities/end", jsonData);

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                var endedActivity = JsonUtility.FromJson<ActivityResponse>(responseText);
                if (_currentRunningActivity?.hash == activityHash)
                {
                    _currentRunningActivity = null;
                }
                onActivityEnded?.Invoke(endedActivity);
                Debug.Log($"Activity ended: {endedActivity.name}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse end activity response: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError("Failed to end activity");
            return false;
        }
    }

    public async UniTask<ActivityResponse> GetCurrentRunningActivityAsync()
    {
        var responseText = await ApiClient.instance.GetRequestAsync("/api/v1/activities/current/running");

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                _currentRunningActivity = JsonUtility.FromJson<ActivityResponse>(responseText);
                Debug.Log($"Current running activity: {_currentRunningActivity?.name ?? "None"}");
                return _currentRunningActivity;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse current activity response: {ex.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogError("Failed to get current running activity");
            return null;
        }
    }

    public ActivityResponse FindActivityByName(string activityName)
    {
        return _userActivities.Find(activity => activity.name == activityName);
    }

    public List<ActivityResponse> GetAllActivities()
    {
        return new List<ActivityResponse>(_userActivities);
    }

    public ActivityResponse GetCurrentRunningActivity()
    {
        return _currentRunningActivity;
    }
}