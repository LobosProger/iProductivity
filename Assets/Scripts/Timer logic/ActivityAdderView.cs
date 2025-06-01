using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityAdderView : MonoBehaviour
{
    [SerializeField] private TMP_InputField _activityInputField;
    [SerializeField] private Button _addActivityButton;
    
    public event Action<string> onActivityAdded; 

    private void Start()
    {
        _addActivityButton.onClick.AddListener(AddNewActivity);
    }

    private void AddNewActivity()
    {
        var isTextNull = string.IsNullOrEmpty(_activityInputField.text);
        if (isTextNull)
            return;
        
        onActivityAdded?.Invoke(_activityInputField.text);
        _activityInputField.text = "";
    }
}
