using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSelectionUI : MonoBehaviour
{
    private PanelRenderer uiRenderer;
    private PanelRenderer.UIReloadCallback uiRegisterCallbackHandler;
    
    // CardSelectionButton[] buttons;
    private UniTaskCompletionSource<Hwatoo> tcs;
    private List<System.Action> registeredClickHandlers = new();

    void Start()
    {
        uiRenderer = GetComponent<PanelRenderer>();
        gameObject.SetActive(false);
    }


    /// <summary>
    /// 화투 패 선택 UI를 표시하고 유저가 선택한 Hwatoo를 반환합니다.
    /// </summary>
    public UniTask<Hwatoo> AskSelection(List<Hwatoo> choices)
    {
        print("선택 호출됨");
        tcs = new UniTaskCompletionSource<Hwatoo>();
        gameObject.SetActive(true);
        
        uiRegisterCallbackHandler = (panelRenderer, root) => {
            List<Button> buttons = root.Query<Button>(className: "hwatoo-card").ToList();

            // 이전에 등록된 핸들러 전부 제거
            for(int i = 0; i < registeredClickHandlers.Count && i < buttons.Count; i++)
                buttons[i].clicked -= registeredClickHandlers[i];
            registeredClickHandlers.Clear();

            for(int i = 0; i < buttons.Count; i++) {
                int cap = i;
                System.Action handler = () => OnCardSelected(choices[cap]);
                buttons[cap].clicked += handler;
                buttons[cap].style.backgroundImage = new StyleBackground(choices[cap].hwatooData.sprite);
                registeredClickHandlers.Add(handler);
            }
        };

        uiRenderer.RegisterUIReloadCallback(uiRegisterCallbackHandler);
        return tcs.Task;
    }

    private void OnCardSelected(Hwatoo hwatoo)
    {
        gameObject.SetActive(false);
        uiRenderer.UnregisterUIReloadCallback(uiRegisterCallbackHandler);
        tcs.TrySetResult(hwatoo);
    }
}
