using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JongCo.Easing;
using UnityEngine;
using UnityEngine.UIElements;

public class TypoEffectUIController : MonoBehaviour
{

    private PanelRenderer uiRenderer;
    private PanelRenderer.UIReloadCallback uiRegisterCallbackHandler;

    private UniTaskCompletionSource tcs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiRenderer = GetComponent<PanelRenderer>();
        gameObject.SetActive(false);
    }

    public UniTask PlayTypoEffect(string text, Color color)
    {
        tcs = new();

        uiRegisterCallbackHandler = (panelRenderer, root) =>
        {
            Label effectLabel = root.Q<Label>("EffectLabel");
            Label effectLabelBlur = root.Q<Label>("EffectLabelBlur");
            VisualElement bg = root.Q<VisualElement>("ScreenElement");
            effectLabel.text = text;
            effectLabelBlur.text = text;
            effectLabel.style.color = color;
            TypeAnimation(new Label[] {effectLabel,effectLabelBlur}, bg).Forget();
        };

        uiRenderer.RegisterUIReloadCallback(uiRegisterCallbackHandler);
        gameObject.SetActive(true);

        return tcs.Task;
    }

    private async UniTask TypeAnimation(IEnumerable<Label> target, VisualElement bg)
    {
        float progress = 0;
        float duration = 1.2f;
        while (progress < duration)
        {
            float r = SingleAxisBezier.CubicBezier(Preset.FastInSlowOut2, progress/duration);
            foreach(Label label in target)
            {
                label.style.letterSpacing = new Length(20 + (1-r) * 100f, LengthUnit.Pixel);
                label.style.opacity = r;
            }
            bg.style.opacity = r;
            await UniTask.WaitForEndOfFrame();
            progress += Time.deltaTime;
        }
        bg.style.opacity = 1;
        foreach(Label label in target)
        {
            label.style.letterSpacing = new Length(20);
        }
        
        progress = 0;
        duration = 0.5f;
        while (progress < duration)
        {
            float r = progress / duration;
            foreach(Label label in target)
            {
                label.style.opacity = 1-r;
            }
            bg.style.opacity = 1-r;
            await UniTask.WaitForEndOfFrame();
            progress += Time.deltaTime;
        }

        gameObject.SetActive(false);
        tcs.TrySetResult();
    }



    
}
