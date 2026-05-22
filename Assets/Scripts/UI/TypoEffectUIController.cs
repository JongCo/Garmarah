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
            VisualElement bg = root.Q<VisualElement>("ScreenElement");
            effectLabel.text = text;
            effectLabel.style.color = color;
            TypeAnimation(effectLabel, bg).Forget();
        };

        uiRenderer.RegisterUIReloadCallback(uiRegisterCallbackHandler);
        gameObject.SetActive(true);

        return tcs.Task;
    }

    private async UniTask TypeAnimation(Label target, VisualElement bg)
    {
        float progress = 0;
        float duration = 1.5f;
        while (progress < duration)
        {
            float r = SingleAxisBezier.CubicBezier(Preset.FastInSlowOut2, progress/duration);
            target.style.letterSpacing = new Length(20 + (1-r) * 100f, LengthUnit.Pixel);
            bg.style.opacity = r;
            await UniTask.WaitForEndOfFrame();
            progress += Time.deltaTime;
        }
        bg.style.opacity = 1;
        target.style.letterSpacing = new Length(20);
        
        progress = 0;
        duration = 0.7f;
        while (progress < duration)
        {
            float r = progress / duration;
            bg.style.opacity = 1-r;
            await UniTask.WaitForEndOfFrame();
            progress += Time.deltaTime;
        }

        gameObject.SetActive(false);
        tcs.TrySetResult();
    }



    
}
