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

        _activityAdderView.onActivityAdded += AddNewActivity;
        ActivityBoxView.onSelectedActivity += SelectActivity;
        ActivityBoxView.onDeleteActivity += DeleteActivity;
    }

    private void OnDestroy()
    {
        _activityAdderView.onActivityAdded -= AddNewActivity;
        ActivityBoxView.onSelectedActivity -= SelectActivity;
        ActivityBoxView.onDeleteActivity -= DeleteActivity;
    }

    private void AddNewActivity(string activityName)
    {
        if (_activities.ContainsKey(activityName))
            return;
        
        var creatingNewActivityBlock = Instantiate(_activityBoxViewTemplate, _activityBoxesContainer);
        creatingNewActivityBlock.InitActivityBox(activityName);

        // Make activity block topper than add activity block
        var reorderingSiblingIndex = creatingNewActivityBlock.transform.GetSiblingIndex() - 1;
        creatingNewActivityBlock.transform.SetSiblingIndex(reorderingSiblingIndex);
        creatingNewActivityBlock.gameObject.SetActive(true);
        
        _activities.Add(activityName, creatingNewActivityBlock);
    }

    private void SelectActivity(string activityName)
    {
        Debug.Log($"Selected activity: {activityName}");
        onSelectedActivity?.Invoke(activityName);
    }

    private void DeleteActivity(string activityName)
    {
        // // After deletion each time select first one activity
        // var firstSelectedBlockKvp = _activities.FirstOrDefault();
        // SelectActivity(firstSelectedBlockKvp.Key);

        var removingActivity = _activities[activityName];
        Destroy(removingActivity.gameObject);
        _activities.Remove(activityName);
    }
}
