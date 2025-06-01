using System;
using SingularityGroup.HotReload;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ExtendedToggle : MonoBehaviour
{
    /// <summary>
    /// Usable for scripts
    /// </summary>
    public UnityEvent<bool> onSwitchedToggle;
    
    // Usable for views in editor
    public UnityEvent onToggledOnView; 
    public UnityEvent onToggledOffView;
    
    private Toggle _toggle;
    private bool _previousToggleValue;
    
    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggled);

        _previousToggleValue = _toggle.isOn;
        SetIsOnWithoutNotify(_toggle.isOn);
    }

    private void OnToggled(bool value)
    {
        if (value == _previousToggleValue)
            return;
        
        _previousToggleValue = _toggle.isOn;
        
        var invokingAction = value ? onToggledOnView : onToggledOffView;
        invokingAction?.Invoke();
        
        onSwitchedToggle?.Invoke(value);
    }

    public void SetIsOnWithoutNotify(bool value)
    {
        _toggle.SetIsOnWithoutNotify(value);
        
        var invokingAction = value ? onToggledOnView : onToggledOffView;
        invokingAction?.Invoke();
    }

    public bool GetValueOfToggle()
    {
        return _toggle.isOn;
    }

    public void SetValueOfToggle(bool value)
    {
        _toggle.isOn = value;
    }
}