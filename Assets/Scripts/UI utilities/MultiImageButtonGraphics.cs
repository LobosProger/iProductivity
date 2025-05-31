using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class MultiImageButtonGraphics : MonoBehaviour
{
    [SerializeField] private MultiImageButton _multiImageButton;
    [Space]
    [SerializeField] private Graphic[] _graphics;
    
    [Button]
    private void SetMultipleGraphics()
    {
        Debug.Log("Set multiple graphics");
        _multiImageButton.SetMultipleGraphics(_graphics);
    }
}
