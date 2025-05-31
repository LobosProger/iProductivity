using System;
using UnityEngine;

// Make this class, not interface, to have ability assign as SerializeField
public abstract class IWindowView : MonoBehaviour
{
    public Action<IWindowView> onStartOpeningWindowHandler;
    public Action<IWindowView> onClosedFullyWindowHandler;

    protected void OnStartOpeningWindowHandler()
    {
        onStartOpeningWindowHandler?.Invoke(this);
    }
    
    protected void OnClosedFullyWindowHandler()
    {
        onClosedFullyWindowHandler?.Invoke(this);
    }
    
    protected abstract CanvasGroup GetCanvasGroup();

    public bool IsWindowVisible()
    {
        // When window start becoming visible by animation transition, it turns on this flag on, so by it value we can recognize, when window start
        // becoming visible, or initially is not visible. In comparison with alpha value, when we for example have in window objects, which
        // initially turned off, and they check is opened window or not based on the alpha - is not efficient, because need some time elapsed
        // when we can say that window is opened (because based on the animation), so the blocks raycasts is more efficient when for instance,
        // we have any kind optimizers when they checks after becoming active as game objects and invoking Start method
        return GetCanvasGroup().blocksRaycasts;
    }
}
