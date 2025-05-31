using System;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TextPreferredWidth : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _maxPreferredWidth;
    
    private void Awake()
    {
        _text.OnPreRenderText += HandlePreferredWidthOfText;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
            return;
        
        HandlePreferredWidthOfText(null);
    }
#endif

    private void HandlePreferredWidthOfText(TMP_TextInfo _)
    {
        var finalPreferredWidth = _text.preferredWidth < _maxPreferredWidth ? _text.preferredWidth : _maxPreferredWidth;
        _text.rectTransform.sizeDelta = new Vector2(finalPreferredWidth, _text.rectTransform.sizeDelta.y);
    }
}
