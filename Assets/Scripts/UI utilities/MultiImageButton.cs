using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(MultiImageButtonGraphics))]
public class MultiImageButton : Button
{
    [HideInInspector]
    [SerializeField] public Graphic[] graphics;
    
    protected Graphic[] Graphics
    {
        get
        {
            if (this.graphics == null)
            {
                this.graphics = targetGraphic.transform.GetComponentsInChildren<Graphic>();
            }

            return this.graphics;
        }
    }


    protected override void DoStateTransition (SelectionState state, bool instant)
    {
        Color color;
        switch (state)
        {
            case (Selectable.SelectionState.Normal):
                color = this.colors.normalColor;
                break;

            case (Selectable.SelectionState.Highlighted):
                color = this.colors.highlightedColor;
                break;

            case (Selectable.SelectionState.Pressed):
                color = this.colors.pressedColor;
                break;

            case (Selectable.SelectionState.Disabled):
                color = this.colors.disabledColor;
                break;
            
            default:
                color = this.colors.normalColor;
                break;
        }

        if (base.gameObject.activeInHierarchy)
        {
            switch (this.transition)
            {
                case (Selectable.Transition.ColorTint):
                    this.ColorTween(color * this.colors.colorMultiplier, instant);
                    break;

                default:
                    throw new System.NotSupportedException();
            }
        }
    }


    private void ColorTween(Color targetColor, bool instant)
    {
        if (this.targetGraphic == null)
        {
            return;
        }

        foreach (var g in this.Graphics)
        {
            g.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
        }
    }

    public void SetMultipleGraphics(Graphic[] settingGraphics)
    {
        this.graphics = settingGraphics;
    }
}