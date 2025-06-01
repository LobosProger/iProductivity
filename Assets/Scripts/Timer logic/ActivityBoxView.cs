using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityBoxView : MonoBehaviour
{
    [SerializeField] private ExtendedToggle _extendedToggle;
    [SerializeField] private TMP_Text _activityText;
    [SerializeField] private Button _deleteButton;

    public static event Action<string> onSelectedActivity; 
    public static event Action<string> onDeleteActivity; 
    
    private string _activityName;
    private string _activityHash;
    
    private void Start()
    {
        _extendedToggle.onSwitchedToggle.AddListener(SelectActivity);
        _deleteButton.onClick.AddListener(DeleteActivity);
    }

    private void SelectActivity(bool select)
    {
        if (select)
        {
            onSelectedActivity?.Invoke(_activityName);
        }
    }

    private void DeleteActivity()
    {
        onDeleteActivity?.Invoke(_activityName);
    }

    public void InitActivityBox(string activityName, string activityHash = null)
    {
        _activityName = activityName;
        _activityHash = activityHash;
        _activityText.text = _activityName;
    }

    public void SelectActivity()
    {
        _extendedToggle.SetIsOnWithoutNotify(true);
    }

    public string GetActivityHash()
    {
        return _activityHash;
    }

    public string GetActivityName()
    {
        return _activityName;
    }
}